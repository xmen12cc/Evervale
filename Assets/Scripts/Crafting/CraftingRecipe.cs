using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CraftingRecipe
{
    public string recipeName;
    public List<Item> requiredItems;
    public Item resultItem;
}
