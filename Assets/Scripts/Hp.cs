using System.Collections.Generic;
using UnityEngine;

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

    private float currentHP;
    private float lastZoneDamage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = 50f;
    }

    // Update is called once per frame
    void Update()
    {
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
        currentHP -= damage;
        // TODO: Handle death
    }

    public bool IsMaxHP()
    {
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }

        return maxHP - currentHP < 0.001f;
    }
}
