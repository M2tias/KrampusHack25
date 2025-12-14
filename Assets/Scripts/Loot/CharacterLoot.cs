using System.Collections.Generic;
using UnityEngine;

public class CharacterLoot : MonoBehaviour
{
    private Dictionary<LootType, PickupData> pickedUpLoot = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
            if (pickedUpLoot.TryGetValue(pickup.LootType, out PickupData oldPickup))
            {
                if (pickup.LootLevel > oldPickup.LootLevel)
                {
                    pickedUpLoot[pickup.LootType] = pickup.Data;
                    Destroy(maybeLoot.gameObject);
                }
            }
            else
            {
                pickedUpLoot.Add(pickup.LootType, pickup.Data);
                Destroy(maybeLoot.gameObject);
            }
        }
    }
}
