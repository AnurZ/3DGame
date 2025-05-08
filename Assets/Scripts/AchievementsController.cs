using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementsController : MonoBehaviour
{
    // TreesChoppedAmount
    public Image TreesChoppedSlider;
    public TextMeshProUGUI TreesChoppedFillAmount;
    public int TreesChopped;
    [SerializeField] private int TreesChoppedGoal = 5000;
    public Image TreesChoppedIconBackground;
    public Image TreesChoppedIcon;
    public Image TreesChoppedCoin;
    public Image TreesChoppedBoughtIcon;
    public TextMeshProUGUI TreesChoppedText;
    public int[] TreesChoppedLevels = new int[5] { 1, 100, 500, 1000, 5000 };
    public int TreesChoppedCurrentLevel = 0;

    // UnlockAllAxes
    public Image UnlockAllAxesSlider;
    public TextMeshProUGUI UnlockAllAxesFillAmount;
    public int UnlockAllAxes = 0;
    [SerializeField] private int UnlockAllAxesGoal = 4;
    public Image UnlockAllAxesIconBackground;
    public Image UnlockAllAxesIcon;
    public Image UnlockAllAxesCoin;
    public Image UnlockAllAxesBoughtIcon;
    public TextMeshProUGUI UnlockAllAxesText;

    // ChopAllTreeTypes
    public Image ChopAllTreeTypesSlider;
    public TextMeshProUGUI ChopAllTreeTypesFillAmount;
    public int ChopAllTreeTypes = 0;
    [SerializeField] private int ChopAllTreeTypesGoal = 5;
    public Image ChopAllTreeTypesIconBackground;
    public Image ChopAllTreeTypesIcon;
    public Image ChopAllTreeTypesCoin;
    public Image ChopAllTreeTypesBoughtIcon;
    public TextMeshProUGUI ChopAllTreeTypesText;
    public int TreeType1 = 0;
    public int TreeType2 = 0;
    public int TreeType3 = 0;
    public int TreeType4 = 0;
    public int TreeType5 = 0;

    // HealFromInjury
    public Image HealFromInjurySlider;
    public TextMeshProUGUI HealFromInjuryFillAmount;
    public int HealFromInjury = 0;
    [SerializeField] private int HealFromInjuryGoal = 1;
    public Image HealFromInjuryIconBackground;
    public Image HealFromInjuryIcon;
    public Image HealFromInjuryCoin;
    public Image HealFromInjuryBoughtIcon;
    public TextMeshProUGUI HealFromInjuryText;

    // UsePotions
    public Image UsePotionsSlider;
    public TextMeshProUGUI UsePotionsFillAmount;
    public int UsePotions = 0;
    [SerializeField] private int UsePotionsGoal = 20;
    public Image UsePotionsIconBackground;
    public Image UsePotionsIcon;
    public Image UsePotionsCoin;
    public Image UsePotionsBoughtIcon;
    public TextMeshProUGUI UsePotionsText;
    public int[] UsePotionsLevel = new int[3] { 1, 10, 20 };
    public int UsePotionsCurrentLevel = 0;

    // UnlockAllUpgrades
    public Image UnlockAllUpgradesSlider;
    public TextMeshProUGUI UnlockAllUpgradesFillAmount;
    public int UnlockAllUpgrades = 0;
    [SerializeField] private int UnlockAllUpgradesGoal = 8;
    public Image UnlockAllUpgradesIconBackground;
    public Image UnlockAllUpgradesIcon;
    public Image UnlockAllUpgradesCoin;
    public Image UnlockAllUpgradesBoughtIcon;
    public TextMeshProUGUI UnlockAllUpgradesText;

    // SpendMoney
    public Image SpendMoneySlider;
    public TextMeshProUGUI SpendMoneyFillAmount;
    public int SpendMoney = 0;
    [SerializeField] private int SpendMoneyGoal = 10000;
    public Image SpendMoneyIconBackground;
    public Image SpendMoneyIcon;
    public Image SpendMoneyCoin;
    public Image SpendMoneyBoughtIcon;
    public TextMeshProUGUI SpendMoneyText;
    public int[] SpendMoneyLevel = new int[3] { 500, 5000, 10000 };
    public int SpendMoneyCurrentLevel = 0;

    // InteractWithNPCs
    public Image InteractWithNPCsSlider;
    public TextMeshProUGUI InteractWithNPCsFillAmount;
    public int InteractWithNPCs = 0;
    [SerializeField] private int InteractWithNPCsGoal = 50;
    public Image InteractWithNPCsIconBackground;
    public Image InteractWithNPCsIcon;
    public Image InteractWithNPCsCoin;
    public Image InteractWithNPCsBoughtIcon;
    public TextMeshProUGUI InteractWithNPCsText;
    public int[] InteractWithNPCSLevel = new int[3] { 10, 20, 50 };
    public int InteractWithNPCsCurrentLevel = 0;
    
    private int dummyInt = 0;

    public void increaseChoppedTreesCurrentGoal()
    {
        TreesChoppedCurrentLevel++;
        if (TreesChopped != TreesChoppedGoal)
        {
            TreesChoppedIcon.gameObject.SetActive(true);
        }
        else if(TreesChoppedCurrentLevel == 5)
        {
            TreesChoppedBoughtIcon.gameObject.SetActive(true);
            Outline outline = TreesChoppedIconBackground.GetComponent<Outline>();
            outline.enabled = true;
            TreesChoppedCoin.gameObject.SetActive(false);
        }
    }

    public void increaseUsePotionsCurrentGoal()
    {
        UsePotionsCurrentLevel++;
        if (UsePotions != UsePotionsGoal)
        {
            UsePotionsIcon.gameObject.SetActive(true);
        }
        else if(UsePotionsCurrentLevel == 3)
        {
            UsePotionsBoughtIcon.gameObject.SetActive(true);
            Outline outline = UsePotionsIconBackground.GetComponent<Outline>();
            outline.enabled = true;
            UsePotionsCoin.gameObject.SetActive(false);
        }
    }

    public void increaseSpendMoneyCurrentGoal()
    {
        SpendMoneyCurrentLevel++;
        if (SpendMoney != SpendMoneyGoal)
        {
            SpendMoneyIcon.gameObject.SetActive(true);
        }
        else if(SpendMoneyCurrentLevel == 3)
        {
            SpendMoneyBoughtIcon.gameObject.SetActive(true);
            Outline outline = SpendMoneyIconBackground.GetComponent<Outline>();
            outline.enabled = true;
            SpendMoneyCoin.gameObject.SetActive(false);
        }
    }

    public void increaseInteractWithNPCsCurrentGoal()
    {
        InteractWithNPCsCurrentLevel++;
        if (InteractWithNPCs != InteractWithNPCsGoal)
        {
            InteractWithNPCsIcon.gameObject.SetActive(true);
        }
        else if(InteractWithNPCsCurrentLevel == 3)
        {
            InteractWithNPCsBoughtIcon.gameObject.SetActive(true);
            Outline outline = InteractWithNPCsIconBackground.GetComponent<Outline>();
            outline.enabled = true;
            InteractWithNPCsCoin.gameObject.SetActive(false);
        }
    }

    
    public void UpdateSliderFill(int currentAmount, int goalAmount, Image slider, TextMeshProUGUI text,  ref int currentLevel, int[] array = null,
        Image icon = null, Image background = null, Image coin = null, Image bought = null)
    {
        int displayGoal = goalAmount;

        if (array != null && currentLevel >= 0 && currentLevel < array.Length)
        {
            displayGoal = array[currentLevel];
        }

        if (currentAmount >= displayGoal)
        {
            icon.gameObject.SetActive(false);
            coin.gameObject.SetActive(true);
            Debug.Log("Current amount: " + currentAmount + "Display amount: " + displayGoal + "Goal amount: " + goalAmount);
                Outline outline = background.gameObject.GetComponent<Outline>();
            if (goalAmount == currentAmount)
            {
                outline.enabled = true;
            }
            else
            {
                outline.enabled = false;
            }
            
        }
        
        text.text = $"{currentAmount}/{displayGoal}";
        slider.fillAmount = Mathf.Clamp01((float)currentAmount / displayGoal);
    }

    public void Update()
{
    UpdateSliderFill(
        TreesChopped,
        TreesChoppedGoal,
        TreesChoppedSlider,
        TreesChoppedFillAmount,
        ref TreesChoppedCurrentLevel,
        TreesChoppedLevels,
        TreesChoppedIcon,
        TreesChoppedIconBackground,
        TreesChoppedCoin,
        TreesChoppedBoughtIcon
    );

    UpdateSliderFill(
        UnlockAllAxes,
        UnlockAllAxesGoal,
        UnlockAllAxesSlider,
        UnlockAllAxesFillAmount,
        ref dummyInt, // no levels for this, you can define dummyInt = 0 above if needed
        null,
        UnlockAllAxesIcon,
        UnlockAllAxesIconBackground,
        UnlockAllAxesCoin,
        UnlockAllAxesBoughtIcon
    );

    UpdateSliderFill(
        ChopAllTreeTypes,
        ChopAllTreeTypesGoal,
        ChopAllTreeTypesSlider,
        ChopAllTreeTypesFillAmount,
        ref dummyInt,
        null,
        ChopAllTreeTypesIcon,
        ChopAllTreeTypesIconBackground,
        ChopAllTreeTypesCoin,
        ChopAllTreeTypesBoughtIcon
    );

    UpdateSliderFill(
        HealFromInjury,
        HealFromInjuryGoal,
        HealFromInjurySlider,
        HealFromInjuryFillAmount,
        ref dummyInt,
        null,
        HealFromInjuryIcon,
        HealFromInjuryIconBackground,
        HealFromInjuryCoin,
        HealFromInjuryBoughtIcon
    );

    UpdateSliderFill(
        UsePotions,
        UsePotionsGoal,
        UsePotionsSlider,
        UsePotionsFillAmount,
        ref UsePotionsCurrentLevel,
        UsePotionsLevel,
        UsePotionsIcon,
        UsePotionsIconBackground,
        UsePotionsCoin,
        UsePotionsBoughtIcon
    );

    UpdateSliderFill(
        UnlockAllUpgrades,
        UnlockAllUpgradesGoal,
        UnlockAllUpgradesSlider,
        UnlockAllUpgradesFillAmount,
        ref dummyInt,
        null,
        UnlockAllUpgradesIcon,
        UnlockAllUpgradesIconBackground,
        UnlockAllUpgradesCoin,
        UnlockAllUpgradesBoughtIcon
    );

    UpdateSliderFill(
        SpendMoney,
        SpendMoneyGoal,
        SpendMoneySlider,
        SpendMoneyFillAmount,
        ref SpendMoneyCurrentLevel,
        SpendMoneyLevel,
        SpendMoneyIcon,
        SpendMoneyIconBackground,
        SpendMoneyCoin,
        SpendMoneyBoughtIcon
    );

    UpdateSliderFill(
        InteractWithNPCs,
        InteractWithNPCsGoal,
        InteractWithNPCsSlider,
        InteractWithNPCsFillAmount,
        ref InteractWithNPCsCurrentLevel,
        InteractWithNPCSLevel,
        InteractWithNPCsIcon,
        InteractWithNPCsIconBackground,
        InteractWithNPCsCoin,
        InteractWithNPCsBoughtIcon
    );
}


}
