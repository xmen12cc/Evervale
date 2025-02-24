using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class HotbarController : MonoBehaviour
{
    [Header("Inventory")]
    public Inventory inventory;
    private int hotbarIndex = 0;
    private InventorySlot selectedHotbar;

    [Header("Rig")]
    public GameObject objectHolder;

    private GameObject equippedItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateSelection();

        selectedHotbar = GetHotbarSlotFromIndex(hotbarIndex);
    }

    // Update is called once per frame
    void Update()
    {
        HandleNumberKeySelection();
        HandleScrollSelection();
    }

    void HandleNumberKeySelection()
    {
        for (int i = 0; i < inventory.hotbarSlots.Count; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                hotbarIndex = i;
                selectedHotbar = GetHotbarSlotFromIndex(hotbarIndex);
                EquipItem();
                UpdateSelection();
                return;
            }
        }
    }

    void HandleScrollSelection()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            hotbarIndex = (hotbarIndex - 1 + inventory.hotbarSlots.Count) % inventory.hotbarSlots.Count;
            selectedHotbar = GetHotbarSlotFromIndex(hotbarIndex);
            EquipItem();
            UpdateSelection();
        }
        else if (scroll < 0f)
        {
            hotbarIndex = (hotbarIndex + 1) % inventory.hotbarSlots.Count;
            selectedHotbar = GetHotbarSlotFromIndex(hotbarIndex);
            EquipItem();
            UpdateSelection();
        }
    }

    void UpdateSelection()
    {

    }

    void EquipItem()
    {
        if (equippedItem != null) { Destroy(equippedItem); } // Remove previous tool

        if (selectedHotbar.myItem != null)
        {
            if (selectedHotbar.myItem.myItem != null)
            {
                Item nextItem = selectedHotbar.myItem.myItem;

                if (nextItem != null && nextItem.itemPrefab != null)
                {
                    if (objectHolder != null)
                    {
                        equippedItem = Instantiate(nextItem.itemPrefab);

                        Debug.Log($"Before Parenting: Position={equippedItem.transform.position}, Scale={equippedItem.transform.localScale}");

                        equippedItem.transform.SetParent(objectHolder.transform, false);

                        equippedItem.transform.localPosition = Vector3.zero;
                        equippedItem.transform.localRotation = Quaternion.identity;

                        Vector3 parentScale = objectHolder.transform.lossyScale;
                        equippedItem.transform.localScale = new Vector3(
                            1f / parentScale.x,
                            1f / parentScale.y,
                            1f / parentScale.z
                        );

                        Debug.Log($"After Parenting: Position={equippedItem.transform.position}, Scale={equippedItem.transform.localScale}");
                    }
                }
            }
        }
    }

    InventorySlot GetHotbarSlotFromIndex(int index)
    {
        return inventory.hotbarSlots[index];
    }
}
