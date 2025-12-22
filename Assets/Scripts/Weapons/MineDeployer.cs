using UnityEngine;

public class MineDeployer : MonoBehaviour, IWeapon
{
    [SerializeField]
    private float mineCD;
    [SerializeField]
    private PoolType pooledPrefabType;
    [SerializeField]
    private float mineDamage;

    private float lastMine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastMine = Time.time;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Shoot()
    {
        if (Time.time - lastMine > mineCD)
        {
            GameObject mineInstance = PrefabPool.Pools[pooledPrefabType].Get();
            mineInstance.transform.position = transform.position - transform.forward * 2f - transform.up;

            Mine mine = mineInstance.GetComponent<Mine>();
            mine.Init(mineDamage);
            mineInstance.SetActive(true);
            lastMine = Time.time;
        }
    }
}
