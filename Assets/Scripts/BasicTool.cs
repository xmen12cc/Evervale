using UnityEditor;
using UnityEngine;

public enum ToolTag { None, Mining, Woodcutting, Farming, Fighting }

public class BasicTool : MonoBehaviour
{

    [Header("Animation")]
    public Animator playerAnimator;
    public GameObject toolModel;

    [Header("Tool Features")]
    public ToolTag toolTag;
    public float damage = 1f;
    public float swingSpeed = 1;

    Inventory inventory = Inventory.Singleton;

    public bool isActionPlaying;

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
        if (!inventory.IsInventoryOpen())
        {
            // Check if mouse down
            //AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);

            //Debug.Log($"Can swing? {(!stateInfo.IsName("Wallop") && !isActionPlaying)}");
            if (Input.GetMouseButton(0))
            {
                if (playerAnimator)
                {
                    // Check if animation is still playing
                    AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);

                    if (!stateInfo.IsName("Wallop") && !isActionPlaying)
                    {
                        if (toolTag == ToolTag.Mining || toolTag == ToolTag.Woodcutting || toolTag == ToolTag.Farming)
                        {
                            //Debug.Log("Wallop");
                            playerAnimator.SetTrigger("Wallop");
                            isActionPlaying = true;
                        }
                    }
                }
            }

            // Detect when animation finishes
            if (isActionPlaying)
            {
                AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
                //Debug.Log(stateInfo.normalizedTime);
                if (stateInfo.IsName("Wallop") && stateInfo.normalizedTime >= 0.98f)
                {
                    isActionPlaying = false;
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
                //Debug.Log("Hit Ore!");
                // Add logic to apply damage or trigger destruction
                //Destroy(other.gameObject);
            }
        }

        if (other.CompareTag("Tree"))
        {
            if (toolTag == ToolTag.Woodcutting)
            {
                //Debug.Log("Hit Tree!");
                // Add logic to apply damage or trigger destruction
                //Destroy(other.gameObject);
            }
        }

        if (other.CompareTag("Farmland"))
        {
            if (toolTag == ToolTag.Farming)
            {
                //Debug.Log("Hit Bush!");
                // Add logic to apply damage or trigger destruction
                //Destroy(other.gameObject);
            }
        }
    }
}
