using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementsController : MonoBehaviour
{
    private const string Prefix = "ACH_";

    public bool DidDrinkPotionToday => !drinkPotionsDailyCollected;

    // -- UI & Data Fields --

    // TreesChopped (5 tiers)
    public Image TreesChoppedSlider;
    public TextMeshProUGUI TreesChoppedFillAmount;
    public int TreesChopped;
    [SerializeField] private int TreesChoppedGoal = 5000;
    public Image TreesChoppedIconBackground, TreesChoppedIcon, TreesChoppedCoin, TreesChoppedBoughtIcon;
    public TextMeshProUGUI TreesChoppedText;
    public int[] TreesChoppedLevels = { 1, 100, 500, 1000, 5000 };
    public int TreesChoppedCurrentLevel = 0;
    private bool[] treesChoppedCollected = new bool[5];

    // UnlockAllAxes (1 tier)
    public Image UnlockAllAxesSlider;
    public TextMeshProUGUI UnlockAllAxesFillAmount;
    public int UnlockAllAxes;
    [SerializeField] private int UnlockAllAxesGoal = 4;
    public Image UnlockAllAxesIconBackground, UnlockAllAxesIcon, UnlockAllAxesCoin, UnlockAllAxesBoughtIcon;
    public TextMeshProUGUI UnlockAllAxesText;
    private bool unlockAllAxesCollected = false;

    // ChopAllTreeTypes (1 tier)
    public Image ChopAllTreeTypesSlider;
    public TextMeshProUGUI ChopAllTreeTypesFillAmount;
    public int ChopAllTreeTypes;
    [SerializeField] private int ChopAllTreeTypesGoal = 5;
    public Image ChopAllTreeTypesIconBackground, ChopAllTreeTypesIcon, ChopAllTreeTypesCoin, ChopAllTreeTypesBoughtIcon;
    public TextMeshProUGUI ChopAllTreeTypesText;
    public int TreeType1, TreeType2, TreeType3, TreeType4, TreeType5;
    private bool chopAllTreeTypesCollected = false;

    // HealFromInjury (1 tier)
    public Image HealFromInjurySlider;
    public TextMeshProUGUI HealFromInjuryFillAmount;
    public int HealFromInjury;
    [SerializeField] private int HealFromInjuryGoal = 1;
    public Image HealFromInjuryIconBackground, HealFromInjuryIcon, HealFromInjuryCoin, HealFromInjuryBoughtIcon;
    public TextMeshProUGUI HealFromInjuryText;
    private bool healFromInjuryCollected = false;

    // UsePotions (3 tiers)
    public Image UsePotionsSlider;
    public TextMeshProUGUI UsePotionsFillAmount;
    public int UsePotions;
    [SerializeField] private int UsePotionsGoal = 20;
    public Image UsePotionsIconBackground, UsePotionsIcon, UsePotionsCoin, UsePotionsBoughtIcon;
    public TextMeshProUGUI UsePotionsText;
    public int[] UsePotionsLevel = { 1, 10, 20 };
    public int UsePotionsCurrentLevel = 0;
    private bool[] usePotionsCollected = new bool[3];

    // UnlockAllUpgrades (1 tier)
    public Image UnlockAllUpgradesSlider;
    public TextMeshProUGUI UnlockAllUpgradesFillAmount;
    public int UnlockAllUpgrades;
    [SerializeField] private int UnlockAllUpgradesGoal = 8;
    public Image UnlockAllUpgradesIconBackground, UnlockAllUpgradesIcon, UnlockAllUpgradesCoin, UnlockAllUpgradesBoughtIcon;
    public TextMeshProUGUI UnlockAllUpgradesText;
    private bool unlockAllUpgradesCollected = false;

    // SpendMoney (3 tiers)
    public Image SpendMoneySlider;
    public TextMeshProUGUI SpendMoneyFillAmount;
    public int SpendMoney;
    [SerializeField] private int SpendMoneyGoal = 10000;
    public Image SpendMoneyIconBackground, SpendMoneyIcon, SpendMoneyCoin, SpendMoneyBoughtIcon;
    public TextMeshProUGUI SpendMoneyText;
    public int[] SpendMoneyLevel = { 500, 5000, 10000 };
    public int SpendMoneyCurrentLevel = 0;
    private bool[] spendMoneyCollected = new bool[3];

    // InteractWithNPCs (3 tiers)
    public Image InteractWithNPCsSlider;
    public TextMeshProUGUI InteractWithNPCsFillAmount;
    public int InteractWithNPCs;
    [SerializeField] public int InteractWithNPCsGoal = 50;
    public Image InteractWithNPCsIconBackground, InteractWithNPCsIcon, InteractWithNPCsCoin, InteractWithNPCsBoughtIcon;
    public TextMeshProUGUI InteractWithNPCsText;
    public int[] InteractWithNPCSLevel = { 10, 20, 50 };
    public int InteractWithNPCsCurrentLevel = 0;
    private bool[] interactWithNPCsCollected = new bool[3];

    // DrinkPotionsDaily (1 tier)
    public Image DrinkPotionsDailySlider;
    public TextMeshProUGUI DrinkPotionsDailyFillAmount;
    public int DrinkPotionsDaily;
    [SerializeField] private int DrinkPotionsDailyGoal = 10;
    public Image DrinkPotionsDailyIconBackground, DrinkPotionsDailyIcon, DrinkPotionsDailyCoin, DrinkPotionsDailyBoughtIcon;
    public TextMeshProUGUI DrinkPotionsDailyText;
    internal bool drinkPotionsDailyCollected = false;

    // CurrentMoneyAmount (1 tier)
    public Image CurrentMoneyAmountSlider;
    public TextMeshProUGUI CurrentMoneyAmountFillAmount;
    public int CurrentMoneyAmount;
    [SerializeField] private int CurrentMoneyAmountGoal = 5000;
    public Image CurrentMoneyAmountIconBackground, CurrentMoneyAmountIcon, CurrentMoneyAmountCoin, CurrentMoneyAmountBoughtIcon;
    public TextMeshProUGUI CurrentMoneyAmountText;
    public TextMeshProUGUI CurrentMoneyAmountTMP;
    private bool currentMoneyAmountCollected = false;

    // Rewards & Audio
    public int[] TreesChoppedRewards = { 1, 20, 50, 100, 250 };
    public int[] UnlockAllAxesRewards = { 200 };
    public int[] ChopAllTreeTypesRewards = { 100 };
    public int[] HealFromInjuryRewards = { 50 };
    public int[] UsePotionsRewards = { 10, 50, 100 };
    public int[] UnlockAllUpgradesRewards = { 500 };
    public int[] SpendMoneyRewards = { 50, 200, 500 };
    public int[] InteractWithNPCsRewards = { 20, 40, 100 };
    public int[] DrinkPotionsDailyRewards = { 200 };
    public int[] CurrentMoneyAmountRewards = { 300 };
    public CurrencyManager currencyManager;
    public AudioSource AudioSource;
    public AudioClip AudioClip;

    private int dummyInt = 0;

    // -- Unity Lifecycle --

    private void Awake()
    {
        LoadAchievements();
    }

    private void OnDisable()
    {
        SaveAchievements();
    }

    private void OnApplicationQuit()
    {
        SaveAchievements();
    }

    private void Start()
    {
        currencyManager = FindObjectOfType<CurrencyManager>();
    }

    // -- Increase Methods (with one‑time checks) --

    public void increaseUnlockAllAxesCurrentGoal()
    {
        UnlockAllAxes++;
        if (!unlockAllAxesCollected && UnlockAllAxes >= UnlockAllAxesGoal)
        {
            unlockAllAxesCollected = true;
            currencyManager.AddMoney(UnlockAllAxesRewards[0]);
            AudioSource.PlayOneShot(AudioClip);
            UnlockAllAxesBoughtIcon.gameObject.SetActive(true);
            UnlockAllAxesIconBackground.GetComponent<Outline>().enabled = true;
            UnlockAllAxesCoin.gameObject.SetActive(false);
        }
    }

    public void increaseChopAllTreeTypesCurrentGoal()
    {
        ChopAllTreeTypes++;
        if (!chopAllTreeTypesCollected && ChopAllTreeTypes >= ChopAllTreeTypesGoal)
        {
            chopAllTreeTypesCollected = true;
            currencyManager.AddMoney(ChopAllTreeTypesRewards[0]);
            AudioSource.PlayOneShot(AudioClip);
            ChopAllTreeTypesBoughtIcon.gameObject.SetActive(true);
            ChopAllTreeTypesIconBackground.GetComponent<Outline>().enabled = true;
            ChopAllTreeTypesCoin.gameObject.SetActive(false);
        }
    }

    public void increaseHealFromInjuryCurrentGoal()
    {
        HealFromInjury++;
        if (!healFromInjuryCollected && HealFromInjury >= HealFromInjuryGoal)
        {
            healFromInjuryCollected = true;
            currencyManager.AddMoney(HealFromInjuryRewards[0]);
            AudioSource.PlayOneShot(AudioClip);
            HealFromInjuryBoughtIcon.gameObject.SetActive(true);
            HealFromInjuryIconBackground.GetComponent<Outline>().enabled = true;
            HealFromInjuryCoin.gameObject.SetActive(false);
        }
    }

    public void increaseUnlockAllUpgradesCurrentGoal()
    {
        UnlockAllUpgrades++;
        if (!unlockAllUpgradesCollected && UnlockAllUpgrades >= UnlockAllUpgradesGoal)
        {
            unlockAllUpgradesCollected = true;
            currencyManager.AddMoney(UnlockAllUpgradesRewards[0]);
            AudioSource.PlayOneShot(AudioClip);
            UnlockAllUpgradesBoughtIcon.gameObject.SetActive(true);
            UnlockAllUpgradesIconBackground.GetComponent<Outline>().enabled = true;
            UnlockAllUpgradesCoin.gameObject.SetActive(false);
        }
    }

    public void increaseDrinkPotionsDailyCurrentGoal()
    {
        if (!drinkPotionsDailyCollected)
        {
            DrinkPotionsDaily++;
            if (DrinkPotionsDaily >= DrinkPotionsDailyGoal)
            {
                drinkPotionsDailyCollected = true;
                currencyManager.AddMoney(DrinkPotionsDailyRewards[0]);
                AudioSource.PlayOneShot(AudioClip);
                DrinkPotionsDailyBoughtIcon.gameObject.SetActive(true);
                DrinkPotionsDailyIconBackground.GetComponent<Outline>().enabled = true;
                DrinkPotionsDailyCoin.gameObject.SetActive(false);
            }
        }
    }

    public void increaseCurrentMoneyAmountCurrentGoal()
    {
        if (!currentMoneyAmountCollected)
        {
            if (int.TryParse(CurrentMoneyAmountTMP.text, out int val))
                CurrentMoneyAmount = Mathf.Min(val, CurrentMoneyAmountGoal);

            if (CurrentMoneyAmount >= CurrentMoneyAmountGoal)
            {
                currentMoneyAmountCollected = true;
                currencyManager.AddMoney(CurrentMoneyAmountRewards[0]);
                AudioSource.PlayOneShot(AudioClip);
                CurrentMoneyAmountBoughtIcon.gameObject.SetActive(true);
                CurrentMoneyAmountIconBackground.GetComponent<Outline>().enabled = true;
                CurrentMoneyAmountCoin.gameObject.SetActive(false);
            }
        }
    }

    public void increaseChoppedTreesCurrentGoal()
    {
        if (TreesChoppedCurrentLevel < TreesChoppedLevels.Length)
        {
            TreesChopped++;
            TreesChoppedCurrentLevel++;
            int idx = TreesChoppedCurrentLevel - 1;
            if (!treesChoppedCollected[idx] && TreesChopped >= TreesChoppedLevels[idx])
            {
                treesChoppedCollected[idx] = true;
                currencyManager.AddMoney(TreesChoppedRewards[idx]);
                AudioSource.PlayOneShot(AudioClip);

                if (idx == TreesChoppedLevels.Length - 1)
                {
                    TreesChoppedBoughtIcon.gameObject.SetActive(true);
                    TreesChoppedIconBackground.GetComponent<Outline>().enabled = true;
                    TreesChoppedCoin.gameObject.SetActive(false);
                }
                else
                {
                    TreesChoppedIcon.gameObject.SetActive(true);
                }
            }
        }
    }

    public void increaseUsePotionsCurrentGoal()
    {
        if (UsePotionsCurrentLevel < UsePotionsLevel.Length)
        {
            UsePotions++;
            UsePotionsCurrentLevel++;
            int idx = UsePotionsCurrentLevel - 1;
            if (!usePotionsCollected[idx] && UsePotions >= UsePotionsLevel[idx])
            {
                usePotionsCollected[idx] = true;
                currencyManager.AddMoney(UsePotionsRewards[idx]);
                AudioSource.PlayOneShot(AudioClip);

                if (idx == UsePotionsLevel.Length - 1)
                {
                    UsePotionsBoughtIcon.gameObject.SetActive(true);
                    UsePotionsIconBackground.GetComponent<Outline>().enabled = true;
                    UsePotionsCoin.gameObject.SetActive(false);
                }
                else
                {
                    UsePotionsIcon.gameObject.SetActive(true);
                }
            }
        }
    }

    public void increaseSpendMoneyCurrentGoal()
    {
        if (SpendMoneyCurrentLevel < SpendMoneyLevel.Length)
        {
            SpendMoney++;
            SpendMoneyCurrentLevel++;
            int idx = SpendMoneyCurrentLevel - 1;
            if (!spendMoneyCollected[idx] && SpendMoney >= SpendMoneyLevel[idx])
            {
                spendMoneyCollected[idx] = true;
                currencyManager.AddMoney(SpendMoneyRewards[idx]);
                AudioSource.PlayOneShot(AudioClip);

                if (idx == SpendMoneyLevel.Length - 1)
                {
                    SpendMoneyBoughtIcon.gameObject.SetActive(true);
                    SpendMoneyIconBackground.GetComponent<Outline>().enabled = true;
                    SpendMoneyCoin.gameObject.SetActive(false);
                }
                else
                {
                    SpendMoneyIcon.gameObject.SetActive(true);
                }
            }
        }
    }

    public void increaseInteractWithNPCsCurrentGoal()
    {
        if (InteractWithNPCsCurrentLevel < InteractWithNPCSLevel.Length)
        {
            InteractWithNPCs++;
            InteractWithNPCsCurrentLevel++;
            int idx = InteractWithNPCsCurrentLevel - 1;
            if (!interactWithNPCsCollected[idx] && InteractWithNPCs >= InteractWithNPCSLevel[idx])
            {
                interactWithNPCsCollected[idx] = true;
                currencyManager.AddMoney(InteractWithNPCsRewards[idx]);
                AudioSource.PlayOneShot(AudioClip);

                if (idx == InteractWithNPCSLevel.Length - 1)
                {
                    InteractWithNPCsBoughtIcon.gameObject.SetActive(true);
                    InteractWithNPCsIconBackground.GetComponent<Outline>().enabled = true;
                    InteractWithNPCsCoin.gameObject.SetActive(false);
                }
                else
                {
                    InteractWithNPCsIcon.gameObject.SetActive(true);
                }
            }
        }
    }

    // -- Slider Update --

    public void UpdateSliderFill(int currentAmount, int goalAmount, Image slider, TextMeshProUGUI text,
        ref int currentLevel, int[] array = null,
        Image icon = null, Image background = null, Image coin = null, Image bought = null)
    {
        int displayGoal = (array != null && currentLevel < array.Length) ? array[currentLevel] : goalAmount;
        float fill = Mathf.Clamp01((float)currentAmount / displayGoal);
        slider.fillAmount = fill;
        text.text = $"{Math.Min(currentAmount, displayGoal)}/{displayGoal}";
        if (currentAmount >= displayGoal)
        {
            icon?.gameObject.SetActive(false);
            coin?.gameObject.SetActive(true);
            if (background != null)
            {
                background.GetComponent<Outline>().enabled = true;
            }
        }
    }

    private void Update()
    {
        // Sync CurrentMoneyAmount from TMP
        if (int.TryParse(CurrentMoneyAmountTMP.text, out int parsed))
            CurrentMoneyAmount = Mathf.Min(parsed, CurrentMoneyAmountGoal);

        // Refresh each slider
        UpdateSliderFill(TreesChopped, TreesChoppedGoal, TreesChoppedSlider, TreesChoppedFillAmount,
            ref TreesChoppedCurrentLevel, TreesChoppedLevels,
            TreesChoppedIcon, TreesChoppedIconBackground, TreesChoppedCoin, TreesChoppedBoughtIcon);

        UpdateSliderFill(UnlockAllAxes, UnlockAllAxesGoal, UnlockAllAxesSlider, UnlockAllAxesFillAmount,
            ref dummyInt, null,
            UnlockAllAxesIcon, UnlockAllAxesIconBackground, UnlockAllAxesCoin, UnlockAllAxesBoughtIcon);

        UpdateSliderFill(ChopAllTreeTypes, ChopAllTreeTypesGoal, ChopAllTreeTypesSlider, ChopAllTreeTypesFillAmount,
            ref dummyInt, null,
            ChopAllTreeTypesIcon, ChopAllTreeTypesIconBackground, ChopAllTreeTypesCoin, ChopAllTreeTypesBoughtIcon);

        UpdateSliderFill(HealFromInjury, HealFromInjuryGoal, HealFromInjurySlider, HealFromInjuryFillAmount,
            ref dummyInt, null,
            HealFromInjuryIcon, HealFromInjuryIconBackground, HealFromInjuryCoin, HealFromInjuryBoughtIcon);

        UpdateSliderFill(UsePotions, UsePotionsGoal, UsePotionsSlider, UsePotionsFillAmount,
            ref UsePotionsCurrentLevel, UsePotionsLevel,
            UsePotionsIcon, UsePotionsIconBackground, UsePotionsCoin, UsePotionsBoughtIcon);

        UpdateSliderFill(UnlockAllUpgrades, UnlockAllUpgradesGoal, UnlockAllUpgradesSlider, UnlockAllUpgradesFillAmount,
            ref dummyInt, null,
            UnlockAllUpgradesIcon, UnlockAllUpgradesIconBackground, UnlockAllUpgradesCoin, UnlockAllUpgradesBoughtIcon);

        UpdateSliderFill(SpendMoney, SpendMoneyGoal, SpendMoneySlider, SpendMoneyFillAmount,
            ref SpendMoneyCurrentLevel, SpendMoneyLevel,
            SpendMoneyIcon, SpendMoneyIconBackground, SpendMoneyCoin, SpendMoneyBoughtIcon);

        UpdateSliderFill(InteractWithNPCs, InteractWithNPCsGoal, InteractWithNPCsSlider, InteractWithNPCsFillAmount,
            ref InteractWithNPCsCurrentLevel, InteractWithNPCSLevel,
            InteractWithNPCsIcon, InteractWithNPCsIconBackground, InteractWithNPCsCoin, InteractWithNPCsBoughtIcon);

        UpdateSliderFill(DrinkPotionsDaily, DrinkPotionsDailyGoal, DrinkPotionsDailySlider, DrinkPotionsDailyFillAmount,
            ref dummyInt, null,
            DrinkPotionsDailyIcon, DrinkPotionsDailyIconBackground, DrinkPotionsDailyCoin, DrinkPotionsDailyBoughtIcon);

        UpdateSliderFill(CurrentMoneyAmount, CurrentMoneyAmountGoal, CurrentMoneyAmountSlider, CurrentMoneyAmountFillAmount,
            ref dummyInt, null,
            CurrentMoneyAmountIcon, CurrentMoneyAmountIconBackground, CurrentMoneyAmountCoin, CurrentMoneyAmountBoughtIcon);
    }

    // -- Persistence --

    public void SaveAchievements()
    {
        // Basic counts and levels
        PlayerPrefs.SetInt(Prefix + "TreesChopped", TreesChopped);
        PlayerPrefs.SetInt(Prefix + "TreesChoppedLevel", TreesChoppedCurrentLevel);
        PlayerPrefs.SetInt(Prefix + "UnlockAllAxes", UnlockAllAxes);
        PlayerPrefs.SetInt(Prefix + "ChopAllTreeTypes", ChopAllTreeTypes);
        PlayerPrefs.SetInt(Prefix + "HealFromInjury", HealFromInjury);
        PlayerPrefs.SetInt(Prefix + "UsePotions", UsePotions);
        PlayerPrefs.SetInt(Prefix + "UsePotionsLevel", UsePotionsCurrentLevel);
        PlayerPrefs.SetInt(Prefix + "UnlockAllUpgrades", UnlockAllUpgrades);
        PlayerPrefs.SetInt(Prefix + "SpendMoney", SpendMoney);
        PlayerPrefs.SetInt(Prefix + "SpendMoneyLevel", SpendMoneyCurrentLevel);
        PlayerPrefs.SetInt(Prefix + "InteractWithNPCs", InteractWithNPCs);
        PlayerPrefs.SetInt(Prefix + "InteractWithNPCsLevel", InteractWithNPCsCurrentLevel);
        PlayerPrefs.SetInt(Prefix + "DrinkPotionsDaily", DrinkPotionsDaily);
        PlayerPrefs.SetInt(Prefix + "CurrentMoneyAmount", CurrentMoneyAmount);

        // Tree‑type counters
        PlayerPrefs.SetInt(Prefix + "TreeType1", TreeType1);
        PlayerPrefs.SetInt(Prefix + "TreeType2", TreeType2);
        PlayerPrefs.SetInt(Prefix + "TreeType3", TreeType3);
        PlayerPrefs.SetInt(Prefix + "TreeType4", TreeType4);
        PlayerPrefs.SetInt(Prefix + "TreeType5", TreeType5);

        // Collected flags: single‑tier
        PlayerPrefs.SetInt(Prefix + "UnlockAllAxesCollected", unlockAllAxesCollected ? 1 : 0);
        PlayerPrefs.SetInt(Prefix + "ChopAllTreeTypesCollected", chopAllTreeTypesCollected ? 1 : 0);
        PlayerPrefs.SetInt(Prefix + "HealFromInjuryCollected", healFromInjuryCollected ? 1 : 0);
        PlayerPrefs.SetInt(Prefix + "UnlockAllUpgradesCollected", unlockAllUpgradesCollected ? 1 : 0);
        PlayerPrefs.SetInt(Prefix + "DrinkPotionsDailyCollected", drinkPotionsDailyCollected ? 1 : 0);
        PlayerPrefs.SetInt(Prefix + "CurrentMoneyAmountCollected", currentMoneyAmountCollected ? 1 : 0);

        // Collected flags: multi‑tier
        for (int i = 0; i < treesChoppedCollected.Length; i++)
            PlayerPrefs.SetInt(Prefix + $"TreesChoppedCollected{i}", treesChoppedCollected[i] ? 1 : 0);

        for (int i = 0; i < usePotionsCollected.Length; i++)
            PlayerPrefs.SetInt(Prefix + $"UsePotionsCollected{i}", usePotionsCollected[i] ? 1 : 0);

        for (int i = 0; i < spendMoneyCollected.Length; i++)
            PlayerPrefs.SetInt(Prefix + $"SpendMoneyCollected{i}", spendMoneyCollected[i] ? 1 : 0);

        for (int i = 0; i < interactWithNPCsCollected.Length; i++)
            PlayerPrefs.SetInt(Prefix + $"InteractWithNPCsCollected{i}", interactWithNPCsCollected[i] ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void LoadAchievements()
    {
        // Basic counts and levels
        TreesChopped = PlayerPrefs.GetInt(Prefix + "TreesChopped", TreesChopped);
        TreesChoppedCurrentLevel = PlayerPrefs.GetInt(Prefix + "TreesChoppedLevel", TreesChoppedCurrentLevel);
        UnlockAllAxes = PlayerPrefs.GetInt(Prefix + "UnlockAllAxes", UnlockAllAxes);
        ChopAllTreeTypes = PlayerPrefs.GetInt(Prefix + "ChopAllTreeTypes", ChopAllTreeTypes);
        HealFromInjury = PlayerPrefs.GetInt(Prefix + "HealFromInjury", HealFromInjury);
        UsePotions = PlayerPrefs.GetInt(Prefix + "UsePotions", UsePotions);
        UsePotionsCurrentLevel = PlayerPrefs.GetInt(Prefix + "UsePotionsLevel", UsePotionsCurrentLevel);
        UnlockAllUpgrades = PlayerPrefs.GetInt(Prefix + "UnlockAllUpgrades", UnlockAllUpgrades);
        SpendMoney = PlayerPrefs.GetInt(Prefix + "SpendMoney", SpendMoney);
        SpendMoneyCurrentLevel = PlayerPrefs.GetInt(Prefix + "SpendMoneyLevel", SpendMoneyCurrentLevel);
        InteractWithNPCs = PlayerPrefs.GetInt(Prefix + "InteractWithNPCs", InteractWithNPCs);
        InteractWithNPCsCurrentLevel = PlayerPrefs.GetInt(Prefix + "InteractWithNPCsLevel", InteractWithNPCsCurrentLevel);
        DrinkPotionsDaily = PlayerPrefs.GetInt(Prefix + "DrinkPotionsDaily", DrinkPotionsDaily);
        CurrentMoneyAmount = PlayerPrefs.GetInt(Prefix + "CurrentMoneyAmount", CurrentMoneyAmount);

        // Tree‑type counters
        TreeType1 = PlayerPrefs.GetInt(Prefix + "TreeType1", TreeType1);
        TreeType2 = PlayerPrefs.GetInt(Prefix + "TreeType2", TreeType2);
        TreeType3 = PlayerPrefs.GetInt(Prefix + "TreeType3", TreeType3);
        TreeType4 = PlayerPrefs.GetInt(Prefix + "TreeType4", TreeType4);
        TreeType5 = PlayerPrefs.GetInt(Prefix + "TreeType5", TreeType5);

        // Collected flags: single‑tier
        unlockAllAxesCollected       = PlayerPrefs.GetInt(Prefix + "UnlockAllAxesCollected", 0) == 1;
        chopAllTreeTypesCollected    = PlayerPrefs.GetInt(Prefix + "ChopAllTreeTypesCollected", 0) == 1;
        healFromInjuryCollected      = PlayerPrefs.GetInt(Prefix + "HealFromInjuryCollected", 0) == 1;
        unlockAllUpgradesCollected   = PlayerPrefs.GetInt(Prefix + "UnlockAllUpgradesCollected", 0) == 1;
        drinkPotionsDailyCollected   = PlayerPrefs.GetInt(Prefix + "DrinkPotionsDailyCollected", 0) == 1;
        currentMoneyAmountCollected  = PlayerPrefs.GetInt(Prefix + "CurrentMoneyAmountCollected", 0) == 1;

        // Collected flags: multi‑tier
        for (int i = 0; i < treesChoppedCollected.Length; i++)
            treesChoppedCollected[i] = PlayerPrefs.GetInt(Prefix + $"TreesChoppedCollected{i}", 0) == 1;

        for (int i = 0; i < usePotionsCollected.Length; i++)
            usePotionsCollected[i] = PlayerPrefs.GetInt(Prefix + $"UsePotionsCollected{i}", 0) == 1;

        for (int i = 0; i < spendMoneyCollected.Length; i++)
            spendMoneyCollected[i] = PlayerPrefs.GetInt(Prefix + $"SpendMoneyCollected{i}", 0) == 1;

        for (int i = 0; i < interactWithNPCsCollected.Length; i++)
            interactWithNPCsCollected[i] = PlayerPrefs.GetInt(Prefix + $"InteractWithNPCsCollected{i}", 0) == 1;

        // Restore UI for already‑collected tiers:
        if (unlockAllAxesCollected)
            UnlockAllAxesBoughtIcon.gameObject.SetActive(true);

        if (chopAllTreeTypesCollected)
            ChopAllTreeTypesBoughtIcon.gameObject.SetActive(true);

        if (healFromInjuryCollected)
            HealFromInjuryBoughtIcon.gameObject.SetActive(true);

        if (unlockAllUpgradesCollected)
            UnlockAllUpgradesBoughtIcon.gameObject.SetActive(true);

        if (drinkPotionsDailyCollected)
            DrinkPotionsDailyBoughtIcon.gameObject.SetActive(true);

        if (currentMoneyAmountCollected)
            CurrentMoneyAmountBoughtIcon.gameObject.SetActive(true);

        for (int i = 0; i < treesChoppedCollected.Length; i++)
            if (treesChoppedCollected[i] && i == treesChoppedCollected.Length - 1)
                TreesChoppedBoughtIcon.gameObject.SetActive(true);

        for (int i = 0; i < usePotionsCollected.Length; i++)
            if (usePotionsCollected[i] && i == usePotionsCollected.Length - 1)
                UsePotionsBoughtIcon.gameObject.SetActive(true);

        for (int i = 0; i < spendMoneyCollected.Length; i++)
            if (spendMoneyCollected[i] && i == spendMoneyCollected.Length - 1)
                SpendMoneyBoughtIcon.gameObject.SetActive(true);

        for (int i = 0; i < interactWithNPCsCollected.Length; i++)
            if (interactWithNPCsCollected[i] && i == interactWithNPCsCollected.Length - 1)
                InteractWithNPCsBoughtIcon.gameObject.SetActive(true);
    }
}
