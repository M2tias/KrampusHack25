using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PrefabPool : MonoBehaviour
{
    public static Dictionary<PoolType, PrefabPool> Pools { get { return pools; } }
    private static Dictionary<PoolType, PrefabPool> pools = new();

    [SerializeField]
    private PoolType poolType;
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private int poolSize;

    private List<GameObject> poolObjects = new(); // all objects
    private Dictionary<int, GameObject> activeObjects = new();
    private List<GameObject> inactiveObjects = new();

    void Awake()
    {
        if (pools.ContainsKey(poolType))
        {
            Destroy(gameObject);
            Debug.LogError($"Prefab pool for type {poolType} already exists!");
            return;
        }

        pools.Add(poolType, this);
    }

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, new Vector3(0, -9999, 0), Quaternion.identity, transform);
            obj.SetActive(false);
            poolObjects.Add(obj);
            inactiveObjects.Add(obj);
        }
    }

    public GameObject Get(bool setActive = false)
    {
        int lastIndex = inactiveObjects.Count - 1;

        if (lastIndex < 0)
        {
            Debug.LogError($"Prefab pool for type {poolType} is EMPTY!");
            return null;
        }

        GameObject poolObj = inactiveObjects[lastIndex];
        inactiveObjects.RemoveAt(lastIndex);
        activeObjects.Add(poolObj.GetInstanceID(), poolObj);

        if (setActive)
        {
            poolObj.SetActive(true);
        }

        return poolObj;
    }

    public void Kill(GameObject poolObj)
    {
        InitObj(poolObj);
        activeObjects.Remove(poolObj.GetInstanceID());
        inactiveObjects.Add(poolObj);
    }

    private void InitObj(GameObject poolObj)
    {
        poolObj.SetActive(false);
        poolObj.transform.position = new Vector3(0, -9999, 0);
        poolObj.transform.rotation = Quaternion.identity;
        poolObj.transform.parent = transform;
    }
}

public enum PoolType
{
    Bullet
}