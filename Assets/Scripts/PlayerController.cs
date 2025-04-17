using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    private Vector2 move;
    private Animator animator;
    private Rigidbody rb;
    private TreeController nearbyTree;
    public bool isChopping = false;
    private float chopTimer = 0f;
    private InventoryManager inventoryManager;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isChopping)
        {
            move = context.ReadValue<Vector2>();
        }
        else
        {
            move = Vector2.zero;
        }
    }

    public void OnChop(InputAction.CallbackContext context)
    {
        if (context.performed && nearbyTree != null && IsHoldingAxe())
        {
            if (isChopping)
            {
                CancelChopping();
            }
            else
            {
                StartCoroutine(StartChopNextFrame());
            }
        }
    }
    
    private bool IsHoldingAxe()
    {
        if (inventoryManager == null) return false;

        Item selectedItem = inventoryManager.GetSelectedItem(false); // false = don't consume
        if (selectedItem != null && selectedItem.isAxe)
        {
            return true;
        }

        return false;
    }

    private IEnumerator StartChopNextFrame()
    {
        yield return new WaitForFixedUpdate(); // Allow physics to complete first
        FaceTreeInstantly();
        StartChopping();
    }

    private void StartChopping()
    {
        if (nearbyTree == null) return;

        chopTimer = nearbyTree.GetChopDuration(); // Get time from tree!
        isChopping = true;

        animator.SetBool("isChopping", true);
        animator.SetBool("IsWalking", false);

        rb.linearVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;

        nearbyTree.StartChopping();
    }

    private void CancelChopping()
    {
        if (!isChopping) return;

        isChopping = false;
        chopTimer = 0f;

        animator.SetBool("isChopping", false);
        animator.SetBool("IsWalking", false);
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        nearbyTree?.StopChopping();
    }

    private void FinishChopping()
    {
        isChopping = false;
        chopTimer = 0f;

        animator.SetBool("isChopping", false);
        animator.SetBool("IsWalking", false);
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        nearbyTree?.FinishChopping();
    }

    void Update()
    {
        if (isChopping)
        {
            chopTimer -= Time.deltaTime;
            
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            
            if (!IsPlayerFacingTree())
            {
                CancelChopping();
            }
            else if (chopTimer <= 0f)
            {
                FinishChopping();
            }
        }

        UpdateAnimations();
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
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.15f);
        }

        rb.linearVelocity = inputDirection * speed;
    }

    private void UpdateAnimations()
    {
        animator.SetBool("IsWalking", !isChopping && move != Vector2.zero);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            nearbyTree = other.GetComponent<TreeController>();
            nearbyTree?.StartHighlighting();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tree") && nearbyTree != null && other.gameObject == nearbyTree.gameObject)
        {
            nearbyTree.StopHighlighting();
            nearbyTree.StopChopping(); // Don't destroy on exit
            nearbyTree = null;
        }
    }

    private void FaceTreeInstantly()
    {
        if (nearbyTree != null)
        {
            Vector3 direction = (nearbyTree.transform.position - transform.position);
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(direction.normalized);
            }
        }
    }

    private bool IsPlayerFacingTree()
    {
        if (nearbyTree != null)
        {
            Vector3 directionToTree = (nearbyTree.transform.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, directionToTree);
            return dotProduct > 0.7f;
        }
        return false;
    }
}
