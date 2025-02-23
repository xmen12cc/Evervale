using UnityEngine;
using System.Collections.Generic;

public class CraftingSystem : MonoBehaviour
{
    [SerializeField] private List<CraftingRecipe> recipes = new List<CraftingRecipe>();
    [SerializeField] private Inventory playerInventory;

    public void CraftItem(CraftingRecipe recipe)
    {
        if (CanCraft(recipe))
        {
            foreach (Item requiredItem in recipe.requiredItems)
            {
                playerInventory.RemoveItem(requiredItem);
            }

            playerInventory.SpawnInventoryItem(recipe.resultItem);
            Debug.Log("Crafted: " + recipe.resultItem.name);
        }
        else
        {
            Debug.Log("Not enough materials to craft " + recipe.resultItem.name);
        }
    }

    private bool CanCraft(CraftingRecipe recipe)
    {
        Dictionary<Item, int> inventoryCount = new Dictionary<Item, int>();

        foreach (InventorySlot slot in playerInventory.inventorySlots)
        {
            if (slot.myItem != null)
            {
                if (!inventoryCount.ContainsKey(slot.myItem.myItem))
                {
                    inventoryCount[slot.myItem.myItem] = 0;
                }
                inventoryCount[slot.myItem.myItem] += slot.myItem.Amount;
            }
        }

        foreach (Item requiredItem in recipe.requiredItems)
        {
            if (!inventoryCount.ContainsKey(requiredItem) || inventoryCount[requiredItem] < 1)
            {
                return false;
            }
        }
        return true;
    }
}
