using UnityEngine;

public enum ItemTag { None, Food, Tool }

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public Sprite sprite;
    public ItemTag itemTag;

    public string description;

    public int maxStack;

    public int sellPrice;

    [Header("If the item can be held")]
    public GameObject itemPrefab;
}