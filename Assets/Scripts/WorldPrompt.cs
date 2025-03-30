using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum PromptMode
{
    Linear, Exponential
}

[RequireComponent (typeof(SphereCollider))]
public class WorldPrompt : MonoBehaviour
{
    [Header("World Settings")]
    public float coverDistance = 1f;
    public PromptMode promptMode = PromptMode.Linear;
    [Range(0f, 1f)] public float threshold = 0.2f; // % distance from being full. 0.2 would mean anything below 20% is full.
    private GameObject player;

    [Header("Prompt Settings")]
    public string prompt;
    private RectTransform rootPanel;
    private TextMeshProUGUI promptTextLabel;
    private Vector2 initialSize;

    private void Start()
    {
        GetComponent<SphereCollider>().radius = coverDistance; // used in a trigger loop to see if player has entered range (cover distance) of prompt.

        promptTextLabel = GameObjectFinder.FindChildRecursive(gameObject, "PromptText").GetComponent<TextMeshProUGUI>(); // Find the text element being used.
        promptTextLabel.text = prompt;

        rootPanel = GameObjectFinder.FindChildRecursive(gameObject, "RootPanel").GetComponent<RectTransform>();
        initialSize = rootPanel.sizeDelta;
    }

    private void Update()
    {
        if (!player) return;

        //Vector3 promptPosition = new Vector3(transform.position.x, 0, transform.position.z);
        float distance = Vector3.Distance(player.transform.position, transform.position);
        //if (distance > coverDistance) return;
        float normalizedDistance = distance / (coverDistance * (1 + threshold)); // 0 = close, 1 = far

        //Debug.Log($"[distance: {distance}] / [coverDistance: {(coverDistance * (1 + threshold))}] = {normalizedDistance}");
        if (normalizedDistance < threshold) // if less than threshold, automatically max
        {
            rootPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(rootPanel.GetComponent<RectTransform>().anchoredPosition.x, 0.5f);

            rootPanel.sizeDelta = Vector2.one;

            GetComponent<CanvasGroup>().alpha = 1;
        }
        else // if distance is within threshold then
        {
            float invertedNormalizedDistance = Mathf.InverseLerp(threshold, 1, threshold + (1f - normalizedDistance));
            //Debug.Log($"[normalizedDistance: {normalizedDistance}], [1f - normalizedDistance: {1 - normalizedDistance}]");

            float alpha = 0f;
            if (promptMode == PromptMode.Linear)
            {
                alpha = invertedNormalizedDistance;
            }
            else if (promptMode == PromptMode.Exponential)
            {
                alpha = Mathf.Pow(invertedNormalizedDistance, 2); // Exponential fade
            }

            float clampedY = Mathf.Lerp(0, 0.5f, invertedNormalizedDistance);
            rootPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(rootPanel.GetComponent<RectTransform>().anchoredPosition.x, clampedY);

            Vector2 lerpedSize = Vector2.Lerp(initialSize, Vector2.one, invertedNormalizedDistance);//1 + Mathf.Lerp(0.75f, 1f, invertedNormalizedDistance);
            rootPanel.sizeDelta = lerpedSize;//initialSize * sizeMultiplier;

            GetComponent<CanvasGroup>().alpha = alpha;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            player = null;
            // Immediately Reset
            GetComponent<CanvasGroup>().alpha = 0;
            rootPanel.sizeDelta = initialSize;
        }
    }
}
