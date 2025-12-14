using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> lootPrefabs;
    private List<Transform> lootSpawnPoints = new();
    private List<Transform> freeSpawnPoints = new();
    private List<GameObject> spawnedLoot = new();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform point in transform)
        {
            lootSpawnPoints.Add(point);
        }

        freeSpawnPoints.AddRange(lootSpawnPoints);

        for (int i = 0; i < lootSpawnPoints.Count; i++)
        {
            GameObject lootPrefab = lootPrefabs[Random.Range(0, lootPrefabs.Count)];
            GameObject loot = Instantiate(lootPrefab);
            int lootSpawnPos = Random.Range(0, freeSpawnPoints.Count);
            loot.transform.position = freeSpawnPoints[lootSpawnPos].position;
            // Debug.Log($"Spawning to pos {freeSpawnPoints[lootSpawnPos].position}");
            freeSpawnPoints.RemoveAt(lootSpawnPos);
            spawnedLoot.Add(loot);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum LootType {
    Health,
    Shield,
    RammingSpike,
    Minigun
}
