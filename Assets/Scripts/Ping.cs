using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public enum PingType
{
    Default,
    Item
}

public class Ping : MonoBehaviour
{
    public PingType pingType = PingType.Default;
    public float lifeTime = 1f;

    public Item item;
    public int amount = 1;
    public bool isError = false;
    public string errorCode;

    private void OnEnable()
    {
        ShowMessage(pingType);
    }

    public void ShowMessage(PingType pingType)
    {
        if (pingType == PingType.Default)
        {
            TextMeshProUGUI pingNameText = GameObjectFinder.FindChildRecursive(gameObject, "PingName").GetComponent<TextMeshProUGUI>();
            pingNameText.text = "Null";
        }
        else if (pingType == PingType.Item)
        {
            TextMeshProUGUI itemNameText = GameObjectFinder.FindChildRecursive(gameObject, "PingName").GetComponent<TextMeshProUGUI>();
            itemNameText.text = item.name;

            Image itemImage = GameObjectFinder.FindChildRecursive(gameObject, "ItemImage").GetComponent<Image>();
            itemImage.sprite = item.sprite;

            if (amount > 1)
            {
                GameObject amountText = GameObjectFinder.FindChildRecursive(gameObject, "Amount");
                amountText.GetComponent<TextMeshProUGUI>().text = amount.ToString();
                amountText.SetActive(true);
            }
        }

        if (isError)
        {
            GameObject errorPanel = GameObjectFinder.FindChildRecursive(gameObject, "ErrorPanel");
            errorPanel.SetActive(true);

            TextMeshProUGUI errorMessage = GameObjectFinder.FindChildRecursive(errorPanel, "MessageText").GetComponent<TextMeshProUGUI>();
            errorMessage.text = errorCode;
        }

        StartCoroutine(DeleteMessage());
    }

    private IEnumerator DeleteMessage()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }
}
