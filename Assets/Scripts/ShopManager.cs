using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{   
    [Header("Shop Items")]
    public List<ShopItem> itemsForSale = new List<ShopItem>();

    [Header("UI References")]
    public Transform playerInventoryParent;
    public GameObject itemUIPrefab; 
    public Transform shopUIListParent;
    public Text goldText; 

    private void Start()
    {
        UpdateGoldDisplay();
        
    }

    public void BuyItem(int index)
    {
        if (index < 0 || index >= itemsForSale.Count) return;

        ShopItem item = itemsForSale[index];

        if (Inventory.Singleton.PlayerGold >= item.price)
        {
            Inventory.Singleton.PlayerGold -= item.price;
            Instantiate(item.prefab, playerInventoryParent.position, Quaternion.identity);
            Debug.Log($"Bought {item.itemName} for {item.price} gold.");
            UpdateGoldDisplay();
        }
        else
        {
            Debug.Log("Not enough gold to buy " + item.itemName);
        }
    }

    void UpdateGoldDisplay()
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {Inventory.Singleton.PlayerGold}";
        }
    }

 
    public void BuyItemByName(string itemName)
    {
        int index = itemsForSale.FindIndex(item => item.itemName == itemName);
        BuyItem(index);
    }
}
