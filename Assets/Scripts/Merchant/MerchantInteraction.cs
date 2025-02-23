using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MerchantInteraction : MonoBehaviour
{
    [SerializeField] private Merchant merchant;
    [SerializeField] private Text interactionText;
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed! Opening merchant shop...");
            merchant.OpenShop();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered range.");
            playerInRange = true;
            interactionText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left range.");
            playerInRange = false;
            merchant.CloseShop();
            interactionText.gameObject.SetActive(false);
        }
    }
}
