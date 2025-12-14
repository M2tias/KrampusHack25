using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    private List<Transform> spawnPoints = new();
    private List<Transform> freeSpawnPoints = new();

    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private List<GameObject> enemyPrefabs;
    [SerializeField]
    private int numberOfEnemies;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform point in transform)
        {
            spawnPoints.Add(point);
        }

        freeSpawnPoints.AddRange(spawnPoints);

        GameObject player = Instantiate(playerPrefab);

        int spawnPos = Random.Range(0, freeSpawnPoints.Count);
        player.transform.position = freeSpawnPoints[spawnPos].position;
        freeSpawnPoints.RemoveAt(spawnPos);

        for (int i = 0; i < numberOfEnemies; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            GameObject enemy = Instantiate(enemyPrefab);
            int enemySpawnPos = Random.Range(0, freeSpawnPoints.Count);
            enemy.transform.position = freeSpawnPoints[enemySpawnPos].position;
            freeSpawnPoints.RemoveAt(enemySpawnPos);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
