using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ChestNode : Node
{
    [Header("Chest Settings")]
    public GameObject rootObject;
    public GameObject rootLock;
    public GameObject rootHinge;
    public WorldPrompt promptObject;
    private bool playerInRange;

    [Header("Open Settings")]
    //public bool bounce = true;
    public float targetAngle = 100f;
    public float maxSwing = 80f;
    public float openFrequency = 2.5f;
    public float damping = 2.0f;
    private bool canOpen = false;

    [Header("Shake Settings")]
    public ShakeType shakeType = ShakeType.Smooth;
    // Jitter
    public float shakeForce = 0.05f;
    // Smooth Shake
    public float maxAngle = 10f;
    public float shakeFrequency = 6f;
    public float decay = 6f;

    [Header("Keybinds")]
    public KeyCode interactKey;
    public bool interactPressed = false;

    private void Start()
    {
        if (health == 1f)
        {
            if (rootLock != null)
            {
                Destroy(rootLock);
                promptObject.UpdatePrompt("Press E");
                canOpen = true;
                StartCoroutine(ShakeChest(0.75f));
            }
        }

        base.Start();
    }

    private void Update()
    {
        interactPressed = Input.GetKey(interactKey);
        if (interactPressed && playerInRange)
        {
            if (health == 1f) { TakeDamage(1f); }
        }
        /*if (Input.GetKeyDown(interactKey))
        {
            interactPressed = true;
            if (playerInRange)
            {
                if (health == 1f) { TakeDamage(1f); }
            }
        }
        else if (Input.GetKeyUp(interactKey))
        {
            interactPressed = false;
        }//*/
    }

    public override void TakeDamage(float damage)
    {
        if (isDestroyed) return;

        if (canOpen)
        {
            if (interactPressed) { damage = health; }
            else 
            {
                damage = 0f;
                StartCoroutine(ShakeChest(0.75f));
            }
        }
        else // if can't open
        {
            if (damage >= health) { damage = health - 1f; } // if damage goes over 1 hp left
            StartCoroutine(ShakeChest(0.75f));
        }

        base.TakeDamage(damage);
    }

    private IEnumerator ShakeChest(float duration = 1f)
    {
        float elapsed = 0f;

        GameObject focusedObject = null;
        if (rootLock != null) { focusedObject = rootLock; }
        else if (rootObject != null) { focusedObject = rootObject; }

        if (focusedObject == null) { StopAllCoroutines(); Debug.Log("Stop Shake"); }

        // Store bush transforms
        Vector3 rootOriginalPos = focusedObject.transform.position;
        Quaternion rootOriginalRot = focusedObject.transform.rotation;

        // check effect type
        if (shakeType == ShakeType.Jittery)
        {
            while (elapsed < duration)
            {
                focusedObject.transform.position = rootOriginalPos + new Vector3(
                    Random.Range(-shakeForce, shakeForce),
                    Random.Range(-shakeForce, shakeForce),
                    Random.Range(-shakeForce, shakeForce)
                );

                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        else if (shakeType == ShakeType.Smooth)
        {
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float decayFactor = Mathf.Exp(-t * decay);

                Vector3 smoothAngle = new Vector3(
                    maxAngle * Mathf.Sin((t * shakeFrequency * Mathf.PI) + Random.Range(0f, Mathf.PI)) * decayFactor,
                    maxAngle * Mathf.Sin((t * shakeFrequency * Mathf.PI) + Random.Range(0f, Mathf.PI)) * decayFactor,
                    maxAngle * Mathf.Sin((t * shakeFrequency * Mathf.PI) + Random.Range(0f, Mathf.PI)) * decayFactor
                );

                focusedObject.transform.rotation = rootOriginalRot * Quaternion.Euler(smoothAngle);

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        // Reset transform
        focusedObject.transform.position = rootOriginalPos;
        focusedObject.transform.rotation = rootOriginalRot;

        // Stop lock from playing again
        if (health == 1f)
        {
            if (rootLock != null)
            {
                StopAllCoroutines();
                Destroy(rootLock);
                promptObject.UpdatePrompt("Press E");
                canOpen = true;
            }
        }
    }

    private IEnumerator OpenChest(float duration = 1f)
    {
        float elapsed = 0f;

        Quaternion originalRotation = rootHinge.transform.rotation;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float decayFactor = Mathf.Exp(-damping * t);  // Exponential decay

            // Compute oscillation using sine wave and decay
            float angleX = -targetAngle + maxSwing * Mathf.Sin(openFrequency * Mathf.PI * t) * decayFactor;

            // Apply only X rotation, keeping other axes unchanged
            rootHinge.transform.rotation = Quaternion.Euler(angleX, originalRotation.eulerAngles.y, originalRotation.eulerAngles.z);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    protected override IEnumerator DestroyNode(float delay = 1f)
    {
        // animate open

        if (rootLock != null) { Destroy(rootLock); }

        StartCoroutine(OpenChest(1f));
        StartCoroutine(ShakeChest(0.75f));

        //yield return new WaitForSeconds(0.5f);

        yield return base.DestroyNode(5f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tool"))
        {
            BasicTool tool = other.GetComponent<BasicTool>();

            if (tool.isActionPlaying)
            {
                bool doDamage = true;
                foreach (GameObject toolObject in triggeredObjects)
                {
                    if (other.gameObject == toolObject)
                    {
                        doDamage = false;
                        break;
                    }
                }
                if (doDamage)
                {
                    float damage = other.GetComponent<BasicTool>().damage;
                    Debug.Log($"Chest Received {damage} Damage!");
                    TakeDamage(damage);
                    triggeredObjects.Add(other.gameObject);
                    return;
                }
            }
        }
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (GameObject toolObject in triggeredObjects)
        {
            if (other.gameObject == toolObject)
            {
                triggeredObjects.Remove(other.gameObject);
                return;
            }
        }
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
