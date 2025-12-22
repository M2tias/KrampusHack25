using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    InputAction shootAction;
    InputAction weapon1Action;

    InputAction weapon2Action;

    InputAction weapon3Action;


    Minigun minigun;
    RocketLauncher rocketLauncher;
    IWeapon currentWeapon = null;
    CharacterLoot loot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shootAction = InputSystem.actions.FindAction("Shoot");
        weapon1Action = InputSystem.actions.FindAction("Weapon1");
        weapon2Action = InputSystem.actions.FindAction("Weapon2");
        weapon3Action = InputSystem.actions.FindAction("Weapon3");
        minigun = GetComponent<Minigun>();
        rocketLauncher = GetComponent<RocketLauncher>();
        loot = GetComponent<CharacterLoot>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: hold to fire & cooldown
        if (shootAction.IsPressed() && currentWeapon != null)
        {
            currentWeapon.Shoot();
        }

        if (weapon1Action.WasPerformedThisFrame() && loot.GetPickupLevel(LootType.Minigun) > 0)
        {
            Debug.Log("Use minigun");
            currentWeapon = minigun;
        }
        else if (weapon2Action.WasPerformedThisFrame() && loot.GetPickupLevel(LootType.Rockets) > 0)
        {
            Debug.Log("Use rocket launcher");
            currentWeapon = rocketLauncher;
        }
        else if (weapon3Action.WasPerformedThisFrame() && loot.GetPickupLevel(LootType.Mines) > 0)
        {
            Debug.Log("Use mines");
            currentWeapon = null; // TODO: mines
        }
    }
}