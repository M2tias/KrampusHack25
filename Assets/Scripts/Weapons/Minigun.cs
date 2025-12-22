using UnityEngine;

public class Minigun : MonoBehaviour, IWeapon
{
    [SerializeField]
    private PoolType pooledPrefabType;
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private float fireRate; //rpm

    private int ammo = 0;
    private float lastShot = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (fireRate == 0)
        {
            fireRate = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Shoot()
    {
        if (Time.time - lastShot >= (60 / fireRate))
        {
            lastShot = Time.time;
            GameObject bullet = PrefabPool.Pools[pooledPrefabType].Get();

            if (bullet == null)
            {
                return;
            }

            bullet.GetComponent<Rigidbody>().linearVelocity = bulletSpeed * transform.forward;
            bullet.transform.position = transform.position + transform.forward * 1.3f;
            bullet.SetActive(true);
        }
    }
}
