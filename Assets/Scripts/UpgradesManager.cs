using System;
using UnityEngine;
using UnityEngine.UI;

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

    private void Start()
    {
        
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

        // 2) Obojaj UI
        Color32 purchasedColor = new Color32(0xc1, 0xc1, 0xc1, 0xff);
        buyButton.GetComponent<Image>().color = purchasedColor;
        bg.color       = purchasedColor;
        icon.color     = purchasedColor;
        infoBg.color   = purchasedColor;

        // 3) Označi flag
        hasFlag = true;

        // 4) Automatski spremi
        saveManager?.SaveGame();
    }

    public void BuyStaminaRegenUpgrade()
    {
        PerformBuy(
            () => { /* nema dodatne logike */ },
            ref hasStaminaRegen,
            staminaRegenBuyButton,
            staminaRegenBackground,
            staminaRegenIcon,
            staminaRegenInfoBackground
        );
        Debug.Log("Buying Stamina Regen");
    }

    public void BuySpeedUpgrade()
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

    public void BuyInjuryShieldUpgrade()
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

    public void BuySevereInjuryShieldUpgrade()
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

    public void BuyPotionEffectUpgrade()
    {
        PerformBuy(
            () => { /* nema dodatne logike */ },
            ref hasPotionEffect,
            potionEffectBuyButton,
            potionEffectBackground,
            potionEffectIcon,
            potionEffectInfoBackground
        );
        Debug.Log("Buying Potion Effect");
    }

    public void BuyChoppingSpeedUpgrade()
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

    public void BuyChoppingStaminaUpgrade()
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
}
