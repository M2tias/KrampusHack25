using System.Collections.Generic;
using UnityEngine;

public class CharacterLoot : MonoBehaviour
{
    private Dictionary<LootType, PickupData> pickedUpLoot = new();

    private Hp hp;
    private bool isAI = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hp = GetComponent<Hp>();
        isAI = gameObject.tag == "Enemy";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int GetPickupLevel(LootType type)
    {
        if (!pickedUpLoot.ContainsKey(type))
        {
            return -1;
        }

        return pickedUpLoot[type].LootLevel;
    }

    private void OnTriggerEnter(Collider maybeLoot)
    {
        if (maybeLoot.gameObject.TryGetComponent(out Pickup pickup))
        {
            if (pickup.LootType == LootType.Health)
            {
                if (!hp.IsMaxHP())
                {
                    hp.IncreaseHP(pickup.LootLevel);
                    Destroy(maybeLoot.gameObject);
                }
            }
            else if (pickedUpLoot.TryGetValue(pickup.LootType, out PickupData oldPickup))
            {
                if (pickup.LootLevel > oldPickup.LootLevel)
                {
                    pickedUpLoot[pickup.LootType] = pickup.Data;
                    Destroy(maybeLoot.gameObject);

                    if (!isAI)
                    {
                        UIManager.main.Pickup(pickup.LootType, pickup.LootLevel);
                    }
                }
            }
            else
            {
                pickedUpLoot.Add(pickup.LootType, pickup.Data);

                if (!isAI)
                {
                    UIManager.main.Pickup(pickup.LootType, pickup.LootLevel);
                }
                Destroy(maybeLoot.gameObject);
            }
        }
    }
}
