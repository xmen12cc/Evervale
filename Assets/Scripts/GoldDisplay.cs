using TMPro;
using UnityEngine;

public class GoldDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;

    private void Start()
    {
        UpdateGoldDisplay(Inventory.Singleton.PlayerGold); // Force first update
        Inventory.OnGoldChanged += UpdateGoldDisplay;
    }

    private void Update()
    {
        goldText.text = $"Gold: {Inventory.Singleton.PlayerGold}";
    }

    private void UpdateGoldDisplay(int amount)
    {
        goldText.text = $"Gold: {amount}";
    }

    private void OnDestroy()
    {
        Inventory.OnGoldChanged -= UpdateGoldDisplay;
    }
}
