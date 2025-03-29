using UnityEngine;
using UnityEngine.EventSystems;

public class SellStation : MonoBehaviour, IDropHandler
{
    [Header("UI")]
    [SerializeField] private GameObject sellPanel;

    public void OnDrop(PointerEventData eventData)
    {
        if (Inventory.carriedItem != null)
        {
            Item item = Inventory.carriedItem.myItem;
            int value = item.value * Inventory.carriedItem.Amount;

            Inventory.Singleton.PlayerGold += value;
            Debug.Log($"Sold {item.name} x{Inventory.carriedItem.Amount} for {value} gold. Total Gold: {Inventory.Singleton.PlayerGold}");

            Inventory.carriedItem.activeSlot.myItem = null;
            Destroy(Inventory.carriedItem.gameObject);
            Inventory.carriedItem = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && sellPanel != null)
        {
            sellPanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && sellPanel != null)
        {
            sellPanel.SetActive(false);
        }
    }
}
