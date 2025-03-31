using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ProceduralSpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] spawnableObjects; // List of objects (trees, rocks, etc.)
    [SerializeField] private int spawnAmount = 50; // Number of objects to spawn
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(10, 10); // Plane size
    [SerializeField] private float minSpawnDistance = 1.5f; // Minimum distance between objects
    [SerializeField] private Transform plane; // Reference to the plane

    private List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        GenerateObjects();
    }

    void GenerateObjects()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 spawnPosition = GetValidSpawnPosition();

            if (spawnPosition != Vector3.zero)
            {
                GameObject objToSpawn = spawnableObjects[Random.Range(0, spawnableObjects.Length)];
                Quaternion randomYRotation = Quaternion.Euler(0, Random.Range(0, 360f), 0); // keep upright
                Instantiate(objToSpawn, spawnPosition, randomYRotation);
                spawnedPositions.Add(spawnPosition);
            }
        }
    }

    Vector3 GetValidSpawnPosition()
    {
        for (int attempts = 0; attempts < 10; attempts++) // Try 10 times to find a valid position
        {
            float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
            float randomZ = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
            Vector3 randomPosition = new Vector3(plane.position.x + randomX, plane.position.y, plane.position.z + randomZ);

            // Ensure object doesn't overlap with others
            if (IsFarFromOthers(randomPosition))
                return randomPosition;
        }

        return Vector3.zero; // If no valid spot is found
    }

    bool IsFarFromOthers(Vector3 position)
    {
        foreach (Vector3 spawnedPos in spawnedPositions)
        {
            if (Vector3.Distance(position, spawnedPos) < minSpawnDistance)
                return false;
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.green; // Set color

        Vector3 position = transform.position;
        float halfWidth = spawnAreaSize.x / 2;
        float halfHeight = spawnAreaSize.y / 2;

        // Define corners
        Vector3 topLeft = position + new Vector3(-halfWidth, 0, halfHeight);
        Vector3 topRight = position + new Vector3(halfWidth, 0, halfHeight);
        Vector3 bottomRight = position + new Vector3(halfWidth, 0, -halfHeight);
        Vector3 bottomLeft = position + new Vector3(-halfWidth, 0, -halfHeight);

        // Draw square
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
