using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Singleton;
    public static InventoryItem carriedItem;

    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] InventorySlot[] hotbarSlots;

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
    }

    void Update()
    {
        if (carriedItem == null) return;

        carriedItem.transform.position = Input.mousePosition;
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
