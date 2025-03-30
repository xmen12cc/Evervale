using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SellStation : MonoBehaviour, IDropHandler
{
    [Header("UI")]
    [SerializeField] private GameObject sellPanel;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text confirmText;

    [Header("Audio")]
    [SerializeField] private AudioClip sellSound;
    private AudioSource audioSource;

    private InventoryItem pendingItem;
    private InventorySlot originalSlot;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        sellButton.onClick.AddListener(SellItem);
        cancelButton.onClick.AddListener(CancelSell);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (Inventory.carriedItem != null)
        {
            pendingItem = Inventory.carriedItem;
            originalSlot = pendingItem.activeSlot;

            confirmText.text = $"Sell {pendingItem.myItem.name} x{pendingItem.Amount}?";
            sellPanel.SetActive(true);

            pendingItem.canvasGroup.blocksRaycasts = false;
        }
    }

    private void SellItem()
    {
        if (pendingItem == null) return;

        Item item = pendingItem.myItem;
        int value = item.value * pendingItem.Amount;

        Inventory.Singleton.PlayerGold += value;
        Inventory.OnGoldChanged?.Invoke(Inventory.Singleton.PlayerGold);

        if (sellSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(sellSound);
        }

        Destroy(pendingItem.gameObject);
        pendingItem.activeSlot.myItem = null;
        pendingItem = null;

        confirmText.text = ""; // Reset text
        // NOTE: Panel remains open.
    }

    private void CancelSell()
    {
        if (pendingItem != null && originalSlot != null)
        {
            originalSlot.SetItem(pendingItem);
            pendingItem.canvasGroup.blocksRaycasts = true;
        }

        pendingItem = null;
        originalSlot = null;
        confirmText.text = ""; // Reset text
        // NOTE: Panel remains open.
    }


    public void TriggerSellConfirmation(InventoryItem item)
    {
        pendingItem = item;
        originalSlot = item.activeSlot;

        confirmText.text = $"Sell {item.myItem.name} x{item.Amount}?";
        sellPanel.SetActive(true);

        item.canvasGroup.blocksRaycasts = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && sellPanel != null)
        {
            sellPanel.SetActive(true);
            Debug.Log("Player entered sell zone.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && sellPanel != null)
        {
            sellPanel.SetActive(false);
            Debug.Log("Player exited sell zone.");
        }
    }
}
