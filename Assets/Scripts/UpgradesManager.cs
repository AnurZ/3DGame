using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesManager : MonoBehaviour
{
    
    public PlayerController playerController;
    public StaminaController staminaController;
    
    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        staminaController = FindObjectOfType<StaminaController>();
    }
    
    //staminaRegen
    public Image staminaRegenBackground;
    public Image staminaRegenIcon;
    public Image staminaRegenInfoBackground;
    public GameObject staminaRegenBuyButton;
    
    public void BuyStaminaRegenUpgrade()
    {
        Color purchasedColor = new Color32(0xc1, 0xc1, 0xc1, 0xff);
        staminaRegenBuyButton.GetComponent<Image>().color = purchasedColor;
        staminaRegenBackground.color = purchasedColor;
        staminaRegenIcon.color = purchasedColor;
        staminaRegenInfoBackground.color = purchasedColor;
        Debug.Log("Buying Stamina");
    }

    //speed
    public Image speedBackground;
    public Image speedIcon;
    public Image speedInfoBackground;
    public GameObject speedBuyButton;

    public void BuySpeedUpgrade()
    {
        Color purchasedColor = new Color32(0xc1, 0xc1, 0xc1, 0xff);
        speedBuyButton.GetComponent<Image>().color = purchasedColor;
        speedBackground.color = purchasedColor;
        speedIcon.color = purchasedColor;
        speedInfoBackground.color = purchasedColor;
        playerController.speed = 10f;
        Debug.Log("Buying Speed");
    }

    //injuryShield
    public Image injuryShieldBackground;
    public Image injuryShieldIcon;
    public Image injuryShieldInfoBackground;
    public GameObject injuryShieldBuyButton;

    public void BuyInjuryShieldUpgrade()
    {
        playerController.injuryShieldUpgrade = true;
        Color purchasedColor = new Color32(0xc1, 0xc1, 0xc1, 0xff);
        injuryShieldBuyButton.GetComponent<Image>().color = purchasedColor;
        injuryShieldBackground.color = purchasedColor;
        injuryShieldIcon.color = purchasedColor;
        injuryShieldInfoBackground.color = purchasedColor;
        Debug.Log("Buying Injury Shield");
    }

    //severeInjuryShield
    public Image severeInjuryShieldBackground;
    public Image severeInjuryShieldIcon;
    public Image severeInjuryShieldInfoBackground;
    public GameObject severeInjuryShieldBuyButton;

    public void BuySevereInjuryShieldUpgrade()
    {
        playerController.severeInjuryShieldUpgrade = true;
        Color purchasedColor = new Color32(0xc1, 0xc1, 0xc1, 0xff);
        severeInjuryShieldBuyButton.GetComponent<Image>().color = purchasedColor;
        severeInjuryShieldBackground.color = purchasedColor;
        severeInjuryShieldIcon.color = purchasedColor;
        severeInjuryShieldInfoBackground.color = purchasedColor;
        Debug.Log("Buying Severe Injury Shield");
    }

    //potionEffect
    public Image potionEffectBackground;
    public Image potionEffectIcon;
    public Image potionEffectInfoBackground;
    public GameObject potionEffectBuyButton;

    public void BuyPotionEffectUpgrade()
    {
        Color purchasedColor = new Color32(0xc1, 0xc1, 0xc1, 0xff);
        potionEffectBuyButton.GetComponent<Image>().color = purchasedColor;
        potionEffectBackground.color = purchasedColor;
        potionEffectIcon.color = purchasedColor;
        potionEffectInfoBackground.color = purchasedColor;
        Debug.Log("Buying Potion Effect");
    }

    //choppingSpeed
    public Image choppingSpeedBackground;
    public Image choppingSpeedIcon;
    public Image choppingSpeedInfoBackground;
    public GameObject choppingSpeedBuyButton;

    public void BuyChoppingSpeedUpgrade()
    {
        playerController.choppingSpeedUpgrade = true;
        Color purchasedColor = new Color32(0xc1, 0xc1, 0xc1, 0xff);
        choppingSpeedBuyButton.GetComponent<Image>().color = purchasedColor;
        choppingSpeedBackground.color = purchasedColor;
        choppingSpeedIcon.color = purchasedColor;
        choppingSpeedInfoBackground.color = purchasedColor;
        Debug.Log("Buying Chopping Speed");
    }

    //choppingStamina
    public Image choppingStaminaBackground;
    public Image choppingStaminaIcon;
    public Image choppingStaminaInfoBackground;
    public GameObject choppingStaminaBuyButton;

    public void BuyChoppingStaminaUpgrade()
    {
        staminaController.staminaReductionRate *= 0.8f;
        Color purchasedColor = new Color32(0xc1, 0xc1, 0xc1, 0xff);
        choppingStaminaBuyButton.GetComponent<Image>().color = purchasedColor;
        choppingStaminaBackground.color = purchasedColor;
        choppingStaminaIcon.color = purchasedColor;
        choppingStaminaInfoBackground.color = purchasedColor;
        Debug.Log("Buying Chopping Stamina");
    }

}
