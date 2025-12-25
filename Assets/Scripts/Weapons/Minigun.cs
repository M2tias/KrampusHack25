using UnityEngine;

public class Minigun : MonoBehaviour, IWeapon
{
    [SerializeField]
    private PoolType pooledPrefabType;
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private float fireRate; //rpm
    [SerializeField]
    private AudioPlayer audioPlayer;

    private int ammo = 0;
    private float lastShot = 0;
    private float baseDamage = 5;
    private float levelBoost = 0.22f;
    private CharacterLoot loot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (fireRate == 0)
        {
            fireRate = 1;
        }

        loot = GetComponent<CharacterLoot>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Shoot()
    {
        if (Time.time - lastShot >= (60 / GetFirerate()))
        {
            lastShot = Time.time;
            GameObject bullet = PrefabPool.Pools[pooledPrefabType].Get();

            if (bullet == null)
            {
                return;
            }

            bullet.GetComponent<Rigidbody>().linearVelocity = bulletSpeed * transform.forward;
            bullet.transform.position = transform.position + transform.forward * 2.5f;
            bullet.GetComponent<Bullet>().Init(GetDamage());
            bullet.SetActive(true);
            audioPlayer.PlayClip();
        }
    }

    private float GetDamage() {
        int lootLevel = loot.GetPickupLevel(LootType.Minigun);

        if(lootLevel < 1) {
            return baseDamage;
        }

        return baseDamage + baseDamage * levelBoost * lootLevel;
    }

    private float GetFirerate() {
        int lootLevel = loot.GetPickupLevel(LootType.Minigun);

        if(lootLevel < 1) {
            return fireRate;
        }

        return fireRate + fireRate * levelBoost * lootLevel;
    }
}
