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

    public GameObject gameplayUI;
    public GameObject moneyUI;

    public GameObject uiPanel;
    public TMP_Text interactionText;

    public bool isChopping = false;
    public bool isInShop = false;

    private InventoryManager inventoryManager;

    public enum InjuryStatus { Healthy, Minor, Moderate, Severe }
    public InjuryStatus currentInjury = InjuryStatus.Healthy;
    private float injuryEffectMultiplier = 1f;

    public Text injuryStateText;
    

    
    public int daysToRecover = 0;
    public static PlayerController Local;
    public GameObject modelToHide;

    public GameObject playerInjuryAlert;
    public Text injuryAlertText;

    private float injuryRisk = 0f;
    private string injuryRiskLevel = "LOW";

    private float chopDurationMultiplier = 1f;
    private InjurySystem.InjuryChanceData chopInjuryChance;
    private bool injuryCalculatedAtStart = false;

    private void Awake()
    {
        Local = this;
    }

    public void EnterShop()
    {
        isInShop = true;
        gameplayUI.SetActive(false);
        uiPanel.SetActive(false);
        DialogueManager.Instance.StartDialogue();
        
        Debug.Log("Entered shop.");
    }

    public void ExitShop()
    {
        isInShop = false;
        if (gameplayUI != null) gameplayUI.SetActive(true);
        if (moneyUI != null) moneyUI.SetActive(true);
        DialogueManager.Instance.EndDialogue();

    }
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        inventoryManager = FindObjectOfType<InventoryManager>();
        dayNightCycle = FindObjectOfType<SimpleDayNightCycle>();

        if (injuryStateText != null)
            injuryStateText.gameObject.SetActive(true);

        uiPanel.SetActive(false);

        currentInjury = InjuryStatus.Healthy;
        playerInjuryAlert.SetActive(false);
    }

    public void SetVisible(bool isVisible)
    {
        if (modelToHide != null)
            modelToHide.SetActive(isVisible);

        var rends = GetComponentsInChildren<Renderer>();
        foreach (var r in rends)
            r.enabled = isVisible;
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

        if (!injuryCalculatedAtStart)
        {
            chopInjuryChance = InjurySystem.CalculateChance(currentInjury, StaminaController.Instance.playerStamina);
            injuryCalculatedAtStart = true;
            Debug.Log($"Injury chance at chop start: Minor {chopInjuryChance.minorChance * 100}% | Moderate {chopInjuryChance.moderateChance * 100}% | Severe {chopInjuryChance.severeChance * 100}%");
        }

        float baseChopTime = 10f;
        float adjustedChopTime = baseChopTime * chopDurationMultiplier;

        isChopping = true;
        animator.SetBool("isChopping", true);
        animator.SetBool("IsWalking", false);

        rb.linearVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;

        nearbyTree.StartChopping(risk, adjustedChopTime);

        if (interactionText != null)
        {
            interactionText.text = "Press Space to Stop Chopping";
            interactionText.color = Color.red;
        }
    }

    public void OnTreeChoppedDown()
    {
        if (injuryCalculatedAtStart)
        {
            TryApplyInjury();
            injuryCalculatedAtStart = false;
        }
    }

    private void TryApplyInjury()
    {
        float roll = Random.value;
        Debug.Log($"Injury Roll: {roll}");

        int recoveryDays = 0;

        if (roll <= chopInjuryChance.severeChance)
        {
            currentInjury = InjuryStatus.Severe;
            recoveryDays = Random.Range(6, 11);
            Debug.Log($"You suffered a SEVERE injury! Recovery time: {recoveryDays} days.");
        }
        else if (roll <= chopInjuryChance.severeChance + chopInjuryChance.moderateChance)
        {
            currentInjury = InjuryStatus.Moderate;
            recoveryDays = Random.Range(3, 6);
            Debug.Log($"You suffered a MODERATE injury! Recovery time: {recoveryDays} days.");
        }
        else if (roll <= chopInjuryChance.severeChance + chopInjuryChance.moderateChance + chopInjuryChance.minorChance)
        {
            currentInjury = InjuryStatus.Minor;
            recoveryDays = Random.Range(1, 3);
            Debug.Log($"You suffered a MINOR injury! Recovery time: {recoveryDays} days.");
        }
        else
        {
            currentInjury = InjuryStatus.Healthy;
            Debug.Log("No injury occurred.");
            if (playerInjuryAlert != null)
                playerInjuryAlert.SetActive(false);
            return;
        }

        daysToRecover = recoveryDays;
        SetInjuryStateText(recoveryDays);

        if (playerInjuryAlert != null)
            playerInjuryAlert.SetActive(true);
    }

    private void SetInjuryStateText(int recoveryDays)
    {
        if (playerInjuryAlert != null)
        {
            if (currentInjury == InjuryStatus.Healthy)
            {
                playerInjuryAlert.SetActive(false);
                injuryStateText.text = "";
                return;
            }

            playerInjuryAlert.SetActive(true);
            string injuryText = "You are injured!\n";

            if (currentInjury == InjuryStatus.Minor)
            {
                injuryText += $"Chopping is 25% slower.\n Recovery time: {recoveryDays} days.";
                injuryAlertText.color = Color.yellow;
            }
            else if (currentInjury == InjuryStatus.Moderate)
            {
                injuryText += $"Chopping is 50% slower.\n Recovery time: {recoveryDays} days.";
                injuryAlertText.color = new Color(1f, 0.647f, 0f);
            }
            else if (currentInjury == InjuryStatus.Severe)
            {
                injuryText += $"You cannot chop trees\n while severely injured.\n Recovery time: {recoveryDays} days.";
                injuryAlertText.color = new Color(0.75f, 0f, 0f);
            }
            else
            {
                injuryAlertText.text = "";
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

        if (nearbyTree != null)
            nearbyTree.StopChopping();

        if (interactionText != null)
        {
            interactionText.text = "Press Space to Chop";
            interactionText.color = Color.white;
        }
    }

    private void Update()
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
        
       
        
        if (!isChopping && !isInShop && canSleep && Input.GetKeyDown(KeyCode.Space))
        {
            dayNightCycle.Sleep();
            uiPanel.SetActive(false);
            interactionText.text = "";
        }
        UpdateInjuryRisk();
        //UpdateInjuryStateText();
    }

    private void FixedUpdate()
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            if (nearbyTree != other.GetComponent<TreeController>())
            {
                if (nearbyTree != null)
                    nearbyTree.StopHighlighting();

                nearbyTree = other.GetComponent<TreeController>();
                nearbyTree?.StartHighlighting();

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
            if (interactionText != null)
            {
                interactionText.text = "Press [Space] to enter shop.";
                uiPanel.SetActive(true);
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            if (nearbyTree != null)
            {
                nearbyTree.StopHighlighting();
                nearbyTree = null;
            }

            if (interactionText != null)
                interactionText.text = "";

            if (uiPanel.activeSelf)
                uiPanel.SetActive(false);
        }
        else if (other.CompareTag("Bed"))
        {
            canSleep = false;
        }
        else if (other.CompareTag("Shop"))
        {
            if (uiPanel.activeSelf)
                uiPanel.SetActive(false);
            if (isInShop)
            {
                isInShop = false;
                gameplayUI.SetActive(true);
                Debug.Log("Exited shop.");
            }
            
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
        // Logika za update injury rizika (pretpostavljam da dolazi iz InjurySystem-a)
        InjurySystem.InjuryChanceData injuryChanceData = InjurySystem.CalculateChance(currentInjury, StaminaController.Instance.playerStamina);

        float totalInjuryChance = injuryChanceData.minorChance + injuryChanceData.moderateChance + injuryChanceData.severeChance;
        injuryRisk = totalInjuryChance;
    }
}
