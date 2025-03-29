using UnityEngine;
using System.Collections;
using UnityEditor.UIElements;
using System.Collections.Generic;

public class StoneNode : Node
{
    [Header("Stone Settings")]
    public List<GameObject> stoneChunks;
    private float healthStep;

    private void Start()
    {
        healthStep = maxHealth / stoneChunks.Count;

        base.Start();
    }

    public override void TakeDamage(float damage)
    {
        if (isDestroyed) return;

        if (stoneChunks == null || stoneChunks.Count == 0) { return; }

        for (int i = 0; i < stoneChunks.Count; i++)
        {
            float threshold = maxHealth - (healthStep * (i + 1));
            stoneChunks[i].SetActive(health > threshold);
        }

        base.TakeDamage(damage);
    }

    protected override IEnumerator DestroyNode()
    {
        for (int i = 0; i < stoneChunks.Count; i++)
        {
            stoneChunks[i].SetActive(false);
        }

        yield return base.DestroyNode();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tool"))
        {
            BasicTool tool = other.GetComponent<BasicTool>();

            if (tool.isActionPlaying && tool.toolTag == ToolTag.Mining)
            {
                //Debug.Log("Stone Received Hit!");
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
