using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public enum Backpack
{
    DEFAULT,
    Travelers,
    Adventurers,
    Everwoven
}

public class Inventory : MonoBehaviour
{
    [Header("Slots")]
    public int numOfInventory = 27;
    public int numOfHotbars = 9;
    [SerializeField] GameObject InventoryContainer;
    [SerializeField] GameObject HotbarContainer;
    [SerializeField] GameObject HotbarDisplay;
    [SerializeField] InventorySlot slotPrefab;

    public static Inventory Singleton;
    public static InventoryItem carriedItem;

    [SerializeField] public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    [SerializeField] public List<InventorySlot> hotbarSlots = new List<InventorySlot>();

    [Header("Upgrades")]
    public Backpack backpack;
    Dictionary<Backpack, Dictionary<string, object>> backpackDetails = new Dictionary<Backpack, Dictionary<string, object>> {
        { Backpack.DEFAULT, new Dictionary<string, object> { { "numOfInventory", 9 }, { "numOfHotbars", 6 } } },
        { Backpack.Travelers, new Dictionary<string, object> { { "numOfInventory", 18 }, { "numOfHotbars", 6 } } },
        { Backpack.Adventurers, new Dictionary<string, object> { { "numOfInventory", 18 }, { "numOfHotbars", 9 } } },
        { Backpack.Everwoven, new Dictionary<string, object> { { "numOfInventory", 27 }, { "numOfHotbars", 9 } } }
    };

    // 0=Head, 1=Chest, 2=Legs, 3=Feet
    //[SerializeField] InventorySlot[] equipmentSlots;

    [SerializeField] Transform draggablesTransform;
    [SerializeField] InventoryItem itemPrefab;

    [Header("Item List")]
    [SerializeField] Item[] items;

    [Header("Debug")]
    [SerializeField] Button giveItemBtn;

    void Awake()
    {
        Singleton = this;
        giveItemBtn.onClick.AddListener(delegate { SpawnInventoryItem(); });

        for (int i = 0; i < numOfHotbars; i++)
        {
            InventorySlot newSlot = Instantiate(slotPrefab, HotbarContainer.transform);
            hotbarSlots.Add(newSlot);
        }
        for (int i = 0; i < numOfInventory; i++)
        {
            InventorySlot newSlot = Instantiate(slotPrefab, InventoryContainer.transform);
            inventorySlots.Add(newSlot);
        }
    }

    void Start()
    {
        CloseInventory();
    }

    void Update()
    {
        if (carriedItem == null) return;

        carriedItem.transform.position = Input.mousePosition;
    }

    public void OpenInventory()
    {
        foreach (InventorySlot slot in hotbarSlots)
        {
            slot.transform.SetParent(HotbarContainer.transform);
        }
    }
    public void CloseInventory()
    {
        foreach (InventorySlot slot in hotbarSlots)
        {
            slot.transform.SetParent(HotbarDisplay.transform);
        }
    }

    public void SetCarriedItem(InventoryItem item)
    {
        if (carriedItem != null)
        {
            //if (item.activeSlot.myTag != SlotTag.None && item.activeSlot.myTag != carriedItem.myItem.itemTag) return;
            item.activeSlot.SetItem(carriedItem);
        }

        //if (item.activeSlot.myTag != SlotTag.None)
        //{ EquipEquipment(item.activeSlot.myTag, null); }

        carriedItem = item;
        carriedItem.canvasGroup.blocksRaycasts = false;
        item.transform.SetParent(draggablesTransform);
    }

    /*public void EquipEquipment(SlotTag tag, InventoryItem item = null)
    {
        switch (tag)
        {
            case SlotTag.Head:
                if (item == null)
                {
                    // Destroy item.equipmentPrefab on the Player Object;
                    Debug.Log("Unequipped helmet on " + tag);
                }
                else
                {
                    // Instantiate item.equipmentPrefab on the Player Object;
                    Debug.Log("Equipped " + item.myItem.name + " on " + tag);
                }
                break;
            case SlotTag.Chest:
                break;
            case SlotTag.Legs:
                break;
            case SlotTag.Feet:
                break;
        }
    }*/

    public void SpawnInventoryItem(Item item = null)
    {
        Item _item = item;
        if (_item == null)
        { _item = PickRandomItem(); }

        InventorySlot stackableSlot = FindStackableSlot(_item);
        if (stackableSlot != null)
        {
            stackableSlot.myItem.Amount += 1;
            return; 
        }

        if (!IsHotbarFull())
        {
            foreach (InventorySlot slot in hotbarSlots)
            {
                if (slot.myItem == null)
                {

                    Instantiate(itemPrefab, slot.transform).Initialize(_item, slot);
                    break;
                }
            }
        }
        else
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.myItem == null)
                {

                    Instantiate(itemPrefab, slot.transform).Initialize(_item, slot);
                    break;
                }
            }
        }
    }

    public bool IsHotbarFull()
    {
        foreach (InventorySlot slot in hotbarSlots)
        {
            if (slot.myItem == null)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsStorageFull()
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.myItem == null)
            {
                return false;
            }
        }
        return true;
    }

    public InventorySlot FindStackableSlot(Item item)
    {
        if (item.maxStack < 2) { return null; }

        foreach (InventorySlot slot in hotbarSlots)
        {
            if (slot.myItem != null)
            {
                if (slot.myItem.myItem == item)
                {
                    if (slot.myItem.Amount < slot.myItem.myItem.maxStack)
                    {
                        return slot;
                    }
                }
            }
        }
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.myItem != null)
            {
                if (slot.myItem.myItem == item)
                {
                    if (slot.myItem.Amount < slot.myItem.myItem.maxStack)
                    {
                        return slot;
                    }
                }
            }
        }
        return null;

    }

    Item PickRandomItem()
    {
        int random = Random.Range(0, items.Length);
        return items[random];
    }
}
