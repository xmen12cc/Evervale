using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Keybinds & Inputs")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;

    private float horizontalInput;
    private float verticalInput;

    Vector3 moveDirection;

    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    private float moveSpeed;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool canJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask GroundMask;

    public Transform orientation;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    Rigidbody rb;

    [Header("State Handling")]
    public MovementState state;

    public enum MovementState
    {
        walking, 
        sprinting, 
        crouching,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        canJump = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        MyInput();
        SpeedControl();
        StateHandler();

        // handle ground drag
        if (IsGrounded())
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && canJump && IsGrounded())
        {
            canJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouching
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouching
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, GroundMask);
    }

    private void StateHandler()
    {
        // Mode: Crouching
        if (IsGrounded() && Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Mode: Sprinting
        if (IsGrounded() && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }


        // Mode: Walking
        else if (IsGrounded())
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        // Mode: Air
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // On Slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        // On Ground
        if (IsGrounded())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        // Disable Gravity While On Slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
            }
        }
        else // limiting speed on ground or in air
        {
            Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            // limit velocity if needed
            if (flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
            }
        }
        
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        Debug.Log("Jump");
    }

    private void ResetJump()
    {
        canJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
