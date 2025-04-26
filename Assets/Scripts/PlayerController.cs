using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    private Vector2 move;
    private Animator animator;
    private Rigidbody rb;
    private TreeController nearbyTree;
    private bool canSleep = false;
    private SimpleDayNightCycle dayNightCycle;

    public GameObject uiPanel;
    public TMP_Text interactionText;

    public bool isChopping = false;
    public bool isInShop = false;

    private InventoryManager inventoryManager;

    // Injury system
    public enum InjuryStatus { Healthy, Minor, Moderate, Severe }
    public InjuryStatus currentInjury = InjuryStatus.Healthy;
    private float injuryEffectMultiplier = 1f;

    public Text injuryStateText;

    public static PlayerController Local;
    public GameObject modelToHide;

    public void SetVisible(bool isVisible)
    {
        if (modelToHide != null)
            modelToHide.SetActive(isVisible);
    }

    void Awake()
    {
        Local = this;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        inventoryManager = FindObjectOfType<InventoryManager>();

        if (injuryStateText != null)
            injuryStateText.gameObject.SetActive(false);

        StartCoroutine(CycleInjuryStates());
        dayNightCycle = FindObjectOfType<SimpleDayNightCycle>();

        uiPanel.SetActive(false);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = isChopping ? Vector2.zero : context.ReadValue<Vector2>();
    }

    public void OnChop(InputAction.CallbackContext context)
    {
        if (context.performed && nearbyTree != null && IsHoldingAxe())
        {
            if (isChopping)
                CancelChopping();
            else
                StartCoroutine(StartChopNextFrame());
        }
    }

    private bool IsHoldingAxe()
    {
        if (inventoryManager == null) return false;

        Item selectedItem = inventoryManager.GetSelectedItem(false);
        return selectedItem != null && selectedItem.isAxe;
    }

    private IEnumerator StartChopNextFrame()
    {
        yield return new WaitForFixedUpdate();
        FaceTreeInstantly();
        StartChopping();
    }

    private void StartChopping()
    {
        if (nearbyTree == null) return;

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

        animator.SetBool("isChopping", false);
        animator.SetBool("IsWalking", false);
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        nearbyTree?.StopChopping();
    }

    void Update()
    {
        if (isInShop)
            return;

        if (isChopping)
        {
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            if (!IsPlayerFacingTree())
                CancelChopping();
        }
        else
        {
            UpdateAnimations();
        }
    }

    void FixedUpdate()
    {
        if (isInShop || isChopping)
            return;

        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 inputDirection = new Vector3(move.x, 0f, move.y).normalized * injuryEffectMultiplier;

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

            if (interactionText != null)
            {
                interactionText.text = "Press Spacebar to Chop";
                uiPanel.SetActive(true);
            }
        }
        else if (other.CompareTag("Bed"))
        {
            canSleep = true;

            if (interactionText != null)
            {
                interactionText.text = "Press [Spacebar] to sleep.";
                uiPanel.SetActive(true);
            }
        }
        else if (other.CompareTag("Shop") && !isInShop)
        {
            interactionText.text = "Press [Space] to enter shop.";
            uiPanel.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tree") || other.CompareTag("Bed") || other.CompareTag("Shop"))
        {
            if (uiPanel.activeSelf)
                uiPanel.SetActive(false);

            if (interactionText != null)
                interactionText.text = "";

            if (other.CompareTag("Bed"))
                canSleep = false;
        }
    }

    private void FaceTreeInstantly()
    {
        if (nearbyTree != null)
        {
            Vector3 direction = (nearbyTree.transform.position - transform.position);
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(direction.normalized);
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

    public void ApplyInjury(InjuryStatus injury)
    {
        currentInjury = injury;

        switch (currentInjury)
        {
            case InjuryStatus.Minor:
                injuryEffectMultiplier = 0.9f;
                UpdateInjuryUI("Minor Injury", Color.green);
                break;
            case InjuryStatus.Moderate:
                injuryEffectMultiplier = 0.7f;
                UpdateInjuryUI("Moderate Injury", Color.yellow);
                break;
            case InjuryStatus.Severe:
                injuryEffectMultiplier = 0.5f;
                UpdateInjuryUI("Severe Injury", Color.red);
                break;
            case InjuryStatus.Healthy:
                injuryEffectMultiplier = 1f;
                if (injuryStateText != null)
                    injuryStateText.gameObject.SetActive(false);
                break;
        }
    }

    private void UpdateInjuryUI(string injuryMessage, Color injuryColor)
    {
        if (injuryStateText != null)
        {
            injuryStateText.gameObject.SetActive(true);
            injuryStateText.text = injuryMessage;
            injuryStateText.color = injuryColor;
        }
    }

    private IEnumerator CycleInjuryStates()
    {
        while (true)
        {
            ApplyInjury(InjuryStatus.Healthy);
            yield return new WaitForSeconds(5f);
            ApplyInjury(InjuryStatus.Minor);
            yield return new WaitForSeconds(5f);
            ApplyInjury(InjuryStatus.Moderate);
            yield return new WaitForSeconds(5f);
            ApplyInjury(InjuryStatus.Severe);
            yield return new WaitForSeconds(5f);
        }
    }
}
