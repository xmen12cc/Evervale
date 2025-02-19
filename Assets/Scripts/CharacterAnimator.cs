using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public enum CharacterState
{
    Idle,
    Walking,
    Running,
}

public enum ItemState
{
    None,   // If ItemState is inactive
    Swing,  // If item acts like a sword
    Wallop, // If item acts like an axe, pickaxe, or hoe
    Carry,  // If item acts like a normal item
}

public class CharacterAnimator : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public GameObject body;
    public GameObject itemHolder;

    private Animation anim;
    private GameObject heldObject;
    private bool triggerCharacterAnim, triggerItemAnim;

    private CharacterState currentState;
    private ItemState itemState;

    void Start()
    {
        animator = GetComponent<Animator>();

        currentState = CharacterState.Idle;
        itemState = ItemState.None;

        //anim = null;
        //heldObject = null;
    }

    void Update()
    {
        switch (currentState)
        {
            case CharacterState.Idle:
                animator.SetBool("Walking", false);
                animator.SetBool("Running", false);
                break;
            case CharacterState.Walking:
                animator.SetBool("Walking", true);
                animator.SetBool("Running", false);
                break;
            case CharacterState.Running:
                animator.SetBool("Walking", true);
                animator.SetBool("Running", true);
                break;
            default:
                animator.SetBool("Walking", false);
                animator.SetBool("Running", false);
                break;
        }

        switch (itemState)
        {
            case ItemState.None:
                break;
            case ItemState.Swing:
                if (triggerItemAnim)
                {
                    triggerItemAnim = false;
                    // trigger swing animation
                }
                break;
            case ItemState.Wallop:
                if (triggerItemAnim)
                {
                    triggerItemAnim = false;
                    // trigger wallop animation
                }
                break;
            case ItemState.Carry:
                break;
            default:
                break;
        }
    }

    public void SetCharacterState(CharacterState state)
    {
        currentState = state;
    }
    public void SetItemState(ItemState state)
    {
        itemState = state;
    }
    public void TriggerAnim(string type)
    {
        if (type == "Character") { triggerCharacterAnim = true; }
        else if (type == "Item") { triggerItemAnim = true; }
    }

    public void SetHeldObject(GameObject prefab)
    {
        // set heldObject to an instantiated clone of the prefab and set it to be held in the player's right hand
    }
}
