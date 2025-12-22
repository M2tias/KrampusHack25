using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryIcon : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI itemLevel;

    [SerializeField]
    private Image itemImage;
    private LootType itemType;

    public void Init(LootType itemType, Sprite itemSprite)
    {
        itemImage.sprite = itemSprite;
        this.itemType = itemType;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        int lvl = UIManager.main.GetItemLevel(itemType);
        
        if (lvl > 0) {
            itemLevel.text = lvl.ToString();
        }
    }
}
