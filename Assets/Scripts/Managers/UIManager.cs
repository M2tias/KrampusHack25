using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager main;

    [SerializeField]
    private GameObject inventoryIconPrefab;
    [SerializeField]
    private Transform inventoryIconParent;

    [SerializeField]
    private List<Pair<LootType, Sprite>> UISprites;

    [SerializeField]
    private TextMeshProUGUI hpValue;

    [SerializeField]
    private Speedometer speedometer;

    private Dictionary<LootType, int> itemLevels = new();
    private Dictionary<LootType, InventoryIcon> itemUIElements = new();

    void Awake()
    {
        if (main != null)
        {
            Destroy(gameObject);
            return;
        }

        main = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (LootType itemType in Enum.GetValues(typeof(LootType)))
        {
            itemLevels.Add(itemType, 0);
            GameObject inventoryIconObj = Instantiate(inventoryIconPrefab);
            inventoryIconObj.transform.SetParent(inventoryIconParent, false);
            InventoryIcon itemUIElement = inventoryIconObj.GetComponent<InventoryIcon>();
            itemUIElements.Add(itemType, itemUIElement);
            var uISPritePair = UISprites.First(x => x.Key == itemType);
            if (uISPritePair == null || uISPritePair.Value == null) Debug.LogError($"UI Sprite {uISPritePair?.Key} is null");
            itemUIElement.Init(itemType, UISprites.First(x => x.Key == itemType).Value);
            inventoryIconObj.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public int GetItemLevel(LootType itemType)
    {
        return itemLevels[itemType];
    }

    public void Pickup(LootType itemType, int lvl)
    {
        itemLevels[itemType] = lvl;
        InventoryIcon uiElement = itemUIElements[itemType];
        uiElement.gameObject.SetActive(true);
    }

    public void SetSpeed(float speed)
    {
        speedometer.SetSpeed(speed);
    }

    public void SetPlayerHP(float HP)
    {
        hpValue.text = Mathf.RoundToInt(HP).ToString();
    }
}
