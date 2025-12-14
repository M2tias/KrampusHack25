using UnityEngine;

public class PooledLifetime : MonoBehaviour
{
    [SerializeField]
    private float lifetime;

    [SerializeField]
    private PoolType poolType;
    private float activated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void OnEnable()
    {
        activated = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (activated < 0)
        {
            return;
        }

        if (Time.time - activated >= lifetime)
        {
            PrefabPool.Pools[poolType].Kill(gameObject);
        }
    }

    void OnDisable()
    {
        activated = -1;
    }
}
