using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public InventoryItem myItem { get; set; }
    private bool isHoldingItem = false;

    private Vector3 originalPosition;
    private InventorySlot originalSlot;

    //public SlotTag myTag;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Debug.Log("shiftclick");
                MoveItemBetweenSlots();
                return;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && myItem != null && !Input.GetKey(KeyCode.LeftShift))
        {
            Debug.Log("ClickHeld");
            isHoldingItem = true;
            Inventory.Singleton.SetCarriedItem(myItem);

            // Save original position and slot in case we need to reset
            originalPosition = myItem.transform.position;
            originalSlot = myItem.activeSlot;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isHoldingItem)
        {
            Debug.Log("ClickReleased");

            isHoldingItem = false;

            // Check if we released over a valid slot
            InventorySlot targetSlot = GetSlotUnderCursor();
            if (targetSlot != null)
            {
                targetSlot.SetItem(Inventory.carriedItem);
            }
            else
            {
                // Reset to original slot if not dropped on a valid one
                originalSlot.SetItem(Inventory.carriedItem);
            }

            Inventory.carriedItem = null;
        }
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

        Inventory.carriedItem = null;

        // Reset old slot
        item.activeSlot.myItem = null;

        // Set current slot
        myItem = item;
        myItem.activeSlot = this;
        myItem.transform.SetParent(transform);

        myItem.transform.localPosition = new Vector3(47.4445f, 0,0);

        myItem.canvasGroup.blocksRaycasts = true;

        //if (myTag != SlotTag.None)
        //{ Inventory.Singleton.EquipEquipment(myTag, myItem); }
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
