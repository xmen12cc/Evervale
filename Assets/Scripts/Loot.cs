using NUnit.Framework.Interfaces;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public Item item;
    public int amount = 1;
    public GameObject pingPrefab;
    private bool isCollected = false;

    public void OnEnable()
    {
        GameObject container = GameObjectFinder.FindChildRecursive(gameObject, "ObjectContainer");

        Instantiate(item.itemPrefab, container.transform.position, Quaternion.identity, container.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("[Loot] " + item.name);
            InventorySlot result = Inventory.Singleton.AddItem(item, amount);

            if (result != null)
            {
                isCollected = true;

                GameObject pingObject = Instantiate(pingPrefab);
                //Debug.Log(HUD.Singleton);
                GameObject pingPanel = GameObjectFinder.FindChildRecursive(HUD.Singleton.gameObject, "PingPanel");
                pingObject.transform.SetParent(pingPanel.transform, false);
                pingObject.GetComponent<Ping>().pingType = PingType.Item;
                pingObject.GetComponent<Ping>().lifeTime = 5f;
                pingObject.GetComponent<Ping>().item = item;
                pingObject.GetComponent<Ping>().amount = amount;
                pingObject.SetActive(true);

                Destroy(gameObject);
            }
            else
            {
                GameObject pingObject = Instantiate(pingPrefab);
                GameObject pingPanel = GameObjectFinder.FindChildRecursive(HUD.Singleton.gameObject, "PingPanel");
                pingObject.transform.SetParent(pingPanel.transform, false);
                pingObject.GetComponent<Ping>().pingType = PingType.Item;
                pingObject.GetComponent<Ping>().lifeTime = 3f;
                pingObject.GetComponent<Ping>().item = item;
                pingObject.GetComponent<Ping>().amount = amount;

                pingObject.GetComponent<Ping>().isError = true;
                pingObject.GetComponent<Ping>().errorCode = "Inventory Full";

                pingObject.SetActive(true);
            }
        }
    }
}
