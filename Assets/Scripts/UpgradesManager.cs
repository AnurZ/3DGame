using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradesManager : MonoBehaviour
{
    
    public int staminaUpgradeCost = 0;
    public int SpeedCost = 0;
    public int InjuryShieldCost = 0;
    public int SevereInjuryShieldCost = 0;
    public int PotionEffectCost = 0;
    public int ChoppingSpeedCost = 0;
    public int ChoppingStaminaCost = 0;
    
    [Header("References")]
    public PlayerController playerController;
    public StaminaController staminaController;
    public SaveManager saveManager;  // ← postavi u Inspectoru!

    [Header("Stamina Regen UI")]
    public Image staminaRegenBackground;
    public Image staminaRegenIcon;
    public Image staminaRegenInfoBackground;
    public GameObject staminaRegenBuyButton;
    public GameObject staminaRegenBoughtButton; // ← NOVO

    [Header("Speed UI")]
    public Image speedBackground;
    public Image speedIcon;
    public Image speedInfoBackground;
    public GameObject speedBuyButton;
    public GameObject speedBoughtButton; // ← NOVO

    [Header("Injury Shield UI")]
    public Image injuryShieldBackground;
    public Image injuryShieldIcon;
    public Image injuryShieldInfoBackground;
    public GameObject injuryShieldBuyButton;
    public GameObject injuryShieldBoughtButton; // ← NOVO

    [Header("Severe Injury Shield UI")]
    public Image severeInjuryShieldBackground;
    public Image severeInjuryShieldIcon;
    public Image severeInjuryShieldInfoBackground;
    public GameObject severeInjuryShieldBuyButton;
    public GameObject severeInjuryShieldBoughtButton; // ← NOVO

    [Header("Potion Effect UI")]
    public Image potionEffectBackground;
    public Image potionEffectIcon;
    public Image potionEffectInfoBackground;
    public GameObject potionEffectBuyButton;
    public GameObject potionEffectBoughtButton; // ← NOVO

    [Header("Chopping Speed UI")]
    public Image choppingSpeedBackground;
    public Image choppingSpeedIcon;
    public Image choppingSpeedInfoBackground;
    public GameObject choppingSpeedBuyButton;
    public GameObject choppingSpeedBoughtButton; // ← NOVO

    [Header("Chopping Stamina UI")]
    public Image choppingStaminaBackground;
    public Image choppingStaminaIcon;
    public Image choppingStaminaInfoBackground;
    public GameObject choppingStaminaBuyButton;
    public GameObject choppingStaminaBoughtButton; // ← NOVO


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
       // CurrencyManager =  FindObjectOfType<CurrencyManager>();
       // achievementsController = FindObjectOfType<AchievementsController>();
        //potionManager = FindObjectOfType<PotionManager>();
    }
    
    public void ApplyUpgradeUI(
        GameObject buyButton,
        GameObject boughtButton,
        Image bg,
        Image icon,
        Image infoBg)
    {
        Debug.Log("RefreshingUpgradeUI");
        
        Color32 purchasedColor = new Color32(0xc1, 0xc1, 0xc1, 0xff);
        // Oboji sve elemente
        bg.color     = purchasedColor;
        icon.color   = purchasedColor;
        infoBg.color = purchasedColor;

        // Onemogući stari buy button
        buyButton.SetActive(false);

        // Aktiviraj bought‑button (checkmark)
        if(boughtButton != null)
            boughtButton.SetActive(true);
    }



    private void Awake()
    {
       // Debug.Log("[UpgradesManager.Awake] Binding references…");

        playerController      = playerController      ?? FindObjectOfType<PlayerController>();
        staminaController     = staminaController     ?? FindObjectOfType<StaminaController>();
        potionManager         = potionManager         ?? FindObjectOfType<PotionManager>();
        CurrencyManager       = CurrencyManager       ?? FindObjectOfType<CurrencyManager>();
        achievementsController= achievementsController?? FindObjectOfType<AchievementsController>();
        saveManager           = saveManager           ?? FindObjectOfType<SaveManager>();

        //Debug.Log($"[UpgradesManager.Awake] potionManager = {(potionManager==null?"NULL":"OK")}");
    }


    [ContextMenu("Refresh All Upgrades UI")]
    public void RefreshAllUpgradesUI()
    {
        Debug.Log("RefreshAllUpgradesUI");

        if (hasStaminaRegen)
            ApplyUpgradeUI(
                staminaRegenBuyButton,
                staminaRegenBoughtButton,
                staminaRegenBackground,
                staminaRegenIcon,
                staminaRegenInfoBackground
            );

        if (hasSpeed)
            ApplyUpgradeUI(
                speedBuyButton,
                speedBoughtButton,
                speedBackground,
                speedIcon,
                speedInfoBackground
            );

        if (hasInjuryShield)
            ApplyUpgradeUI(
                injuryShieldBuyButton,
                injuryShieldBoughtButton,
                injuryShieldBackground,
                injuryShieldIcon,
                injuryShieldInfoBackground
            );

        if (hasSevereInjuryShield)
            ApplyUpgradeUI(
                severeInjuryShieldBuyButton,
                severeInjuryShieldBoughtButton,
                severeInjuryShieldBackground,
                severeInjuryShieldIcon,
                severeInjuryShieldInfoBackground
            );

        if (hasPotionEffect)
            ApplyUpgradeUI(
                potionEffectBuyButton,
                potionEffectBoughtButton,
                potionEffectBackground,
                potionEffectIcon,
                potionEffectInfoBackground
            );

        if (hasChoppingSpeed)
            ApplyUpgradeUI(
                choppingSpeedBuyButton,
                choppingSpeedBoughtButton,
                choppingSpeedBackground,
                choppingSpeedIcon,
                choppingSpeedInfoBackground
            );

        if (hasChoppingStamina)
            ApplyUpgradeUI(
                choppingStaminaBuyButton,
                choppingStaminaBoughtButton,
                choppingStaminaBackground,
                choppingStaminaIcon,
                choppingStaminaInfoBackground
            );
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
        if (CurrencyManager.TrySpendMoney(staminaUpgradeCost))
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
            interactionText.text = "You do not have enough money to buy this upgrade!";
            interactionText.color = Color.red;
            staminaRegenBuyButton.SetActive(false);
        }
    }

    public void BuySpeedUpgrade()
    {
        if (CurrencyManager.TrySpendMoney(SpeedCost))
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
            interactionText.text = "You do not have enough money to buy this upgrade!";
            interactionText.color = Color.red;
            speedBuyButton.SetActive(false);
        }
    }

    public void BuyInjuryShieldUpgrade()
    {
        if (CurrencyManager.TrySpendMoney(InjuryShieldCost))
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
            interactionText.text = "You do not have enough money to buy this upgrade!";
            interactionText.color = Color.red;
            injuryShieldBuyButton.SetActive(false);
        }
    }

    public void BuySevereInjuryShieldUpgrade()
    {
        if (CurrencyManager.TrySpendMoney(SevereInjuryShieldCost))
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
            interactionText.text = "You do not have enough money to buy this upgrade!";
            interactionText.color = Color.red;
            severeInjuryShieldBuyButton.SetActive(false);
        }
    }

    public void BuyPotionEffectUpgrade()
    {
        if (CurrencyManager.TrySpendMoney(PotionEffectCost))
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
            interactionText.text = "You do not have enough money to buy this upgrade!";
            interactionText.color = Color.red;
            potionEffectBuyButton.SetActive(false);
        }
    }

    public void BuyChoppingSpeedUpgrade()
    {
        if (CurrencyManager.TrySpendMoney(ChoppingSpeedCost))
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
            interactionText.text = "You do not have enough money to buy this upgrade!";
            interactionText.color = Color.red;
            choppingSpeedBuyButton.SetActive(false);
        }
    }

    public void BuyChoppingStaminaUpgrade()
    {
        if (CurrencyManager.TrySpendMoney(ChoppingStaminaCost))
        {
            PerformBuy(
                () => { staminaController.staminaReductionRate *= 0.8f; },
                ref hasChoppingStamina,
                choppingStaminaBuyButton,
                choppingStaminaBackground,
                choppingStaminaIcon,
                choppingStaminaInfoBackground
            );
            //Debug.Log("Buying Chopping Stamina");
        }
        else
        {
            interactionText.gameObject.SetActive(true);
            interactionText.text = "You do not have enough money to buy this upgrade!";
            interactionText.color = Color.red;
            choppingStaminaBuyButton.SetActive(false);
        }
    }
}
