using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    [SerializeField]
    private GameObject spikeModel;

    private InputAction shootAction;
    private InputAction dropMineAction;
    private InputAction weapon1Action;

    private InputAction weapon2Action;

    private InputAction weapon3Action;


    private Minigun minigun;
    private RocketLauncher rocketLauncher;
    private MineDeployer mineDeployer;
    private RammingSpikes spikes;

    private IWeapon currentWeapon = null;
    private CharacterLoot loot;
    private Rigidbody rb;

    private bool hasSpikes = false;
    private bool isAI = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isAI = gameObject.tag == "Enemy";
        shootAction = InputSystem.actions.FindAction("Shoot");
        dropMineAction = InputSystem.actions.FindAction("DropMine");
        weapon1Action = InputSystem.actions.FindAction("Weapon1");
        weapon2Action = InputSystem.actions.FindAction("Weapon2");
        weapon3Action = InputSystem.actions.FindAction("Weapon3");
        minigun = GetComponent<Minigun>();
        rocketLauncher = GetComponent<RocketLauncher>();
        mineDeployer = GetComponent<MineDeployer>();
        loot = GetComponent<CharacterLoot>();
        spikes = spikeModel.GetComponent<RammingSpikes>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasSpikes)
        {
            hasSpikes = loot.GetPickupLevel(LootType.RammingSpike) > 0;
            spikeModel.SetActive(hasSpikes);
        }
        else
        {
            spikes.SetSpeed(rb.linearVelocity.magnitude);
        }

        if (isAI)
        {
            return;
        }

        // TODO: hold to fire & cooldown
        if (shootAction.IsPressed() && minigun != null && loot.GetPickupLevel(LootType.Minigun) > 0)
        {
            minigun.Shoot();
        }
        else if (dropMineAction.IsPressed() && mineDeployer != null && loot.GetPickupLevel(LootType.Mines) > 0)
        {
            mineDeployer.Shoot();
        }

        // if (weapon1Action.WasPerformedThisFrame() && loot.GetPickupLevel(LootType.Minigun) > 0)
        // {
        //     Debug.Log("Use minigun");
        //     currentWeapon = minigun;
        // }
        // else if (weapon2Action.WasPerformedThisFrame() && loot.GetPickupLevel(LootType.Rockets) > 0)
        // {
        //     Debug.Log("Use rocket launcher");
        //     currentWeapon = rocketLauncher;
        // }
        // else if (weapon3Action.WasPerformedThisFrame() && loot.GetPickupLevel(LootType.Mines) > 0)
        // {
        //     Debug.Log("Use mines");
        //     currentWeapon = mineDeployer;
        // }
    }

    public void JustShoot()
    {
        if (loot.GetPickupLevel(LootType.Rockets) > 0)
        {
            Debug.Log("Use rocket launcher");
            currentWeapon = rocketLauncher;
        }
        else if (loot.GetPickupLevel(LootType.Minigun) > 0)
        {
            Debug.Log("Use minigun");
            currentWeapon = minigun;
        }

        currentWeapon?.Shoot();
    }
}