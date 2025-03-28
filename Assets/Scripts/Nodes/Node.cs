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

    [Header("Loot Settings")]
    public LootSpawner lootSpawner;
    [SerializeField] public List<LootEntry> lootPool = new List<LootEntry>();

    public virtual void Start()
    {
        lootSpawner = GameObjectFinder.FindChildRecursive(gameObject, "LootSpawner").GetComponent<LootSpawner>();
        if (lootSpawner != null) { lootSpawner.possibleLoot = lootPool; }
    }

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
        yield return new WaitForSeconds(0.5f);

        // Find and activate LootNode
        if (lootSpawner != null) 
        { 
            lootSpawner.GenerateLoot();
            Debug.Log("Spawn Loot");
        }

        yield return new WaitForSeconds(4f);

        Destroy(gameObject);
    }
}
