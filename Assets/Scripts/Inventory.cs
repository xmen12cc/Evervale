using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

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
    public int PlayerGold = 500;
    [SerializeField] GameObject InventoryContainer;
    [SerializeField] GameObject InventoryPanel;//HotbarContainer;
    [SerializeField] GameObject HotbarPanel;//HotbarDisplay;
    [SerializeField] InventorySlot slotPrefab;

    public static Inventory Singleton;
    public static InventoryItem carriedItem;

    [SerializeField] public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    [SerializeField] public List<InventorySlot> hotbarSlots = new List<InventorySlot>();

    [Header("Upgrades")]
    public Backpack backpack;
    Dictionary<Backpack, Dictionary<string, Color>> backpackDetails = new Dictionary<Backpack, Dictionary<string, Color>>
    {
        { Backpack.DEFAULT, new Dictionary<string, Color>
            {
                { "baseColor", new Color(0.84f, 0.78f, 0.69f) },
                { "outerColor", new Color(0.7f, 0.59f, 0.46f) },
                { "depthColor", new Color(0.48f, 0.38f, 0.15f) }
            }
        },
        { Backpack.Travelers, new Dictionary<string, Color>
            {
                { "baseColor", new Color(0, 0, 0) },
                { "outerColor", new Color(0, 0, 0) },
                { "depthColor", new Color(0, 0, 0) }
            }
        },
        { Backpack.Adventurers, new Dictionary<string, Color>
            {
                { "baseColor", new Color(0, 0, 0) },
                { "outerColor", new Color(0, 0, 0) },
                { "depthColor", new Color(0, 0, 0) }
            }
        },
        { Backpack.Everwoven, new Dictionary<string, Color>
            {
                { "baseColor", new Color(0, 0, 0) },
                { "outerColor", new Color(0, 0, 0) },
                { "depthColor", new Color(0, 0, 0) }
            }
        }
    };
    /*Dictionary<Backpack, Dictionary<string, object>> backpackDetails = new Dictionary<Backpack, Dictionary<string, object>> {
        { Backpack.DEFAULT, new Dictionary<string, object> { { "numOfInventory", 9 }, { "numOfHotbars", 6 } } },
        { Backpack.Travelers, new Dictionary<string, object> { { "numOfInventory", 18 }, { "numOfHotbars", 6 } } },
        { Backpack.Adventurers, new Dictionary<string, object> { { "numOfInventory", 18 }, { "numOfHotbars", 9 } } },
        { Backpack.Everwoven, new Dictionary<string, object> { { "numOfInventory", 27 }, { "numOfHotbars", 9 } } }
    };*/

    // 0=Head, 1=Chest, 2=Legs, 3=Feet
    //[SerializeField] InventorySlot[] equipmentSlots;

    [SerializeField] Transform draggablesTransform;
    [SerializeField] InventoryItem itemPrefab;

    [Header("Item List")]
    [SerializeField] Item[] items;

    [Header("Debug")]
    [SerializeField] Button giveItemBtn;

    [Header("Keybinds")]
    [SerializeField] KeyCode toggleKey;
    private bool isInventoryOpen = false;

    void Awake()
    {
        Singleton = this;
        giveItemBtn.onClick.AddListener(delegate { SpawnInventoryItem(); });

        GameObject hotbarContainer = GameObjectFinder.FindChildRecursive(InventoryPanel, "Hotbar");
        GameObject inventoryContainer = GameObjectFinder.FindChildRecursive(InventoryPanel, "Storage");

        for (int i = 0; i < numOfHotbars; i++)
        {
            InventorySlot newSlot = Instantiate(slotPrefab, hotbarContainer.transform);
            hotbarSlots.Add(newSlot);
        }
        for (int i = 0; i < numOfInventory; i++)
        {
            InventorySlot newSlot = Instantiate(slotPrefab, inventoryContainer.transform);
            inventorySlots.Add(newSlot);
        }
    }

    void Start()
    {
        CloseInventory();

        SetBackpack();
    }

    void Update()
    {
        CheckKeys();

        if (carriedItem == null) return;

        carriedItem.transform.position = Input.mousePosition;
    }

    private void CheckKeys()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isInventoryOpen = !isInventoryOpen;
            if (isInventoryOpen)
            {
                OpenInventory();
            }
            else
            {
                CloseInventory();
            }
        }
    }

    public bool IsInventoryOpen()
    {
        return isInventoryOpen;
    }

    public void OpenInventory()
    {
        InventoryPanel.SetActive(true);
        HotbarPanel.SetActive(false);

        GameObject hotbarContainer = GameObjectFinder.FindChildRecursive(InventoryPanel, "Hotbar");

        foreach (InventorySlot slot in hotbarSlots)
        {
            slot.interactable = true;
            slot.transform.SetParent(hotbarContainer.transform);
        }
    }
    public void CloseInventory()
    {
        InventoryPanel.SetActive(false);
        HotbarPanel.SetActive(true);

        GameObject hotbarDisplay = GameObjectFinder.FindChildRecursive(HotbarPanel, "Hotbar (Display)");

        foreach (InventorySlot slot in hotbarSlots)
        {
            slot.interactable = false;
            slot.transform.SetParent(hotbarDisplay.transform);
        }
    }

    private void SetBackpack()
    {
        Color baseColor = Color.white;
        Color outerColor = Color.gray;
        Color depthColor = Color.black;

        if (backpackDetails.TryGetValue(backpack, out Dictionary<string, Color> colors))
        {
            baseColor = colors["baseColor"];
            outerColor = colors["outerColor"];
            depthColor = colors["depthColor"];
        }

        InventoryPanel.GetComponent<Image>().color = baseColor;
        GameObjectFinder.FindChildRecursive(InventoryPanel, "OuterDesign").GetComponent<Image>().color = outerColor;
        GameObjectFinder.FindChildRecursive(InventoryPanel, "Hotbar").GetComponent<Image>().color = depthColor;
        GameObjectFinder.FindChildRecursive(InventoryPanel, "Storage").GetComponent<Image>().color = depthColor;

        HotbarPanel.GetComponent<Image>().color = baseColor;
        GameObjectFinder.FindChildRecursive(HotbarPanel, "OuterDesign").GetComponent<Image>().color = outerColor;
    }

    public void RemoveItem(Item item)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.myItem != null && slot.myItem == item)
            {
                inventorySlots.Remove(slot);
                return;
            }
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
                    if (slot.border != null) { slot.border.SetAsLastSibling(); }
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

    public List<InventorySlot> GetItems()
    {
        return inventorySlots;
    }

    public InventorySlot AddItem(Item item = null, int amount = 1, Loot lootObject = null)
    {
        InventorySlot[] allInventorySlots = hotbarSlots.Concat(inventorySlots).ToArray();

        Debug.Log($"[Inventory][AddItem] {item.name} (x{amount})");
        //InventorySlot _slot = null;
        int remainingAmount = amount;

        // If item is stackable, try adding to an existing stack first
        while (remainingAmount > 0)
        {
            InventorySlot stackableSlot = FindStackableSlot(item);
            if (stackableSlot != null)
            {
                int availableSpace = stackableSlot.myItem.myItem.maxStack - stackableSlot.myItem.Amount;
                if (remainingAmount <= availableSpace)
                {
                    stackableSlot.myItem.Amount += remainingAmount;
                    return stackableSlot;
                }
                else
                {
                    stackableSlot.myItem.Amount = stackableSlot.myItem.myItem.maxStack;
                    remainingAmount -= availableSpace;
                }
            }
            else
            {
                break; // No more stackable slots available, move to empty slots
            }
        }

        // If there are remaining items, place them in empty slots
        if (remainingAmount > 0)
        {
            foreach (InventorySlot slot in hotbarSlots.Concat(inventorySlots))
            {
                if (slot.myItem == null)
                {
                    Instantiate(itemPrefab, slot.transform).Initialize(item, slot);
                    slot.myItem.Amount = remainingAmount;
                    if (slot.border != null) { slot.border.SetAsLastSibling(); }
                    return slot;
                }
            }
        }

        return null; // Inventory is full

        /*Debug.Log("[Inventory][AddItem] " + item.name);
        InventorySlot _slot = null;
        Item _item = item;
        int _amount = amount;

        InventorySlot stackableSlot = FindStackableSlot(_item);
        if (stackableSlot != null) // if a stackable slot was found
        {
            if (stackableSlot.myItem.Amount + _amount <= stackableSlot.myItem.myItem.maxStack) // if the item's amount results in less than max
            {
                stackableSlot.myItem.Amount += _amount;
                return stackableSlot;
            }
            if (lootObject != null)
            {
                //(item.maxStack - stackableSlot.myItem.Amount)
            }
            _slot = stackableSlot; // remember stackable slot for later
            //return null; // return because if the item is picked up the player will lose value
        }

        if (!IsHotbarFull())
        {
            foreach (InventorySlot slot in hotbarSlots)
            {
                if (slot.myItem == null)
                {
                    if (_slot != null) // if _slot has already been set to stackable slot
                    {
                        _amount -= _slot.myItem.myItem.maxStack - _slot.myItem.Amount; // take away what can be given to stackable slot
                        _slot.myItem.Amount = _slot.myItem.myItem.maxStack;
                        _slot = slot;
                    }

                    //Debug.Log($"[Inventory][AddItem][!IsHotbarFull()][slot.myItem is null] {itemPrefab.ToString()}, {_slot.name}, {_item.name}");
                    Debug.Log(itemPrefab);
                    Debug.Log(_slot);
                    Debug.Log(_item);
                    Instantiate(itemPrefab, _slot.transform).Initialize(_item, _slot);
                    _slot.myItem.Amount = _amount;
                    if (_slot.border != null) { _slot.border.SetAsLastSibling(); }
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
                    if (_slot != null) // if _slot has already been set to stackable slot
                    {
                        _amount -= _slot.myItem.myItem.maxStack - _slot.myItem.Amount;
                        _slot.myItem.Amount = _slot.myItem.myItem.maxStack;
                        _slot = slot;
                    }

                    Instantiate(itemPrefab, slot.transform).Initialize(_item, slot);
                    _slot.myItem.Amount = _amount;
                    if (_slot.border != null) { _slot.border.SetAsLastSibling(); }
                    break;
                }
            }
        }
        if (_slot == stackableSlot) { _slot = null; } // If slot never changed from stackable then it means inventory was full

        return _slot;*/

    }



}
