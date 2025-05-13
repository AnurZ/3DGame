using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradesManager : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    public StaminaController staminaController;
    public SaveManager saveManager;  // ← postavi u Inspectoru!

    [Header("Stamina Regen UI")]
    public Image staminaRegenBackground;
    public Image staminaRegenIcon;
    public Image staminaRegenInfoBackground;
    public GameObject staminaRegenBuyButton;

    [Header("Speed UI")]
    public Image speedBackground;
    public Image speedIcon;
    public Image speedInfoBackground;
    public GameObject speedBuyButton;

    [Header("Injury Shield UI")]
    public Image injuryShieldBackground;
    public Image injuryShieldIcon;
    public Image injuryShieldInfoBackground;
    public GameObject injuryShieldBuyButton;

    [Header("Severe Injury Shield UI")]
    public Image severeInjuryShieldBackground;
    public Image severeInjuryShieldIcon;
    public Image severeInjuryShieldInfoBackground;
    public GameObject severeInjuryShieldBuyButton;

    [Header("Potion Effect UI")]
    public Image potionEffectBackground;
    public Image potionEffectIcon;
    public Image potionEffectInfoBackground;
    public GameObject potionEffectBuyButton;

    [Header("Chopping Speed UI")]
    public Image choppingSpeedBackground;
    public Image choppingSpeedIcon;
    public Image choppingSpeedInfoBackground;
    public GameObject choppingSpeedBuyButton;

    [Header("Chopping Stamina UI")]
    public Image choppingStaminaBackground;
    public Image choppingStaminaIcon;
    public Image choppingStaminaInfoBackground;
    public GameObject choppingStaminaBuyButton;

    // Interni flagovi
    [HideInInspector] public bool hasStaminaRegen;
    [HideInInspector] public bool hasSpeed;
    [HideInInspector] public bool hasInjuryShield;
    [HideInInspector] public bool hasSevereInjuryShield;
    [HideInInspector] public bool hasPotionEffect;
    [HideInInspector] public bool hasChoppingSpeed;
    [HideInInspector] public bool hasChoppingStamina;

    
    public TMP_Text interactionText;
    
    public AchievementsController achievementsController;

    public CurrencyManager CurrencyManager;
    
    public PotionManager potionManager;
    
    private void Start()
    {
        CurrencyManager =  FindObjectOfType<CurrencyManager>();
        achievementsController = FindObjectOfType<AchievementsController>();
        potionManager = FindObjectOfType<PotionManager>();
    }

    private void Awake()
    {
        if (playerController == null) 
            playerController = FindObjectOfType<PlayerController>();
        if (staminaController == null) 
            staminaController = FindObjectOfType<StaminaController>();
        if (saveManager == null)
            saveManager = FindObjectOfType<SaveManager>();
    }

    private void PerformBuy(Action applyLogic, 
                            ref bool hasFlag,
                            GameObject buyButton, Image bg, Image icon, Image infoBg)
    {
        if (playerController == null)
        {
            Debug.LogError("UpgradesManager: playerController JE NULL!");
            playerController = FindObjectOfType<PlayerController>();
            if (playerController == null) return;
        }
        // 1) Postavi internu logiku
        applyLogic();

        // 2) Oboji UI
        Color32 purchasedColor = new Color32(0xc1, 0xc1, 0xc1, 0xff);
        buyButton.GetComponent<Image>().color = purchasedColor;
        bg.color       = purchasedColor;
        icon.color     = purchasedColor;
        infoBg.color   = purchasedColor;

        achievementsController.UnlockAllUpgrades++;
        
        // 3) Označi flag
        hasFlag = true;

        // 4) Automatski spremi
        saveManager?.SaveGame();
    }

    public void BuyStaminaRegenUpgrade()
    {
        if (CurrencyManager.TrySpendMoney(1))
        {
            PerformBuy(
                () =>
                {
                    playerController.isStaminaRegenUpgrade = true;
                },
                ref hasStaminaRegen,
                staminaRegenBuyButton,
                staminaRegenBackground,
                staminaRegenIcon,
                staminaRegenInfoBackground
            );
        }
        else
        {
            interactionText.gameObject.SetActive(true);
            interactionText.text = "You do not have enough money to buy this potion!";
            interactionText.color = Color.red;
            staminaRegenBuyButton.SetActive(false);
        }
    }

    public void BuySpeedUpgrade()
    {
        if (CurrencyManager.TrySpendMoney(1500))
        {
            PerformBuy(
                () => { playerController.speed = 10f; },
                ref hasSpeed,
                speedBuyButton,
                speedBackground,
                speedIcon,
                speedInfoBackground
            );
            Debug.Log("Buying Speed");
        }
        else
        {
            Debug.Log("Nema dobvoljno");
            interactionText.gameObject.SetActive(true);
            interactionText.text = "You do not have enough money to buy this potion!";
            interactionText.color = Color.red;
            speedBuyButton.SetActive(false);
        }
    }

    public void BuyInjuryShieldUpgrade()
    {
        if (CurrencyManager.TrySpendMoney(1000))
        {
            PerformBuy(
                () => { playerController.injuryShieldUpgrade = true; },
                ref hasInjuryShield,
                injuryShieldBuyButton,
                injuryShieldBackground,
                injuryShieldIcon,
                injuryShieldInfoBackground
            );
            Debug.Log("Buying Injury Shield");
        }
        else
        {
            interactionText.gameObject.SetActive(true);
            interactionText.text = "You do not have enough money to buy this potion!";
            interactionText.color = Color.red;
            injuryShieldBuyButton.SetActive(false);
        }
    }

    public void BuySevereInjuryShieldUpgrade()
    {
        if (CurrencyManager.TrySpendMoney(3000))
        {
            PerformBuy(
                () => { playerController.severeInjuryShieldUpgrade = true; },
                ref hasSevereInjuryShield,
                severeInjuryShieldBuyButton,
                severeInjuryShieldBackground,
                severeInjuryShieldIcon,
                severeInjuryShieldInfoBackground
            );
            Debug.Log("Buying Severe Injury Shield");
        }
        else
        {
            interactionText.gameObject.SetActive(true);
            interactionText.text = "You do not have enough money to buy this potion!";
            interactionText.color = Color.red;
            severeInjuryShieldBuyButton.SetActive(false);
        }
    }

    public void BuyPotionEffectUpgrade()
    {
        if (CurrencyManager.TrySpendMoney(1000))
        {
            PerformBuy(
                () =>
                {
                    potionManager.PotionEffectUpgradeBought = true;
                },
                ref hasPotionEffect,
                potionEffectBuyButton,
                potionEffectBackground,
                potionEffectIcon,
                potionEffectInfoBackground
            );
            Debug.Log("Buying Potion Effect");
        }
        else
        {
            interactionText.gameObject.SetActive(true);
            interactionText.text = "You do not have enough money to buy this potion!";
            interactionText.color = Color.red;
            potionEffectBuyButton.SetActive(false);
        }
    }

    public void BuyChoppingSpeedUpgrade()
    {
        if (CurrencyManager.TrySpendMoney(1500))
        {
            PerformBuy(
                () => { playerController.choppingSpeedUpgrade = true; },
                ref hasChoppingSpeed,
                choppingSpeedBuyButton,
                choppingSpeedBackground,
                choppingSpeedIcon,
                choppingSpeedInfoBackground
            );
            Debug.Log("Buying Chopping Speed");
        }
        else
        {
            interactionText.gameObject.SetActive(true);
            interactionText.text = "You do not have enough money to buy this potion!";
            interactionText.color = Color.red;
            choppingSpeedBuyButton.SetActive(false);
        }
    }

    public void BuyChoppingStaminaUpgrade()
    {
        if (CurrencyManager.TrySpendMoney(2000))
        {
            PerformBuy(
                () => { staminaController.staminaReductionRate *= 0.8f; },
                ref hasChoppingStamina,
                choppingStaminaBuyButton,
                choppingStaminaBackground,
                choppingStaminaIcon,
                choppingStaminaInfoBackground
            );
            Debug.Log("Buying Chopping Stamina");
        }
        else
        {
            interactionText.gameObject.SetActive(true);
            interactionText.text = "You do not have enough money to buy this potion!";
            interactionText.color = Color.red;
            choppingStaminaBuyButton.SetActive(false);
        }
    }
}
