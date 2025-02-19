using UnityEngine;
using UnityEngine.InputSystem;

public class TD_PlayerController : MonoBehaviour
{

    [Header("Movement")]
    public float speed = 10f;
    private Vector2 move;
    private Vector3 lastDirection = Vector3.forward;
    private Rigidbody rb;
    private Vector3 velocity = Vector3.zero;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 45f;
    private bool onSlope = false;
    private Vector3 slopeNormal;

    [Header("temp")]
    public CharacterAnimator character;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    void Update()
    {
        RotatePlayer();
        MovePlayer();


    }

    public void RotatePlayer()
    {
        Vector3 movement = new Vector3(move.x, 0f, move.y).normalized;

        if (movement.magnitude > 0.01f)
        {
            lastDirection = movement;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lastDirection), 0.15f);
        }
    }

    public void MovePlayer()
    {
        Vector3 movement = new Vector3(move.x, 0f, move.y).normalized * speed;

        // Check if player is on a slope
        if (OnSlope())
        {
            movement = Vector3.ProjectOnPlane(movement, slopeNormal);
        }

        //transform.Translate(movement * speed * Time.deltaTime * 0.07f, Space.World);
        //characterController.Move(movement * speed * Time.deltaTime);
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);

        float movementSum = Mathf.Abs(movement.x) + Mathf.Abs(movement.y) + Mathf.Abs(movement.z);
        if (movementSum > 5f)
        {
            character.SetCharacterState(CharacterState.Running);
        }
        else if (movementSum > 0.1f)
        {
            character.SetCharacterState(CharacterState.Walking);
        }
        else
        {
            character.SetCharacterState(CharacterState.Idle);
        }
    }

    bool OnSlope()
    {
        // Cast a downward ray to detect slope
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f))
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if (angle < maxSlopeAngle && angle > 0f)
            {
                slopeNormal = hit.normal;
                return true;
            }
        }
        return false;
    }

}
