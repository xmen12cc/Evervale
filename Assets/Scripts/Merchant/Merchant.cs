using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Merchant : MonoBehaviour
{
    [SerializeField] private Inventory merchantInventory;
    [SerializeField] private int merchantGold = 1000;
    [SerializeField] private Transform merchantUI;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Text goldText;
    [SerializeField] private GameObject itemButtonPrefab;
    [SerializeField] private Transform itemsContainer;

    private void Start()
    {
        UpdateGoldUI();
        PopulateMerchantInventory();
    }

    public int GetGold()
    {
        return merchantGold;
    }

    public Inventory GetInventory()
    {
        return merchantInventory;
    }

    public void OpenShop()
    {
        merchantUI.gameObject.SetActive(true);
        DisplayMerchantInventory();
        DisplayPlayerInventory();
    }


    public void CloseShop()
    {
        merchantUI.gameObject.SetActive(false);
    }

    void DisplayMerchantInventory()
    {
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (InventorySlot slot in merchantInventory.GetItems())
        {
            if (slot.myItem != null)
            {
                Item item = slot.myItem.myItem;
                GameObject itemButton = Instantiate(itemButtonPrefab, itemsContainer);
                Button button = itemButton.GetComponent<Button>();
                Text buttonText = itemButton.GetComponentInChildren<Text>();
                buttonText.text = item.name + " - Price: " + item.value;

                button.onClick.AddListener(() => BuyItem(item));
            }
        }
    }

    [SerializeField] private Transform playerItemsContainer;
    [SerializeField] private GameObject playerItemButtonPrefab;
void DisplayPlayerInventory()
{
    foreach (Transform child in playerItemsContainer)
    {
        Destroy(child.gameObject);
    }

    foreach (InventorySlot slot in playerInventory.GetItems())
    {
        Item item = slot.myItem.myItem;
        GameObject itemButton = Instantiate(playerItemButtonPrefab, playerItemsContainer);
        Button button = itemButton.GetComponent<Button>();
        Text buttonText = itemButton.GetComponentInChildren<Text>();
        buttonText.text = item.name;

        button.onClick.AddListener(() => SellItem(item));
    }
}


    public void BuyItem(Item item)
    {
        if (playerInventory.PlayerGold >= item.value)
        {
            if (!playerInventory.IsStorageFull())
            {
                playerInventory.PlayerGold -= item.value;
                merchantGold += item.value;
                playerInventory.SpawnInventoryItem(item);
                merchantInventory.RemoveItem(item);
                UpdateGoldUI();
                Debug.Log("Bought " + item.name);
            }
            else
            {
                Debug.Log("Inventory Full!");
            }
        }
        else
        {
            Debug.Log("Not Enough Gold!");
        }
    }


    public void SellItem(Item item)
    {
        if (merchantGold >= item.value)
        {
            merchantGold -= item.value;
            playerInventory.PlayerGold += item.value;
            merchantInventory.AddItem(item);
            playerInventory.RemoveItem(item);
            UpdateGoldUI();
            DisplayPlayerInventory();
            Debug.Log("Sold " + item.name);
        }
        else
        {
            Debug.Log("Merchant Doesn't Have Enough Gold!");
        }
    }

    void UpdateGoldUI()
    {
        goldText.text = "Merchant Gold: " + merchantGold;
    }

    void PopulateMerchantInventory()
    {
        merchantInventory.AddItem(new Item("Potion", 100));
        merchantInventory.AddItem(new Item("Sword", 300));
        merchantInventory.AddItem(new Item("Shield", 200));
    }
}
