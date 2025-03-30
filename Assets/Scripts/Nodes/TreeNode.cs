using UnityEngine;
using System.Collections;
using UnityEditor.UIElements;

public class TreeNode : Node
{
    [Header("Tree Settings")]
    public float fallSpeed = 45f;
    public float fallAngle = 90f;
    public GameObject treeRotation;
    private Vector3 lastHitPosition;  // Stores where the player hit

    private bool isFalling = false;

    GameObject player;

    private void Start()
    {
        player = TD_PlayerController.Singleton.gameObject;

        base.Start();
    }

    /*private void Update()
    {
        // Calculate the fall direction (away from the last hit position)
        Vector3 pos0 = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        Vector3 pos1 = new Vector3(treeRotation.transform.position.x, 0, treeRotation.transform.position.z);
        Vector3 fallDirection = (pos0 - pos1).normalized;

        Vector3 forward = fallDirection;  // The tree's forward direction
        Vector3 right = Vector3.Cross(Vector3.up, forward);  // Right vector, perpendicular to forward
        Vector3 up = Vector3.Cross(forward, right);  // Up vector, perpendicular to both forward and right

        // Step 3: Create a rotation that rotates the tree by 90 degrees around the right vector
        Quaternion fallRotation = Quaternion.AngleAxis(-90f, right);  // Rotate by 90 degrees around the right vector

        // Step 4: Apply the calculated rotation to the tree
        treeRotation.transform.rotation = fallRotation * Quaternion.Euler(0f, 0f, 0f);
    }*/

    public override void TakeDamage(float damage)
    {
        if (isDestroyed) return;

        // Store the last hit position based on the attacker's position
        if (triggeredObjects.Count > 0)
        {
            lastHitPosition = player.transform.position;//triggeredObjects[0].GetComponentInParent<TD_PlayerController>().gameObject.transform.position;
        }
        else
        {
            lastHitPosition = player.transform.position;
        }

            base.TakeDamage(damage);
    }

    protected override IEnumerator DestroyNode(float delay = 1f)
    {
        isFalling = true;

        // Calculate the fall direction (away from the last hit position)
        Vector3 pos0 = new Vector3(lastHitPosition.x, 0, lastHitPosition.z);
        Vector3 pos1 = new Vector3(treeRotation.transform.position.x, 0, treeRotation.transform.position.z);
        Vector3 fallDirection = (pos0 - pos1).normalized;

        // Calculate the forward, right, and up vectors from the fall direction
        Vector3 forward = fallDirection;  // The tree's forward direction
        Vector3 right = Vector3.Cross(Vector3.up, forward);  // Right vector, perpendicular to forward
        Vector3 up = Vector3.Cross(forward, right);  // Up vector, perpendicular to both forward and right

        // Apply the fall angle along the X-axis for the tilt
        float rotated = 0f;
        while (rotated < fallAngle)
        {
            // Rotate the tree along the right axis as it falls
            float step = fallSpeed * Time.deltaTime;
            //treeRotation.transform.RotateAround(treeRotation.transform.position, treeRotation.transform.right, step);
            Quaternion fallRotation = Quaternion.AngleAxis(-rotated, right);
            treeRotation.transform.rotation = fallRotation * transform.rotation;
            rotated += step;
            rotated *= 1.01f;
            yield return null;
        }

        yield return base.DestroyNode();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tool"))
        {
            BasicTool tool = other.GetComponent<BasicTool>();

            if (tool.isActionPlaying && (tool.toolTag == ToolTag.Woodcutting || tool.toolTag == ToolTag.Hammering))
            {
                //Debug.Log("Tree Received Hit!");
                // Add logic to apply damage
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
                    Debug.Log($"Tree Received {damage}!");
                    TakeDamage(damage);
                    triggeredObjects.Add(other.gameObject);
                }
            }
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
    }
}
