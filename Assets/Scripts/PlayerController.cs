using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // Player movement speed
    public float maxSlope = 45f; // Maximum climbable slope angle in degrees
    public float raycastDistance = 1.1f; // Distance to check for ground
    public float snapDistance = 2f; // Distance to the tree to trigger snapping
    public float chopDuration = 4f; // Duration for which chopping animation plays

    private Vector2 move; // Store movement input
    private Animator animator; // Animator reference
    private Rigidbody rb; // Rigidbody reference

    private Vector3 moveDirection; // Final direction of movement
    private TreeController nearbyTree; // Reference to the nearby tree for interaction
    private bool isChopping = false; // Track whether the player is chopping
    private float chopTimer = 0f; // Timer to control animation duration

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>(); // Get Animator component
        rb = GetComponent<Rigidbody>(); // Get Rigidbody component

        // Freeze all rotation to prevent spinning issues
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

    // Handle chop action input (spacebar or mapped key)
    public void OnChop(InputAction.CallbackContext context)
    {
        if (nearbyTree != null) // Ensure the nearby tree is not null
        {
            if (context.performed && !isChopping) // Button pressed and not already chopping
            {
                StartChopping();
            }
            else if (context.performed && isChopping) // Button pressed and already chopping
            {
                CancelChopping();
            }
        }
        else
        {
            Debug.LogWarning("No nearby tree to chop.");
        }
    }

    // Start the chopping process
    private void StartChopping()
    {
        isChopping = true; // Start chopping
        chopTimer = chopDuration; // Set the chop duration
        animator.SetBool("isChopping", true); // Start chopping animation
        nearbyTree.StartChopping(); // Start tree shake effect
    }

    // Cancel the chopping process
    private void CancelChopping()
    {
        isChopping = false; // Stop chopping
        animator.SetBool("isChopping", false); // Stop chopping animation
        nearbyTree.StopChopping(); // Stop tree shake effect
        chopTimer = 0f; // Reset chop timer
    }

    // Update is called once per frame (for animations only)
    void Update()
    {
        UpdateAnimations(); // Handle walking animations

        if (isChopping)
        {
            chopTimer -= Time.deltaTime; // Decrease the timer
            if (chopTimer <= 0f)
            {
                CancelChopping(); // Stop chopping when time is up
            }

            SnapToTree(); // Continuously check for snapping to tree during chopping
        }
    }

    // FixedUpdate for physics-based movement
    void FixedUpdate()
    {
        if (!isChopping) // Only handle movement if not chopping
        {
            MovePlayer(); // Handle player physics movement
        }
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
                    moveDirection = inputDirection * speed; // Apply movement
                }
                else
                {
                    moveDirection = Vector3.zero; // Too steep, prevent moving
                }
            }
            else
            {
                moveDirection = Vector3.zero; // No ground detected (airborne)
            }

            // Smoothly rotate player toward movement direction
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.15f);
        }
        else
        {
            moveDirection = Vector3.zero; // No input = no movement
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
        animator.SetBool("IsWalking", isWalking); // Update walking animation state
    }

    // Check for tree interaction and store nearby tree reference
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tree")) // Tree tagged as "Tree"
        {
            nearbyTree = other.GetComponent<TreeController>(); // Cache tree reference
            Debug.Log("Tree detected");
            nearbyTree.StartHighlighting(); // Start highlighting the tree
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            if (nearbyTree != null && other.gameObject == nearbyTree.gameObject)
            {
                nearbyTree.StopHighlighting(); // Stop highlighting when leaving the tree area
                nearbyTree.StopChopping(); // Stop shaking when leaving the tree area
                nearbyTree = null; // Remove reference when leaving tree trigger
                Debug.Log("Left tree area");
            }
        }
    }

    // Snap the player to the tree's direction only when close and pressing space
    private void SnapToTree()
    {
        if (nearbyTree != null)
        {
            // Check if the player is within snap distance of the tree
            float distanceToTree = Vector3.Distance(transform.position, nearbyTree.transform.position);
            
            if (distanceToTree <= snapDistance)
            {
                // If within range, always snap to the tree direction
                Vector3 directionToTree = (nearbyTree.transform.position - transform.position).normalized; // Direction to tree
                Quaternion targetRotation = Quaternion.LookRotation(directionToTree); // Target rotation to face the tree

                // Smoothly rotate player to the tree's direction
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
            }
        }
    }
}
