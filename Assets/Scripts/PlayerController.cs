using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // Player movement speed
    public float maxSlope = 45f; // Maximum climbable slope angle in degrees
    public float raycastDistance = 1.1f; // Distance to check for ground

    private Vector2 move; // Store movement input
    private Animator animator; // Animator reference
    private Rigidbody rb; // Rigidbody reference

    private Vector3 moveDirection; // Final direction of movement

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>(); // Get Animator component
        rb = GetComponent<Rigidbody>(); // Get Rigidbody component

        // **Freeze all rotation** to prevent any spinning issues
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Optional: Smooth collision handling
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    // Handle player movement input
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>(); // Get move input
    }

    // Handle chop action input
    public void OnChop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Chop action triggered!");
            animator.SetTrigger("Chop");
            animator.SetBool("IsChopping", true);
        }
        else if (context.canceled)
        {
            animator.SetBool("IsChopping", false);
        }
    }

    // Update is called once per frame (for animations only)
    void Update()
    {
        UpdateAnimations(); // Handle walking/chopping animations
    }

    // FixedUpdate for physics-based movement
    void FixedUpdate()
    {
        MovePlayer(); // Handle player physics movement
    }

    // Player movement with slope limit and ground check
    private void MovePlayer()
    {
        Vector3 inputDirection = new Vector3(move.x, 0f, move.y).normalized;

        if (inputDirection != Vector3.zero)
        {
            // Raycast to detect ground and get normal
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, raycastDistance))
            {
                Vector3 groundNormal = hit.normal;
                float slopeAngle = Vector3.Angle(groundNormal, Vector3.up);

                // Check if slope is acceptable
                if (slopeAngle <= maxSlope)
                {
                    // Apply movement
                    moveDirection = inputDirection * speed;
                }
                else
                {
                    // Too steep, prevent moving
                    moveDirection = Vector3.zero;
                }
            }
            else
            {
                // No ground detected (airborne)
                moveDirection = Vector3.zero;
            }

            // Smoothly rotate player toward movement direction
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.15f);
        }
        else
        {
            // No input = no movement
            moveDirection = Vector3.zero;
        }

        // Check if grounded before applying movement
        bool isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, raycastDistance);

        if (isGrounded)
        {
            // Keep Y velocity from gravity but move on X and Z
            Vector3 velocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
            rb.linearVelocity = velocity;
        }
        else
        {
            // Falling, keep Y velocity only
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    // Handle animation state updates
    private void UpdateAnimations()
    {
        bool isWalking = move != Vector2.zero;
        animator.SetBool("IsWalking", isWalking);
    }
}
