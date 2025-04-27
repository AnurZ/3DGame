using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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

    public enum InjuryStatus { Healthy, Minor, Moderate, Severe }
    public InjuryStatus currentInjury = InjuryStatus.Healthy;
    private float injuryEffectMultiplier = 1f;

    public int daysToRecover = 0;
    public Text injuryStateText;
    public static PlayerController Local;
    public GameObject modelToHide;

    public GameObject playerInjuryAlert;
    public Text injuryAlertText;
    
    private float injuryRisk = 0f;
    private string injuryRiskLevel = "LOW";

    // Duration multiplier for chop duration
    private float chopDurationMultiplier = 1f;

    private InjurySystem.InjuryChanceData chopInjuryChance;
    private bool injuryCalculatedAtStart = false;

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
        dayNightCycle = FindObjectOfType<SimpleDayNightCycle>();

        if (injuryStateText != null)
            injuryStateText.gameObject.SetActive(true); // Ensure it's visible

        uiPanel.SetActive(false);

        // Set the player to be healthy at the start
        currentInjury = InjuryStatus.Healthy;
        
        playerInjuryAlert.SetActive(false);
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
            {
                // Calculate and log injury chances at the start of chopping
                InjurySystem.InjuryChanceData injuryChanceData = InjurySystem.CalculateChance(currentInjury, StaminaController.Instance.playerStamina);

                // Calculate the total injury chance (minor + moderate + severe)
                float totalInjuryChance = injuryChanceData.minorChance + injuryChanceData.moderateChance + injuryChanceData.severeChance;
                Debug.Log($"Total injury chance at chop start: {totalInjuryChance * 100}%");

                // Log individual injury chances for reference
                Debug.Log($"Injury chance at chop start: Minor {injuryChanceData.minorChance * 100}% | Moderate {injuryChanceData.moderateChance * 100}% | Severe {injuryChanceData.severeChance * 100}%");
            
                StartCoroutine(StartChopNextFrame());
            }
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
        UpdateInjuryRisk();
        StartChopping(injuryRisk);
    }

    private void StartChopping(float risk)
    {
        if (nearbyTree == null) return;
        if (currentInjury == InjuryStatus.Severe)
        {
            Debug.Log("Cannot chop while severely injured!");
            return;
        }

        // Calculate injury chance ONCE at start
        if (!injuryCalculatedAtStart)
        {
            chopInjuryChance = InjurySystem.CalculateChance(currentInjury, StaminaController.Instance.playerStamina);
            injuryCalculatedAtStart = true;
            Debug.Log($"Injury chance set at chop start: Minor {chopInjuryChance.minorChance * 100}% | Moderate {chopInjuryChance.moderateChance * 100}% | Severe {chopInjuryChance.severeChance * 100}%");
        }

        // Adjust chop duration based on injury state
        float baseChopTime = 10f;
        float adjustedChopTime = baseChopTime * chopDurationMultiplier;

        isChopping = true;
        animator.SetBool("isChopping", true);
        animator.SetBool("IsWalking", false);

        rb.linearVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;

        nearbyTree.StartChopping(risk, adjustedChopTime); // Pass adjusted chop time

        if (interactionText != null)
        {
            interactionText.text = "Press Space to Stop Chopping";
            interactionText.color = Color.red;
        }
    }

    public void OnTreeChoppedDown()
    {
        // Player finished chopping, now roll for injury
        if (injuryCalculatedAtStart)
        {
            TryApplyInjury();  // Injury check after chopping
            injuryCalculatedAtStart = false;  // Reset the flag
        }
    }

    private void TryApplyInjury()
    {
        float roll = Random.value; // 0.0 to 1.0
        Debug.Log($"Injury Roll: {roll}");

        // Generate random recovery time based on injury severity
        int recoveryDays = 0;

        if (roll <= chopInjuryChance.severeChance)
        {
            currentInjury = InjuryStatus.Severe;
            recoveryDays = Random.Range(6, 11); // 6-10 days for severe injury
            Debug.Log($"You suffered a SEVERE injury! Recovery time: {recoveryDays} days.");
        }
        else if (roll <= chopInjuryChance.severeChance + chopInjuryChance.moderateChance)
        {
            currentInjury = InjuryStatus.Moderate;
            recoveryDays = Random.Range(3, 6); // 3-5 days for moderate injury
            Debug.Log($"You suffered a MODERATE injury! Recovery time: {recoveryDays} days.");
        }
        else if (roll <= chopInjuryChance.severeChance + chopInjuryChance.moderateChance + chopInjuryChance.minorChance)
        {
            currentInjury = InjuryStatus.Minor;
            recoveryDays = Random.Range(1, 3); // 1-2 days for minor injury
            Debug.Log($"You suffered a MINOR injury! Recovery time: {recoveryDays} days.");
        }
        else
        {
            currentInjury = InjuryStatus.Healthy;
            Debug.Log("No injury occurred.");
        
            // Hide the injury alert if no injury occurred
            if (playerInjuryAlert != null)
            {
                playerInjuryAlert.SetActive(false);
            }
            return; // No injury to process
        }
        daysToRecover = recoveryDays;
        // Show the injury state and recovery text after the injury has been decided
        SetInjuryStateText(recoveryDays);

        // Make sure the player injury alert is visible when an injury occurs
        if (playerInjuryAlert != null)
        {
            playerInjuryAlert.SetActive(true);
        }
    }



    private void SetInjuryStateText(int recoveryDays)
    {
        if (playerInjuryAlert != null)
        {
            if (currentInjury == InjuryStatus.Healthy)
            {
                playerInjuryAlert.SetActive(false); // Hide if healthy
                injuryStateText.text = ""; // Optionally clear the text
                return;
            }
            playerInjuryAlert.SetActive(true); // Show the alert if injured

            // Set the injury text based on current injury status
            string injuryText = $"You are injured!\n";

            if (currentInjury == InjuryStatus.Minor)
            {
                injuryText += $"Chopping is 25% slower.\n Recovery time: {recoveryDays} days.";
                injuryAlertText.color = Color.yellow; // Minor - Yellow
            }
            else if (currentInjury == InjuryStatus.Moderate)
            {
                injuryText += $"Chopping is 50% slower.\n Recovery time: {recoveryDays} days.";
                injuryAlertText.color = new Color(1f, 0.647f, 0f); // Moderate - Orange
            }
            else if (currentInjury == InjuryStatus.Severe)
            {
                injuryText += $"You cannot chop trees\n while severely injured.\n Recovery time: {recoveryDays} days.";
                injuryAlertText.color = new Color(0.75f, 0f, 0f); // Severe - Dark Red
            }
            else
            {
                injuryAlertText.text = ""; // Healthy
                return;
            }

            injuryAlertText.text = injuryText;
        }
    }


    private void CancelChopping()
    {
        if (!isChopping) return;

        isChopping = false;
        animator.SetBool("isChopping", false);
        animator.SetBool("IsWalking", false);
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        nearbyTree?.StopChopping();

        if (interactionText != null)
        {
            interactionText.text = "Press Space to Chop";
            interactionText.color = Color.white;
        }
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

        // Update injury state text every frame
        UpdateInjuryRisk();

        // Get recovery days based on current injury
        int recoveryDays = daysToRecover;

        // Update injury state text with recovery days
        UpdateInjuryStateText();
    }

    private void UpdateInjuryStateText()
    {
        if (injuryStateText != null)
        {
            string injuryText = "Injury state: ";

            int recoveryDays = daysToRecover;

            // Set injury text and color based on injury state
            switch (currentInjury)
            {
                case InjuryStatus.Minor:
                    injuryText += $"Minor injury ({recoveryDays} days left)";
                    injuryStateText.color = Color.yellow; // Minor - Yellow
                    break;
                case InjuryStatus.Moderate:
                    injuryText += $"Moderate injury ({recoveryDays} days left)";
                    injuryStateText.color = new Color(1f, 0.647f, 0f); // Moderate - Orange
                    break;
                case InjuryStatus.Severe:
                    injuryText += $"Severe injury ({recoveryDays} days left)";
                    injuryStateText.color = new Color(0.75f, 0f, 0f); // Severe - Dark Red
                    break;
                default:
                    injuryText = "";
                    break;
            }

            injuryStateText.text = injuryText;
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
            if (nearbyTree != other.GetComponent<TreeController>()) // Ensure we're not already highlighting this tree
            {
                if (nearbyTree != null)
                {
                    nearbyTree.StopHighlighting(); // Stop highlighting previous tree
                }

                nearbyTree = other.GetComponent<TreeController>();
                nearbyTree?.StartHighlighting(); // Highlight the new tree

                if (interactionText != null)
                {
                    if (currentInjury == InjuryStatus.Severe)
                    {
                        interactionText.text = "Cannot chop while severely injured!";
                        interactionText.color = Color.red;
                    }
                    else
                    {
                        interactionText.text = isChopping ? "Press Space to Stop Chopping" : "Press Space to Chop";
                        interactionText.color = isChopping ? Color.red : Color.white;
                    }
                    uiPanel.SetActive(true);
                }
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
        if (other.CompareTag("Tree"))
        {
            if (nearbyTree != null && nearbyTree.CompareTag("Tree"))
            {
                nearbyTree.StopHighlighting(); // Stop highlighting the tree when leaving its collider
                nearbyTree = null;
            }

            // Optional: Reset interaction text if the player exits a tree's trigger zone
            if (interactionText != null)
            {
                interactionText.text = "";
            }

            if (uiPanel.activeSelf)
                uiPanel.SetActive(false);
        }
        else if (other.CompareTag("Bed"))
        {
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

    private void UpdateInjuryRisk()
    {
        float stamina = StaminaController.Instance.playerStamina;

        // Update chop duration multiplier based on injury status
        if (currentInjury == InjuryStatus.Healthy)
        {
            chopDurationMultiplier = 1f; // Normal chop speed
        }
        else if (currentInjury == InjuryStatus.Minor)
        {
            chopDurationMultiplier = 1.25f; // 25% slower
        }
        else if (currentInjury == InjuryStatus.Moderate)
        {
            chopDurationMultiplier = 1.5f; // 50% slower
        }
        else if (currentInjury == InjuryStatus.Severe)
        {
            chopDurationMultiplier = 0f; // Prevent chopping (already handled in StartChopping)
        }

        if (currentInjury == InjuryStatus.Healthy)
        {
            if (stamina <= 30) { injuryRisk = 35; injuryRiskLevel = "HIGH"; }
            else if (stamina <= 40) { injuryRisk = 25; injuryRiskLevel = "MEDIUM"; }
            else if (stamina <= 50) { injuryRisk = 15; injuryRiskLevel = "LOW"; }
            else { injuryRisk = 0; injuryRiskLevel = "LOW"; }
        }
        else if (currentInjury == InjuryStatus.Minor)
        {
            if (stamina <= 30) { injuryRisk = 50; injuryRiskLevel = "VERY HIGH"; }
            else if (stamina <= 40) { injuryRisk = 40; injuryRiskLevel = "HIGH"; }
            else if (stamina <= 50) { injuryRisk = 30; injuryRiskLevel = "MEDIUM"; }
            else { injuryRisk = 15; injuryRiskLevel = "LOW"; }
        }
    }
}
