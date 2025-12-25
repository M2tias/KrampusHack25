using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hp : MonoBehaviour
{
    [SerializeField]
    private bool isPlayer;
    [SerializeField]
    private float maxHP;
    [SerializeField]
    private List<float> hpGainForLevel; // TODO: move to global
    [SerializeField]
    private float zoneDamageCD;
    [SerializeField]
    private float zoneDamage;
    [SerializeField]
    private AudioPlayer audioPlayer;
    [SerializeField]
    private AudioPlayer mineAudioClip;
    [SerializeField]
    private Destruction destruction;

    private float currentHP;
    private float lastZoneDamage;
    private bool isAI = false;
    private int shieldLevel = -1;
    private float shieldDamageMult = 0.95f;
    private CharacterLoot loot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isAI = gameObject.tag == "Enemy";
        currentHP = maxHP;
        loot = GetComponent<CharacterLoot>();
    }

    // Update is called once per frame
    void Update()
    {
        shieldLevel = loot.GetPickupLevel(LootType.Shield);
        if (Time.time - lastZoneDamage > zoneDamageCD)
        {
            //bool insideZone = Physics.SphereCast(transform.position, 0.1f, transform.up, out RaycastHit _, 0.1f, LayerMask.GetMask("Zone"), QueryTriggerInteraction.Ignore);
            bool insideZone = ZoneWall.main.CheckInside(transform);

            if (!insideZone)
            {
                currentHP -= zoneDamage;
            }

            lastZoneDamage = Time.time;
        }

        if (isPlayer)
        {
            UIManager.main.SetPlayerHP(currentHP);
        }

        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }

        if (currentHP <= 0 || transform.position.y < -2)
        {
            if (isAI)
            {
                destruction.TriggerDestruction(false);
                CharacterSpawner.main.EnemyKilled();
                Destroy(gameObject);
            }
            else
            {
                destruction.TriggerDestruction(true);
                Destroy(gameObject);
            }
        }
    }

    public void IncreaseHP(int hpPickupLevel)
    {
        IncreaseHP(hpGainForLevel[hpPickupLevel - 1]);
    }

    public void IncreaseHP(float hpAmount)
    {
        currentHP = Mathf.Min(currentHP + hpAmount, maxHP);
    }

    public void DoDamage(float damage)
    {
        float coef = shieldLevel == -1 ? 1 : shieldDamageMult * shieldLevel;
        currentHP -= damage * coef;
        audioPlayer.PlayClip();
    }

    public bool IsMaxHP()
    {
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }

        return maxHP - currentHP < 0.001f;
    }

    public void PlayMineExplosion() {
        mineAudioClip.PlayClip();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out Bullet bullet))
        {
            DoDamage(bullet.Damage);
            Destroy(bullet.gameObject);
        }
    }
}
