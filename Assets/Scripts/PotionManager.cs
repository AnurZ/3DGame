using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PotionManager : MonoBehaviour
{
    public GameObject potionPrefab; // Reference to the potion prefab (set in the Inspector)
    public PlayerController playerController;
    private bool isPotionHeld = false; // To track if the potion is currently held
    private PotionType currentPotionType;
    public InventoryManager inventoryManager;

    public StaminaController staminaController;

    public int ShieldPotionHours = 0;
    public GameObject shieldPotionInfo;
    public Text shieldPotionText;
    
    
    public int FocusPotionHours = 0;
    public GameObject FocusPotionInfo;
    public Text FocusPotionText;

    public int UpgradePotionHours = 0;
    public GameObject UpgradePotionInfo;
    public Text UpgradePotionText;
    
    public GameObject Prompt;
    public TextMeshProUGUI promptText;
    
    public AchievementsController achievementsController;

    public bool PotionEffectUpgradeBought = false;
    
    public enum PotionType
    {
        HealthPotion,
        StaminaPotion,
        ShieldPotion,
        UpgradePotion,
        FocusPotion
    }

    public PotionType getSelectedPotionType()
    {
        var potionPrefab = inventoryManager.GetSelectedItem(false).itemPrefab;
            Potion potionScript = potionPrefab.GetComponent<Potion>();
        return potionScript.potionType;
    }
    
    private bool IsHoldingPotion()
    {
        if (inventoryManager == null) return false;
        Item selectedItem = inventoryManager.GetSelectedItem(false);
        return selectedItem != null && selectedItem.isPotion;
    }
    
    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        staminaController = FindObjectOfType<StaminaController>();
        achievementsController = FindObjectOfType<AchievementsController>();
    }

    private void Update()
    {
        if (UpgradePotionHours <= 0)
        {
            UpgradePotionInfo.SetActive(false);
        }
        else
        {
            UpgradePotionText.text = UpgradePotionHours + " hour" + (UpgradePotionHours > 1 ? "s left" : " left");
            UpgradePotionInfo.SetActive(true);
        }
        if (FocusPotionHours <= 0)
        {
            FocusPotionInfo.SetActive(false);
        }
        else
        {
            FocusPotionText.text = FocusPotionHours + " hour" + (FocusPotionHours > 1 ? "s left" : " left");
            FocusPotionInfo.SetActive(true);
        }
        if (ShieldPotionHours <= 0)
        {
            shieldPotionInfo.SetActive(false);
        }
        else
        {
            shieldPotionText.text = ShieldPotionHours + " hour" + (ShieldPotionHours > 1 ? "s left" : " left");
            shieldPotionInfo.SetActive(true);
        }
        isPotionHeld = IsHoldingPotion();
        if (isPotionHeld && Input.GetKeyDown(KeyCode.Space)) // Space bar is used to drink the potion
        {
            DrinkPotion();
        }
    }

    private void DrinkPotion()
    {
        isPotionHeld = IsHoldingPotion();
        // If a potion is held, drink it to reduce the injury state
        if (isPotionHeld && playerController != null)
        {
            var PotionHeld = inventoryManager.GetSelectedItem(false);
            // Perform the effect based on the potion type
            switch (getSelectedPotionType())
            {
                case PotionType.HealthPotion:
                    ReduceInjury();
                    break;

                case PotionType.StaminaPotion:
                    IncreaseStamina();
                    break;

                case PotionType.ShieldPotion:
                    ActivateShield();
                    break;
                
                case PotionType.FocusPotion:
                    IncreaseFocus();
                    break;
                
                case PotionType.UpgradePotion:
                    UpgradePotion();
                    break;

                default:
                    Debug.Log("Unknown potion type.");
                    break;
            }
            
            // After drinking, reset the potion state
            isPotionHeld = false;
        }
    }

    

    private void ReduceInjury()
    {
        if (playerController.currentInjury == PlayerController.InjuryStatus.Healthy)
        {
            Prompt.SetActive(true);
            promptText.text = "You are already healthy!";
            return;
        }
        // If the player is not severely injured, reduce the injury
        if (playerController.currentInjury == PlayerController.InjuryStatus.Severe)
        {
            playerController.currentInjury = PlayerController.InjuryStatus.Moderate;
            playerController.UpdateInjuryStateText();
            Debug.Log("Potion has no effect on severe injuries.");
        }
        else if (playerController.currentInjury == PlayerController.InjuryStatus.Minor)
        {
            // If the injury is minor, set to healthy and keep recovery days
            playerController.currentInjury = PlayerController.InjuryStatus.Healthy;
            playerController.daysToRecover = 0; // Instant recovery
            playerController.UpdateInjuryStateText();
            achievementsController.HealFromInjury = 1;
            Debug.Log("Potion healed minor injury.");
        }
        else if (playerController.currentInjury == PlayerController.InjuryStatus.Moderate)
        {
            // If the injury is moderate, reduce it to minor
            playerController.currentInjury = PlayerController.InjuryStatus.Minor;
            playerController.UpdateInjuryStateText();
        }
        if(PotionEffectUpgradeBought && playerController.daysToRecover > 0)
            playerController.daysToRecover--;
        inventoryManager.RemoveItemFromHand();
    }

    private void IncreaseStamina()
    {
        if (staminaController.playerStamina == 100)
        {
            Prompt.SetActive(true);
            promptText.text = "Your stamina is already full!";
            return;
        }
        staminaController.playerStamina += (PotionEffectUpgradeBought ? 55 : 50);
        if (staminaController.playerStamina >= 100)
            staminaController.playerStamina = 100;
        inventoryManager.RemoveItemFromHand();
    }

    private void ActivateShield()
    {
        ShieldPotionHours += (PotionEffectUpgradeBought ? 26 : 24);
        shieldPotionText.text = ShieldPotionHours + " hour" + (ShieldPotionHours > 1 ? "s left" : " left");
        shieldPotionInfo.SetActive(true);
        inventoryManager.RemoveItemFromHand();
    }
    
    private void IncreaseFocus()
    {
        FocusPotionHours += (PotionEffectUpgradeBought ? 26 : 24);
        FocusPotionText.text = FocusPotionHours + " hour" + (FocusPotionHours > 1 ? "s left" : " left");
        FocusPotionInfo.SetActive(true);
        inventoryManager.RemoveItemFromHand();
    }

    private void UpgradePotion()
    { 
        UpgradePotionHours += (PotionEffectUpgradeBought ? 26 : 24);
        UpgradePotionText.text = UpgradePotionHours + " hour" + (UpgradePotionHours > 1 ? "s left" : " left");
        UpgradePotionInfo.SetActive(true);
        inventoryManager.RemoveItemFromHand();
    }
}
