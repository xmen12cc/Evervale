using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShakeType { Jittery, Smooth }

public class BushNode : Node
{
    [Header("Bush Settings")]
    public GameObject rootBush;
    public List<GameObject> bushes;
    public List<GameObject> berries;
    private bool playerInRange;

    [Header("Shake Settings")]
    public ShakeType shakeType = ShakeType.Smooth;
    // Jitter
    public float shakeForce = 0.05f;
    // Smooth Shake
    public float maxAngle = 20f;
    public float frequency = 6f;
    public float decay = 6f;

    [Header("Keybinds")]
    public KeyCode interactKey;

    private void Start()
    {

        base.Start();
    }

    private void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(interactKey))
            {
                TakeDamage(1f);
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        if (isDestroyed) return;

        if (berries != null && berries.Count > 0)
        {
            float healthStep = maxHealth / berries.Count; // Divide max health into steps

            for (int i = 0; i < berries.Count; i++)
            {
                float threshold = maxHealth - (healthStep * (i));
                berries[i].SetActive(health > threshold);
            }

            StartCoroutine(ShakeBush(1f));
            lootSpawner.GenerateLoot();
            Debug.Log("Spawn Loot");
            // if health is x% lower than maxhealth, set corrosponding index of berries to false.
            // after that shake both the RootBush and all the bushes fro mthe list for a second's time.
            // additionally, generate new items
        }

        base.TakeDamage(damage);
    }

    private IEnumerator ShakeBush(float duration = 1f)
    {
        float elapsed = 0f;

        // Store bush transforms
        Vector3 rootOriginalPos = rootBush.transform.position;
        Quaternion rootOriginalRot = rootBush.transform.rotation;
        Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();
        Dictionary<GameObject, Quaternion> originalRotations = new Dictionary<GameObject, Quaternion>();

        foreach (GameObject bush in bushes)
        {
            originalPositions[bush] = bush.transform.position;
            originalRotations[bush] = bush.transform.rotation;
        }

        // check effect type
        if (shakeType == ShakeType.Jittery)
        {
            while (elapsed < duration)
            {
                rootBush.transform.position = rootOriginalPos + new Vector3(
                    Random.Range(-shakeForce, shakeForce),
                    Random.Range(-shakeForce, shakeForce),
                    Random.Range(-shakeForce, shakeForce)
                );

                foreach (var bush in bushes)
                {
                    bush.transform.position = originalPositions[bush] + new Vector3(
                        Random.Range(-shakeForce, shakeForce),
                        Random.Range(-shakeForce, shakeForce),
                        Random.Range(-shakeForce, shakeForce)
                    );
                }

                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        else if (shakeType == ShakeType.Smooth)
        {
            Dictionary<GameObject, float> phaseOffsets = new Dictionary<GameObject, float>();

            foreach (var bush in bushes)
            {
                phaseOffsets[bush] = Random.Range(0f, Mathf.PI * 2); // Random offset in radians
            }

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float decayFactor = Mathf.Exp(-t * decay);

                Vector3 smoothAngle = new Vector3(
                    maxAngle * Mathf.Sin((t * frequency * Mathf.PI) + Random.Range(0f, Mathf.PI)) * decayFactor,
                    maxAngle * Mathf.Sin((t * frequency * Mathf.PI) + Random.Range(0f, Mathf.PI)) * decayFactor,
                    maxAngle * Mathf.Sin((t * frequency * Mathf.PI) + Random.Range(0f, Mathf.PI)) * decayFactor
                );

                rootBush.transform.rotation = rootOriginalRot * Quaternion.Euler(smoothAngle);
                foreach (var bush in bushes)
                {
                    float phaseOffset = phaseOffsets[bush];

                    Vector3 smoothBushAngle = new Vector3(
                        maxAngle * Mathf.Sin((t * frequency * Mathf.PI) + phaseOffset) * decayFactor,
                        maxAngle * Mathf.Sin((t * frequency * Mathf.PI) + phaseOffset) * decayFactor,
                        maxAngle * Mathf.Sin((t * frequency * Mathf.PI) + phaseOffset) * decayFactor
                    );

                    bush.transform.rotation = originalRotations[bush] * Quaternion.Euler(smoothBushAngle);
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        // Reset transform
        rootBush.transform.position = rootOriginalPos;
        rootBush.transform.rotation = rootOriginalRot;
        foreach (var bush in bushes)
        {
            bush.transform.position = originalPositions[bush];
            bush.transform.rotation = originalRotations[bush];
        }
    }

    protected override IEnumerator DestroyNode()
    {
        foreach (GameObject berry in berries)
        {
            berry.SetActive(false);
        }

        yield return base.DestroyNode();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
