using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradesManager : MonoBehaviour
{
    [Header("Costs")]
    public int staminaUpgradeCost      = 100;
    public int speedUpgradeCost        = 1500;
    public int injuryShieldCost        = 1000;
    public int severeInjuryShieldCost  = 3000;
    public int potionEffectCost        = 1000;
    public int choppingSpeedCost       = 1500;
    public int choppingStaminaCost     = 2000;

    [Header("References")]
    public PlayerController playerController;
    public StaminaController staminaController;
    public PotionManager potionManager;          // ← referenca na instancu
    public SaveManager saveManager;
    public CurrencyManager CurrencyManager;
    public AchievementsController achievementsController;

    [Header("Stamina Regen UI")]
    public Image staminaRegenRowBg;
    public Image staminaRegenIcon;
    public Image staminaRegenInfoBg;
    public GameObject staminaRegenBuyButton;
    public GameObject staminaRegenBoughtButton;

    [Header("Speed UI")]
    public Image speedRowBg;
    public Image speedIcon;
    public Image speedInfoBg;
    public GameObject speedBuyButton;
    public GameObject speedBoughtButton;

    [Header("Injury Shield UI")]
    public Image injuryShieldRowBg;
    public Image injuryShieldIcon;
    public Image injuryShieldInfoBg;
    public GameObject injuryShieldBuyButton;
    public GameObject injuryShieldBoughtButton;

    [Header("Severe Injury Shield UI")]
    public Image severeInjuryShieldRowBg;
    public Image severeInjuryShieldIcon;
    public Image severeInjuryShieldInfoBg;
    public GameObject severeInjuryShieldBuyButton;
    public GameObject severeInjuryShieldBoughtButton;

    [Header("Potion Effect UI")]
    public Image potionEffectRowBg;
    public Image potionEffectIcon;
    public Image potionEffectInfoBg;
    public GameObject potionEffectBuyButton;
    public GameObject potionEffectBoughtButton;

    [Header("Chopping Speed UI")]
    public Image choppingSpeedRowBg;
    public Image choppingSpeedIcon;
    public Image choppingSpeedInfoBg;
    public GameObject choppingSpeedBuyButton;
    public GameObject choppingSpeedBoughtButton;

    [Header("Chopping Stamina UI")]
    public Image choppingStaminaRowBg;
    public Image choppingStaminaIcon;
    public Image choppingStaminaInfoBg;
    public GameObject choppingStaminaBuyButton;
    public GameObject choppingStaminaBoughtButton;

    // Interni flagovi
    [HideInInspector] public bool hasStaminaRegen;
    [HideInInspector] public bool hasSpeed;
    [HideInInspector] public bool hasInjuryShield;
    [HideInInspector] public bool hasSevereInjuryShield;
    [HideInInspector] public bool hasPotionEffect;
    [HideInInspector] public bool hasChoppingSpeed;
    [HideInInspector] public bool hasChoppingStamina;

    public TMP_Text interactionText;
    public AudioSource AudioSource;
    public AudioClip AudioClip;
    public AudioClip AudioClip2;
    private void Awake()
    {
        // Ako nisu postavljene u Inspectoru, pronađi ih
        playerController       ??= FindObjectOfType<PlayerController>();
        staminaController      ??= FindObjectOfType<StaminaController>();
        potionManager          ??= FindObjectOfType<PotionManager>();
        saveManager            ??= FindObjectOfType<SaveManager>();
        CurrencyManager        ??= FindObjectOfType<CurrencyManager>();
        achievementsController ??= FindObjectOfType<AchievementsController>();
    }

    /// <summary>
    /// Sinkroniziraj UI nakon LoadGame poziva.
    /// </summary>
    [ContextMenu("Refresh All Upgrades UI")]
    public void RefreshAllUpgradesUI()
    {
        if (hasStaminaRegen)    ApplyPurchasedUI(staminaRegenBuyButton,   staminaRegenBoughtButton,   staminaRegenRowBg,   staminaRegenIcon,   staminaRegenInfoBg);
        if (hasSpeed)           ApplyPurchasedUI(speedBuyButton,          speedBoughtButton,          speedRowBg,          speedIcon,          speedInfoBg);
        if (hasInjuryShield)    ApplyPurchasedUI(injuryShieldBuyButton,   injuryShieldBoughtButton,   injuryShieldRowBg,   injuryShieldIcon,   injuryShieldInfoBg);
        if (hasSevereInjuryShield) ApplyPurchasedUI(severeInjuryShieldBuyButton, severeInjuryShieldBoughtButton, severeInjuryShieldRowBg, severeInjuryShieldIcon, severeInjuryShieldInfoBg);
        if (hasPotionEffect)    ApplyPurchasedUI(potionEffectBuyButton,   potionEffectBoughtButton,   potionEffectRowBg,   potionEffectIcon,   potionEffectInfoBg);
        if (hasChoppingSpeed)   ApplyPurchasedUI(choppingSpeedBuyButton,  choppingSpeedBoughtButton,  choppingSpeedRowBg,  choppingSpeedIcon,  choppingSpeedInfoBg);
        if (hasChoppingStamina) ApplyPurchasedUI(choppingStaminaBuyButton,choppingStaminaBoughtButton,choppingStaminaRowBg,choppingStaminaIcon,choppingStaminaInfoBg);
    }

    private void ApplyPurchasedUI(
        GameObject buyBtn,
        GameObject boughtBtn,
        Image rowBg,
        Image icon,
        Image infoBg)
    {
        Color32 grey = new Color32(0xC1, 0xC1, 0xC1, 0xFF);
        buyBtn.SetActive(false);
        boughtBtn.SetActive(true);
        rowBg.color   = grey;
        icon.color    = grey;
        infoBg.color  = grey;
    }

    /// <summary>
    /// Centralna logika za kupnju + automatski save.
    /// </summary>
    private void PerformBuy(
        int cost,
        Action applyLogic,
        ref bool hasFlag,
        GameObject buyBtn,
        GameObject boughtBtn,
        Image rowBg,
        Image icon,
        Image infoBg)
    {
        // 1) Pokušaj skinuti pare
        if (!CurrencyManager.TrySpendMoney(cost))
        {
            interactionText.gameObject.SetActive(true);
            interactionText.text = "You do not have enough money!";
            interactionText.color = Color.red;
            
            buyBtn.SetActive(true);
            boughtBtn.SetActive(false);
            AudioSource.PlayOneShot(AudioClip2);
            return;
        }

        // 2) Primijeni efekt
        applyLogic();
        hasFlag = true;
        achievementsController.UnlockAllUpgrades++;

        // 3) Osvježi UI na kupljeno
        ApplyPurchasedUI(buyBtn, boughtBtn, rowBg, icon, infoBg);
        AudioSource.PlayOneShot(AudioClip);
        // 4) Automatski spremi
        saveManager?.SaveGame();
    }

    #region BuyMethods

    public void BuyStaminaRegenUpgrade()
    {
        PerformBuy(
            staminaUpgradeCost,
            () => playerController.isStaminaRegenUpgrade = true,
            ref hasStaminaRegen,
            staminaRegenBuyButton,
            staminaRegenBoughtButton,
            staminaRegenRowBg,
            staminaRegenIcon,
            staminaRegenInfoBg
        );
    }

    public void BuySpeedUpgrade()
    {
        PerformBuy(
            speedUpgradeCost,
            () => playerController.speed = 10f,
            ref hasSpeed,
            speedBuyButton,
            speedBoughtButton,
            speedRowBg,
            speedIcon,
            speedInfoBg
        );
    }

    public void BuyInjuryShieldUpgrade()
    {
        PerformBuy(
            injuryShieldCost,
            () => playerController.injuryShieldUpgrade = true,
            ref hasInjuryShield,
            injuryShieldBuyButton,
            injuryShieldBoughtButton,
            injuryShieldRowBg,
            injuryShieldIcon,
            injuryShieldInfoBg
        );
    }

    public void BuySevereInjuryShieldUpgrade()
    {
        PerformBuy(
            severeInjuryShieldCost,
            () => playerController.severeInjuryShieldUpgrade = true,
            ref hasSevereInjuryShield,
            severeInjuryShieldBuyButton,
            severeInjuryShieldBoughtButton,
            severeInjuryShieldRowBg,
            severeInjuryShieldIcon,
            severeInjuryShieldInfoBg
        );
    }

    public void BuyPotionEffectUpgrade()
    {
        // **Ovdje koristimo instancu potionManager, ne klasu**
        PerformBuy(
            potionEffectCost,
            () => potionManager.PotionEffectUpgradeBought = true,
            ref hasPotionEffect,
            potionEffectBuyButton,
            potionEffectBoughtButton,
            potionEffectRowBg,
            potionEffectIcon,
            potionEffectInfoBg
        );
    }

    public void BuyChoppingSpeedUpgrade()
    {
        PerformBuy(
            choppingSpeedCost,
            () => playerController.choppingSpeedUpgrade = true,
            ref hasChoppingSpeed,
            choppingSpeedBuyButton,
            choppingSpeedBoughtButton,
            choppingSpeedRowBg,
            choppingSpeedIcon,
            choppingSpeedInfoBg
        );
    }

    public void BuyChoppingStaminaUpgrade()
    {
        PerformBuy(
            choppingStaminaCost,
            () => staminaController.staminaReductionRate *= 0.8f,
            ref hasChoppingStamina,
            choppingStaminaBuyButton,
            choppingStaminaBoughtButton,
            choppingStaminaRowBg,
            choppingStaminaIcon,
            choppingStaminaInfoBg
        );
    }

    #endregion
}
