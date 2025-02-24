using UnityEngine;

public enum ToolTag { None, Mining, Woodcutting, Farming, Fighting }

public class BasicTool : MonoBehaviour
{

    [Header("Animation")]
    public Animator playerAnimator;
    public GameObject toolModel;

    [Header("Tool Features")]
    public ToolTag toolTag;
    public float swingSpeed = 1;

    void Start()
    {
        playerAnimator = GetComponentInParent<Animator>();

        if (playerAnimator)
        {
            playerAnimator.SetFloat("SwingSpeed", swingSpeed);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (playerAnimator)
            {
                if (toolTag == ToolTag.Mining || toolTag == ToolTag.Woodcutting || toolTag == ToolTag.Farming)
                {
                    playerAnimator.SetTrigger("Wallop");
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ore"))
        {
            if (toolTag == ToolTag.Mining)
            {
                Debug.Log("Hit Ore! Breaking...");
                // Add logic to apply damage or trigger destruction
                Destroy(other.gameObject);
            }
        }

        if (other.CompareTag("Tree"))
        {
            if (toolTag == ToolTag.Woodcutting)
            {
                Debug.Log("Hit Tree! Chopping...");
                // Add logic to apply damage or trigger destruction
                Destroy(other.gameObject);
            }
        }

        if (other.CompareTag("Farmland"))
        {
            if (toolTag == ToolTag.Farming)
            {
                Debug.Log("Hit Bush! Farming...");
                // Add logic to apply damage or trigger destruction
                Destroy(other.gameObject);
            }
        }
    }
}
