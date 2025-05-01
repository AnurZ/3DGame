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

    public int ShieldPotionDays = 0;
    public GameObject shieldPotionInfo;
    public Text shieldPotionText;
    
    
    public int FocusPotionDays = 0;
    public GameObject FocusPotionInfo;
    public Text FocusPotionText;
    
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
    }

    private void Update()
    {
        if (FocusPotionDays <= 0)
        {
            FocusPotionInfo.SetActive(false);
        }
        else
        {
            FocusPotionText.text = FocusPotionDays + " day" + (FocusPotionDays > 1 ? "s left" : " left");
            FocusPotionInfo.SetActive(true);
        }
        if (ShieldPotionDays <= 0)
        {
            shieldPotionInfo.SetActive(false);
        }
        else
        {
            shieldPotionText.text = ShieldPotionDays + " day" + (ShieldPotionDays > 1 ? "s left" : " left");
            shieldPotionInfo.SetActive(true);
        }
        isPotionHeld = IsHoldingPotion();
        if (isPotionHeld && Input.GetKeyDown(KeyCode.Space)) // Space bar is used to drink the potion
        {
            DrinkPotion();
        }
    }

    public void reduceShieldPotionDays()
    {
        ShieldPotionDays--;
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
            return;
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
            Debug.Log("Potion healed minor injury.");
        }
        else if (playerController.currentInjury == PlayerController.InjuryStatus.Moderate)
        {
            // If the injury is moderate, reduce it to minor
            playerController.currentInjury = PlayerController.InjuryStatus.Minor;
            playerController.UpdateInjuryStateText();
        }
        inventoryManager.RemoveItemFromHand();
    }

    private void IncreaseStamina()
    {
        staminaController.playerStamina += 50;
        if (staminaController.playerStamina >= 100)
            staminaController.playerStamina = 100;
        inventoryManager.RemoveItemFromHand();
    }

    private void ActivateShield()
    {
        ShieldPotionDays += 1;
        shieldPotionText.text = ShieldPotionDays + " day" + (ShieldPotionDays > 1 ? "s left" : " left");
        shieldPotionInfo.SetActive(true);
        inventoryManager.RemoveItemFromHand();
    }
    
    private void IncreaseFocus()
    {
        FocusPotionDays += 1;
        FocusPotionText.text = FocusPotionDays + " day" + (FocusPotionDays > 1 ? "s left" : " left");
        FocusPotionInfo.SetActive(true);
        inventoryManager.RemoveItemFromHand();
    }

    private void UpgradePotion() => throw new System.NotImplementedException();
}
