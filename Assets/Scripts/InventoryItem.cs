using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    Image itemIcon;
    public CanvasGroup canvasGroup { get; private set; }

    public Item myItem { get; set; }
    public InventorySlot activeSlot { get; set; }

    public TextMeshProUGUI textBox;

    private int amount { get; set; }
    public int Amount
    {
        get { return amount; }
        set
        {
            if (amount != value)
            {
                amount = value;
                textBox.text = value.ToString();
            }
        }
    }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        itemIcon = GetComponent<Image>();
        if (textBox == null)
        {
            textBox = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public void Initialize(Item item, InventorySlot parent)
    {
        activeSlot = parent;
        activeSlot.myItem = this;
        myItem = item;
        if (itemIcon == null) { Awake(); } // Force Awake if added to storage mid game
        itemIcon.sprite = item.sprite;

        Amount += 1;

        if (myItem.maxStack < 2)
        {
            textBox.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (activeSlot != null)
        {
            activeSlot.OnPointerClick(eventData);
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //Inventory.Singleton.SetCarriedItem(this);
        }
    }
}
