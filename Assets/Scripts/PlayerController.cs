using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private AudioClip grassWalkClip;
    [SerializeField] private AudioClip woodWalkClip;
    [SerializeField] private AudioClip choppingClip;
    [SerializeField] private AudioClip InjuryClip;
    private bool isOnBridge = false;

    
    private AudioSource walkingSource;
    private AudioSource choppingSource;
    
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
    public enum InjuryStatus { Healthy = 0, Minor, Moderate, Severe }
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
    
    public PotionManager potionManager;
    
    
    public GameObject ChoppingGameObject;
    public Image choppingImage;

    public StaminaController staminaController;

    public bool choppingSpeedUpgrade = false;
    public bool severeInjuryShieldUpgrade = false;
    public bool injuryShieldUpgrade = false;
    
    public TransparencyController transparencyManager;
    
    public AchievementsController achievementsController;

    public bool isUpgradePotionActive = false;
    public bool isStaminaRegenUpgrade = false;
    private float passiveStaminaTimer = 0f;

    public TreeSpawner treeSpawner;
    public Image SkillCheckArea;
    public TMP_Text TimeLeftText;
    
    
    private void Awake()
    {
        Local = this;
        
    }
    
    public void OnDayPassed()
    {
        if (currentInjury != InjuryStatus.Healthy)
        {
            daysToRecover--;
//             Debug.Log("daysToRecover" + daysToRecover);
            if (daysToRecover <= 0)
            {
                currentInjury = InjuryStatus.Healthy;
                achievementsController.HealFromInjury = 1;
                injuryEffectMultiplier = 1;
            }
            
            UpdateInjuryStateText();
        }
    }

    public void EnterShop()
    {
        isInShop = true;
        gameplayUI.SetActive(false);
        uiPanel.SetActive(false);
        DialogueManager.Instance.StartDialogue();
        
        if (walkingSource.isPlaying)
            walkingSource.Stop();

        
//         Debug.Log("Entered shop.");
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
        
        var sources = GetComponents<AudioSource>();
        walkingSource  = sources[0];
        choppingSource = sources.Length > 1 ? sources[1] : walkingSource;
        
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        inventoryManager = FindObjectOfType<InventoryManager>();
        dayNightCycle = FindObjectOfType<SimpleDayNightCycle>();
        potionManager = FindObjectOfType<PotionManager>();
        staminaController = FindObjectOfType<StaminaController>();
        transparencyManager = FindObjectOfType<TransparencyController>();
        achievementsController = FindObjectOfType<AchievementsController>();
        treeSpawner = FindObjectOfType<TreeSpawner>();
        treeSpawner.LoadTrees();
        
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

    private IEnumerator ChopSoundLoop()
    {
        while (isChopping)
        {
            if (choppingSource != null && choppingClip != null)
                choppingSource.PlayOneShot(choppingClip);
            yield return new WaitForSeconds(chopSoundInterval);
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

    [SerializeField] private float chopSoundInterval = 0.5f; // koliko sekundi čekaš između udaraca
    //private Coroutine chopSfxCoroutine;
    //[SerializeField] private float chopSoundDelay = 0f;
    private void StartChopping(float risk)
    {
        if (nearbyTree == null) return;
        if (currentInjury == InjuryStatus.Severe)
        {
//             Debug.Log("Cannot chop while severely injured!");
            return;
        }
        
        Item selectedItem = inventoryManager.GetSelectedItem(false);
        if (!IsHoldingAxe() || (int)nearbyTree.treeType > (int)selectedItem.currentAxeType + (potionManager.UpgradePotionHours > 0 ? 1 : 0))
        {
            interactionText.text = "You need a higher level axe to chop this tree!";
            interactionText.color = Color.red;
            return;
        }
        
       


        
        
        chopInjuryChance = InjurySystem.CalculateChance(currentInjury, StaminaController.Instance.playerStamina);
        if (potionManager.ShieldPotionHours > 0)
        {
            chopInjuryChance.minorChance *= 0.5f;
            chopInjuryChance.moderateChance *= 0.5f;
            chopInjuryChance.severeChance *= 0.5f;
        }
        if (severeInjuryShieldUpgrade)
        {
            chopInjuryChance.minorChance += chopInjuryChance.severeChance * 0.5f;
            chopInjuryChance.moderateChance += chopInjuryChance.severeChance * 0.5f;
            chopInjuryChance.severeChance *= 0.0f;
        }

        if (injuryShieldUpgrade)
        {
            chopInjuryChance.minorChance *= 0.9f;
            chopInjuryChance.moderateChance *= 0.9f;
            chopInjuryChance.severeChance *= 0.9f;
        }
//         Debug.Log($"Injury chance at chop start: Minor {chopInjuryChance.minorChance * 100}% | Moderate {chopInjuryChance.moderateChance * 100}% | Severe {chopInjuryChance.severeChance * 100}%");
        

        float baseChopTime = nearbyTree.choppingDuration;
        
        if (currentInjury == InjuryStatus.Minor)
            chopDurationMultiplier = 1.25f;
        else if(currentInjury == InjuryStatus.Moderate)
            chopDurationMultiplier = 1.5f;
        else if (currentInjury == InjuryStatus.Healthy)
            chopDurationMultiplier = 1f;
        
        if(potionManager.FocusPotionHours > 0)
            chopDurationMultiplier *= 0.5f;
        
        if(choppingSpeedUpgrade)
            chopDurationMultiplier *= 0.8f;


        float axeChoppingSpeed = baseChopTime - 0.5f * ((int)selectedItem.currentAxeType - (int)nearbyTree.treeType);
        float adjustedChopTime = axeChoppingSpeed * chopDurationMultiplier;

        if (staminaController.playerStamina < adjustedChopTime)
        {
            interactionText.text = "Cannot chop while low on stamina!";
            interactionText.color = Color.red;
            uiPanel.SetActive(true);
            return;
        }
        
        
        
        isChopping = true;
        animator.SetBool("isChopping", true);
        animator.SetBool("IsWalking", false);

        rb.linearVelocity = Vector3.zero; // Stop all motion
        rb.angularVelocity = Vector3.zero;

// Freeze position and rotation on all axes (adjust if your movement is on X/Z)
        rb.constraints = RigidbodyConstraints.FreezePositionX |
                         RigidbodyConstraints.FreezePositionZ |
                         RigidbodyConstraints.FreezeRotation;


        nearbyTree.StartChopping(risk, adjustedChopTime);
        
       // if (chopSfxCoroutine != null) 
        //    StopCoroutine(chopSfxCoroutine);
       // chopSfxCoroutine = StartCoroutine(ChopSoundLoop());

        if (interactionText != null)
        {
            interactionText.text = "Press Space to Stop Chopping";
            interactionText.color = Color.red;
        }
        
        
    }
    
    public void PlayChopSound()
    {
        if (choppingSource != null && choppingClip != null)
            choppingSource.PlayOneShot(choppingClip);
    }


    public void OnTreeChoppedDown()
    {
//         Debug.Log($"Ukupni chance {injuryRisk} Injury chance at chop start: Minor {chopInjuryChance.minorChance * 100}% | Moderate {chopInjuryChance.moderateChance * 100}% | Severe {chopInjuryChance.severeChance * 100}%");
        TryApplyInjury();
    }

    private void TryApplyInjury()
    {
        float roll = Random.value;
//         Debug.Log($"Injury Roll: {roll}");
        
        int recoveryDays = daysToRecover;

        if (roll <= chopInjuryChance.severeChance)
        {
            currentInjury = InjuryStatus.Severe;
            recoveryDays = Random.Range(6, 11);
        }
        else if (roll <= chopInjuryChance.severeChance + chopInjuryChance.moderateChance && currentInjury != InjuryStatus.Moderate)
        {
            currentInjury = InjuryStatus.Moderate;
            recoveryDays = Random.Range(3, 6);
        }
        else if (roll <= chopInjuryChance.severeChance + chopInjuryChance.moderateChance + chopInjuryChance.minorChance && currentInjury != InjuryStatus.Minor &&  currentInjury != InjuryStatus.Moderate)
        {
            currentInjury = InjuryStatus.Minor;
            recoveryDays = Random.Range(1, 3);
        }
        else
        {
            if (playerInjuryAlert != null)
                playerInjuryAlert.SetActive(false);
            return;
        }

        daysToRecover = recoveryDays;
        SetInjuryStateText(recoveryDays);

        if (playerInjuryAlert != null)
            
            playerInjuryAlert.SetActive(true);
    }

    public void SetInjuryStateText(int recoveryDays)
    {
        daysToRecover = recoveryDays;
        if (playerInjuryAlert != null)
        {
            if (currentInjury == InjuryStatus.Healthy)
            {
                playerInjuryAlert.SetActive(false);
                injuryStateText.text = "";
                return;
            }
            choppingSource.PlayOneShot(InjuryClip);
            playerInjuryAlert.SetActive(true);
            string injuryText = "";

            if (currentInjury == InjuryStatus.Minor)
            {
                injuryText += $"You suffered a <color=#FFFF66>MINOR</color> injury!\n" +
                              $"Your chopping is 25% slower\n " +
                              $"Recovery time: {recoveryDays} days.\"";
            }
            else if (currentInjury == InjuryStatus.Moderate)
            {
                injuryText += $"You suffered a <color=#FFA500>MODERATE</color> injury!\n" +
                              $"Your chopping is 50% slower\n" +
                              $" Recovery time: {recoveryDays} days.";
            }
            else if (currentInjury == InjuryStatus.Severe)
            {
                injuryText += $"You suffered a <color=#C0392B>SEVERE</color> injury!\n" +
                              $"You are not able to chop\n" +
                              $"while severly injured\n" +
                              $" Recovery time: {recoveryDays} days.";
            }

            injuryAlertText.text = injuryText;
        }
    }



    private void CancelChopping()
    {
        if (!isChopping) return;

        //if (chopSfxCoroutine != null)
       // {
        //    StopCoroutine(chopSfxCoroutine);
       //     chopSfxCoroutine = null;
        //}
        
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
    [SerializeField] private float grassWalkVolume = 0.5f;
    [SerializeField] private float woodWalkVolume  = 0.7f;

    private void Update()
    {
           // Debug.Log("Regeneracija " + isStaminaRegenUpgrade + !isChopping + !animator.GetBool("IsWalking") + !canSleep);
        if (isStaminaRegenUpgrade && !isChopping && !animator.GetBool("IsWalking") && !canSleep)
        {
            passiveStaminaTimer += Time.deltaTime;

            if (passiveStaminaTimer >= 10f)
            {
                staminaController.playerStamina += 5f;
                staminaController.playerStamina = staminaController.playerStamina > 100 ? 100 : staminaController.playerStamina;
                passiveStaminaTimer = 0f; // Reset timer
//                 Debug.Log("Passive stamina regenerated +5 " + staminaController.playerStamina);
            }
        }
        else
        {
            passiveStaminaTimer = 0f; // Reset if conditions not met
        }
        
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
        
        if (move != Vector2.zero && !isChopping)
        {
            AudioClip clipToUse = isOnBridge ? woodWalkClip : grassWalkClip;
            float   volume    = isOnBridge ? woodWalkVolume : grassWalkVolume;

            if (walkingSource.clip != clipToUse || !walkingSource.isPlaying)
            {
                walkingSource.clip   = clipToUse;
                walkingSource.loop   = true;
                walkingSource.volume = volume;      // postavi glasnoću
                walkingSource.Play();
            }
        }
        else
        {
            if (walkingSource.isPlaying)
                walkingSource.Stop();
        }


       
        
        if (!isChopping && !isInShop && canSleep && Input.GetKeyDown(KeyCode.Space))
        {
            dayNightCycle.Sleep();
            FindAnyObjectByType<SaveManager>().SaveGame();
//             Debug.Log("SavedGame!");
            uiPanel.SetActive(false);
            interactionText.text = "";
        }
        UpdateInjuryRisk();
        UpdateInjuryStateText();
    }
    
    public void UpdateInjuryStateText()
    {
        if (injuryStateText == null)
            return;

        switch (currentInjury)
        {
            case InjuryStatus.Healthy:
                injuryStateText.text = "";
                break;

            case InjuryStatus.Minor:
                injuryStateText.text = $"<color=#FFFF66>Light injury</color> ({daysToRecover} day{(daysToRecover > 1 ? "s" : "")})";
                break;

            case InjuryStatus.Moderate:
                injuryStateText.text = $"<color=#FFA500>Moderate injury</color> ({daysToRecover} day{(daysToRecover > 1 ? "s" : "")})";
                break;

            case InjuryStatus.Severe:
                injuryStateText.text = $"<color=#C0392B>Severe injury</color> ({daysToRecover} day{(daysToRecover > 1 ? "s" : "")})";
                break;
        }
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
        if (other.CompareTag("Bridge"))
        {
            isOnBridge = true;
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
            if (uiPanel.activeSelf)
                uiPanel.SetActive(false);
        }
        else if (other.CompareTag("Shop"))
        {
            if (uiPanel.activeSelf)
                uiPanel.SetActive(false);
            if (isInShop)
            {
                isInShop = false;
                gameplayUI.SetActive(true);
//                 Debug.Log("Exited shop.");
            }
            
        }
        if (other.CompareTag("Bridge"))
        {
            isOnBridge = false;
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
