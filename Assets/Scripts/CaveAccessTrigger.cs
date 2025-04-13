using UnityEngine;
using UnityEngine.SceneManagement;

public class CaveAccessTrigger : MonoBehaviour
{
    public GameObject caveAccessPanel;
    public int accessPrice = 200;
    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            caveAccessPanel.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            caveAccessPanel.SetActive(false);
        }
    }

    public void BuyCaveAccess()
    {
        if (Inventory.Singleton.PlayerGold >= accessPrice)
        {
            Inventory.Singleton.PlayerGold -= accessPrice;
            SceneManager.LoadScene("EndScene");
        }
        else
        {
            Debug.Log("Not enough gold to enter the cave.");
        }
    }

    public void CancelAccess()
    {
        caveAccessPanel.SetActive(false);
    }
}

