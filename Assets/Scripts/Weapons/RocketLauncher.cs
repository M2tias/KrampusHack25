using UnityEngine;

public class RocketLauncher : MonoBehaviour, IWeapon
{
    [SerializeField]
    private float timeBetweenRockets;
    [SerializeField]
    private float timeToReload;
    [SerializeField]
    private PoolType pooledPrefabType;

    private int maxLoad = 3;
    private int currentLoad = 3;
    private float lastRocketShot;
    private float reloadingStarted;

    private float searchRadius = 60f;
    private float searchRadiusSqr;
    private float searchAngle = 35f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastRocketShot = Time.time;
        searchRadiusSqr = searchRadius * searchRadius;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.DrawLine(transform.position, transform.position + transform.forward * searchRadius, Color.red);
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0, searchAngle, 0) * transform.forward * searchRadius, Color.green);
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(0, -searchAngle, 0) * transform.forward * searchRadius, Color.green);
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(-searchAngle, 0, 0) * transform.forward * searchRadius, Color.green);
        Debug.DrawLine(transform.position, transform.position + Quaternion.Euler(searchAngle, 0, 0) * transform.forward * searchRadius, Color.green);
    }

    public void Shoot()
    {
        if (Time.time - lastRocketShot >= timeBetweenRockets && currentLoad > 0)
        {
            Debug.Log("Shooting a rocket");
            currentLoad--;

            if (currentLoad == 0)
            {
                reloadingStarted = Time.time;
            }

            lastRocketShot = Time.time;
            GameObject rocketInstance = PrefabPool.Pools[pooledPrefabType].Get();

            if (rocketInstance == null)
            {
                return;
            }

            Transform target = null; // = new GameObject().transform;

            // RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, searchRadius, transform.forward, 0.1f, LayerMask.GetMask("Enemy"), QueryTriggerInteraction.Ignore);
            bool targetFound = false;
            //Debug.Log($"Found {rayHits.Length} enemies");

            foreach (GameObject enemy in CharacterSpawner.main.Enemies)
            {
                if ((enemy.transform.position - transform.position).sqrMagnitude > searchRadiusSqr)
                {
                    continue;
                }

                float angle = Vector3.Angle(enemy.transform.position - transform.position, transform.forward);

                if (angle <= searchAngle)
                {
                    target = enemy.transform;
                    targetFound = true;
                }
            }

            if (!targetFound)
            {
                target = new GameObject().transform;
                target.position = transform.position + transform.forward * 20f;
            }

            Debug.Log($"Initializing a rocket with target {target.gameObject.name}");
            Rocket rocket = rocketInstance.GetComponent<Rocket>();
            Vector3 dir = (transform.forward * 0.3f + transform.up * 0.7f).normalized;
            rocket.Init(dir, target, "Enemy", 10f, 20f, 180f);
            rocket.transform.position = transform.position + transform.up;
            rocketInstance.SetActive(true);
        }
        else if (Time.time - reloadingStarted > timeToReload)
        {
            currentLoad = maxLoad;
        }
    }
}
