using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private ShopItem shopItem;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Image icon;
    [SerializeField] private Button buyButton;

    private void Start()
    {
        nameText.text = shopItem.itemName;
        priceText.text = shopItem.price.ToString();
        icon.sprite = shopItem.icon;
        buyButton.onClick.AddListener(BuyItem);
    }

    void BuyItem()
    {
        if (Inventory.Singleton.PlayerGold >= shopItem.price)
        {
            Inventory.Singleton.PlayerGold -= shopItem.price;
            Inventory.OnGoldChanged?.Invoke(Inventory.Singleton.PlayerGold);

            GameObject itemGO = Instantiate(shopItem.prefab);
            InventoryItem itemComp = itemGO.GetComponent<InventoryItem>();

            if (itemComp != null)
            {
                Inventory.Singleton.SpawnInventoryItem(itemComp.myItem);
                Destroy(itemGO);
            }
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }
}
