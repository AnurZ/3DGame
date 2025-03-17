using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float maxSlope = 45f;
    public float raycastDistance = 1.1f;
    public float snapDistance = 2f;
    public float chopDuration = 4f;

    private Vector2 move;
    private Animator animator;
    private Rigidbody rb;

    private Vector3 moveDirection;
    private TreeController nearbyTree;
    private bool isChopping = false;
    private float chopTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnChop(InputAction.CallbackContext context)
    {
        if (nearbyTree != null && context.performed && !isChopping)
        {
            StartChopping();
        }
        else if (context.performed && isChopping)
        {
            CancelChopping();
        }
    }

    private void StartChopping()
    {
        isChopping = true;
        chopTimer = chopDuration;
        animator.SetBool("isChopping", true);
        nearbyTree.StartChopping();
    }

    private void CancelChopping()
    {
        isChopping = false;
        animator.SetBool("isChopping", false);
        nearbyTree.StopChopping();
        chopTimer = 0f;
    }

    void Update()
    {
        UpdateAnimations();

        if (isChopping)
        {
            chopTimer -= Time.deltaTime;
            if (chopTimer <= 0f)
            {
                CancelChopping();
            }
            SnapToTree();
        }
    }

    void FixedUpdate()
    {
        if (!isChopping)
        {
            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        Vector3 inputDirection = new Vector3(move.x, 0f, move.y).normalized;

        if (inputDirection != Vector3.zero)
        {
            // Calculate the next position
            Vector3 nextPosition = transform.position + inputDirection * speed * Time.deltaTime;

            // Raycast to check if the next position is valid
            RaycastHit hit;
            if (Physics.Raycast(nextPosition + Vector3.up * 0.1f, Vector3.down, out hit, raycastDistance))
            {
                // Check if the slope is walkable
                if (Vector3.Angle(hit.normal, Vector3.up) <= maxSlope)
                {
                    // Check if the next position's Y is greater than or equal to 0
                    if (hit.point.y >= 0f)  // Prevent movement into any ground with y < 0
                    {
                        // Allow movement to the next position
                        moveDirection = inputDirection * speed;
                    }
                    else
                    {
                        // Prevent movement if the ground is below 0
                        moveDirection = Vector3.zero;
                    }
                }
                else
                {
                    // Prevent movement if the slope is too steep
                    moveDirection = Vector3.zero;
                }
            }
            else
            {
                // No ground detected below, prevent movement
                moveDirection = Vector3.zero;
            }

            // Apply rotation towards the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.15f);
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        // Grounded check
        bool isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, raycastDistance);

        if (isGrounded)
        {
            // Apply movement if grounded
            Vector3 velocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
            rb.linearVelocity = velocity;
        }
        else
        {
            // Prevent downward movement if not grounded
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    private void UpdateAnimations()
    {
        animator.SetBool("IsWalking", move != Vector2.zero);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            nearbyTree = other.GetComponent<TreeController>();
            nearbyTree.StartHighlighting();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tree") && nearbyTree != null && other.gameObject == nearbyTree.gameObject)
        {
            nearbyTree.StopHighlighting();
            nearbyTree.StopChopping();
            nearbyTree = null;
        }
    }

    private void SnapToTree()
    {
        if (nearbyTree != null)
        {
            float distanceToTree = Vector3.Distance(transform.position, nearbyTree.transform.position);
            if (distanceToTree <= snapDistance)
            {
                Vector3 directionToTree = (nearbyTree.transform.position - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(directionToTree);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);
            }
        }
    }
}
