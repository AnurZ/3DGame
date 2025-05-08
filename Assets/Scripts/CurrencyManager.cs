using System;
using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public int CurrentMoney { get; set; } = 150;
    public TMP_Text moneyText;
    public static CurrencyManager Instance { get; private set; }
    public AchievementsController achievementsController;

    public void Start()
    {
        achievementsController = FindObjectOfType<AchievementsController>();
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    
    public void AddMoney(int amount)
    {
        CurrentMoney += amount;
        Debug.Log("Dodano novca: " + amount + " | Trenutno: " + CurrentMoney);
        SetMoneyText(); 
    }
    
    public void SetMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = "" + CurrentMoney;
        }
    }

    public bool TrySpendMoney(int amount)
    {
        if (CurrentMoney >= amount)
        {
            CurrentMoney -= amount;
            Debug.Log("Potrošeno: " + amount + " | Preostalo: " + CurrentMoney);
            if(achievementsController != null)
                achievementsController.SpendMoney += amount;
            else
            {
            }
            return true;
        }
        else
        {
            Debug.Log("Nema dovoljno novca!");
            return false;
        }
    }
    
    
}