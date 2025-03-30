using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public InventoryItem myItem { get; set; }
    public bool interactable = true;
    private bool isHoldingItem = false;
    public bool isShopSlot = false;


    private Vector3 originalPosition;
    private InventorySlot originalSlot;

    //public SlotTag myTag;

    public RectTransform border;

    private void Awake()
    {
        border = GameObjectFinder.FindChildRecursive(gameObject, "BorderDesign").GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[InventorySlot][OnPointerClick/]");
        if (interactable)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    Debug.Log("[InventorySlot][OnPointerClick][_ShiftClick]");
                    MoveItemBetweenSlots();
                    Debug.Log("[InventorySlot][OnPointerClick][MoveItemBetweenSlots()]");
                    return;
                }
            }
        }
        Debug.Log("[InventorySlot][/]");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("[InventorySlot][OnPointerDown/]");
        if (interactable)
        {
            if (eventData.button == PointerEventData.InputButton.Left && myItem != null && !Input.GetKey(KeyCode.LeftShift))
            {
                isHoldingItem = true;
                Debug.Log("[InventorySlot][OnPointerDown][_ClickHeld]");
                Inventory.Singleton.SetCarriedItem(myItem);
                Debug.Log($"[InventorySlot][OnPointerDown][Inventory.SetCarriedItem({myItem.myItem.name})]");

                // Save original position and slot in case we need to reset
                originalPosition = myItem.transform.position;
                originalSlot = this;//myItem.activeSlot;
            }
        }
        Debug.Log("[InventorySlot][/]");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("[InventorySlot][OnPointerUp/]");
        if (interactable)
        {
            if (isHoldingItem)
            {
                isHoldingItem = false;
                Debug.Log("[InventorySlot][OnPointerUp][_ClickReleased]");

                InventorySlot targetSlot = GetSlotUnderCursor();
                if (targetSlot != null)
                {
                    if (targetSlot.isShopSlot)
                    {
                        // SELLING logic for the shop
                        Item item = Inventory.carriedItem.myItem;
                        int amount = Inventory.carriedItem.Amount;
                        int value = item.value * amount;

                        Inventory.Singleton.PlayerGold += value;
                        Debug.Log($"[SOLD] {item.name} x{amount} for {value} gold.");

                        Inventory.carriedItem.activeSlot.myItem = null;
                        Destroy(Inventory.carriedItem.gameObject);
                        Inventory.carriedItem = null;
                        return;
                    }
                    else if (targetSlot.myItem == null)
                    {
                        targetSlot.SetItem(Inventory.carriedItem);
                        Debug.Log($"[InventorySlot][OnPointerUp][Moved {Inventory.carriedItem.myItem.name} To TargetSlot]");
                    }
                    else
                    {
                        InventoryItem targetSlotItem = targetSlot.myItem;
                        originalSlot.SetItem(targetSlotItem);
                        Debug.Log($"[InventorySlot][OnPointerUp][Moved {originalSlot.myItem.myItem.name} To OriginalSlot]");
                        targetSlot.SetItem(Inventory.carriedItem);
                        originalSlot.myItem = targetSlotItem;
                        Debug.Log($"[InventorySlot][OnPointerUp][Moved {Inventory.carriedItem.myItem.name} To TargetSlot]");
                    }
                }
                else
                {
                    originalSlot.SetItem(Inventory.carriedItem);
                    Debug.Log($"[InventorySlot][OnPointerUp][Moved {Inventory.carriedItem.myItem.name} Back To OriginalSlot]");
                }

                Debug.Log($"[InventorySlot][OnPointerUp][Set CarriedItem({Inventory.carriedItem.myItem.name}) To Null]");
                Inventory.carriedItem = null;
            }
        }
        Debug.Log("[InventorySlot][/]");
    }

    private InventorySlot GetSlotUnderCursor()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();
            if (slot != null)
            {
                return slot;
            }
        }

        return null;
    }

    public void SetItem(InventoryItem item)
    {
        if (item == null) return;
        Debug.Log($"[InventorySlot][SetItem({item.myItem.name})/]");

        //Inventory.carriedItem = null;

        // Reset old slot
        if (item.activeSlot.myItem != null) { Debug.Log($"[InventorySlot][SetItem({item.myItem.name})/][Set {item.activeSlot.myItem.myItem.name} To Null]"); }
        item.activeSlot.myItem = null;

        // Set current slot
        myItem = item;
        myItem.activeSlot = this;
        myItem.transform.SetParent(transform);

        myItem.transform.localPosition = new Vector3(47.4445f, 0,0);

        myItem.canvasGroup.blocksRaycasts = true;

        if (border != null) { border.SetAsLastSibling(); }

        //if (myTag != SlotTag.None)
        //{ Inventory.Singleton.EquipEquipment(myTag, myItem); }
        //Debug.Log("[InventorySlot][/]");
    }

    void MoveItemBetweenSlots()
    {
        if (myItem == null) return;

        Inventory inventory = Inventory.Singleton;

        // Determine the source and target slot lists
        bool isInHotbar = inventory.hotbarSlots.Contains(this);
        List<InventorySlot> sourceSlots = isInHotbar ? inventory.hotbarSlots : inventory.inventorySlots;
        List<InventorySlot> targetSlots = isInHotbar ? inventory.inventorySlots : inventory.hotbarSlots;

        // Find an available slot in the target list
        foreach (InventorySlot slot in targetSlots)
        {
            if (slot.myItem == null) // Empty slot found
            {
                InventoryItem tempItem = myItem;
                myItem = null; // Immediately clear from this slot
                slot.SetItem(tempItem); // Move the item

                return;
            }
        }

        Debug.Log("No available slot in target inventory.");
    }
}
