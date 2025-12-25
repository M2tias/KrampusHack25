using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSpawner : MonoBehaviour
{
    public static CharacterSpawner main;

    public List<GameObject> Enemies { get { return enemies; } }
    public GameObject Player { get { return player; } }

    private List<Transform> spawnPoints = new();
    private List<Transform> freeSpawnPoints = new();

    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private List<GameObject> enemyPrefabs;
    [SerializeField]
    private int numberOfEnemies;

    private List<GameObject> enemies = new();
    private GameObject player;
    private int killedEnemies = 0;

    void Awake()
    {
        if (main != null)
        {
            Debug.LogError("One character spawner already exists!");
            Destroy(gameObject);
            return;
        }

        main = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform point in transform)
        {
            spawnPoints.Add(point);
            point.GetComponent<MeshRenderer>().enabled = false;
        }

        freeSpawnPoints.AddRange(spawnPoints);

        player = Instantiate(playerPrefab);

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
            enemies.Add(enemy);
        }

        enemies.AddRange(FindGameObjectsWithLayer(LayerMask.NameToLayer("Enemy")));
    }

    List<GameObject> FindGameObjectsWithLayer(int layer)
    {
        var goArray = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        var goList = new List<GameObject>();

        for (var i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == layer)
            {
                goList.Add(goArray[i]);
            }
        }

        return goList;
    }

    // Update is called once per frame
    void Update()
    {
        UIManager.main.SetEnemyCount(enemies.Count - killedEnemies);
        if (enemies.Count == killedEnemies)
        {
            Invoke("Victory", 5f);
        }
    }

    public void EnemyKilled()
    {
        killedEnemies++;
    }

    public int GetEnemyCount()
    {
        return enemies.Count;
    }

    public bool IsVictory()
    {
        return enemies.Count == killedEnemies;
    }

    private void Victory()
    {
        SceneManager.LoadScene("victory", LoadSceneMode.Single);
    }
}
