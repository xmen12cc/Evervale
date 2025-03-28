using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootEntry
{
    public Item item;
    public int amount = 1;
    public bool independant;
    [UnityEngine.Range(1, 100)] public int chance = 1;
}

public class LootSpawner : MonoBehaviour
{
    public Vector2 flingPowerMinMax = new Vector2(2f, 5f);
    public GameObject lootPrefab;
    public List<LootEntry> possibleLoot = new List<LootEntry>();

    public void GenerateLoot()
    {
        foreach (LootEntry lootEntry in possibleLoot)
        {
            int chance = (int)Random.Range(1f, lootEntry.chance);

            if (chance == 1)
            {
                if (!lootEntry.independant)
                {
                    SpawnLoot(lootEntry.item, lootEntry.amount);
                }
                else
                {
                    for (int i = 0; i < lootEntry.amount; i++)
                    {
                        SpawnLoot(lootEntry.item, 1);
                    }
                }
            }
        }
    }

    public void SpawnLoot(Item item, int amount)
    {
        GameObject lootObject = Instantiate(lootPrefab, transform.position, Quaternion.identity);
        lootObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        Loot loot = lootObject.GetComponent<Loot>();

        if (loot != null)
        {
            loot.item = item;
            loot.amount = amount;
            loot.gameObject.SetActive(true);
            FlingLoot(loot);
        }
    }

    public void FlingLoot(Loot loot)
    {
        GameObject lootObject = loot.gameObject;

        // fling physics here

        Rigidbody rb = lootObject.GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = lootObject.AddComponent<Rigidbody>(); // Ensure Rigidbody exists
        }

        rb.mass = 1f; // Set mass for realistic physics
        rb.linearDamping = 0f; // Optional: adds slight resistance

        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized;

        float forceMagnitude = Random.Range(flingPowerMinMax.x, flingPowerMinMax.y); // Adjust for desired fling power
        rb.AddForce(randomDirection * forceMagnitude, ForceMode.Impulse);
    }
}
