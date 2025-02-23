using UnityEngine;

public enum ItemTag { None, Food, Tool }

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public Sprite sprite;
    public ItemTag itemTag;
    public int value;
    public string description;
    public int maxStack;

    [Header("If the item can be held")]
    public GameObject itemPrefab;

    public Item(string name, int value)
    {
        this.name = name;
        this.value = value;
    }
}
