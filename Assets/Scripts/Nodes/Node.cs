using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class Node : MonoBehaviour
{
    [Header("Node Settings")]
    public float health = 100f;
    protected bool isDestroyed = false;

    [HideInInspector] public List<GameObject> triggeredObjects = new List<GameObject>();

    public virtual void TakeDamage(float damage)
    {
        if (isDestroyed) return;

        health -= damage;
        if (health <= 0)
        {
            isDestroyed = true;
            StartCoroutine(DestroyNode());
        }
    }

    protected virtual IEnumerator DestroyNode()
    {
        // Placeholder for animation delay
        yield return new WaitForSeconds(1f);

        // Find and activate LootNode
        //LootNode loot = GetComponentInChildren<LootNode>();
        //if (loot) loot.ReleaseLoot();

        Destroy(gameObject);
    }
}
