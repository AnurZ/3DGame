using TMPro;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public int CurrentMoney { get; set; } = 150;
    public CurrencyManager currencyManager;
    public TMP_Text moneyText;
    public static CurrencyManager Instance { get; private set; }

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
            moneyText.text = "Money: $" + CurrentMoney;
        }
    }

    public bool TrySpendMoney(int amount)
    {
        if (CurrentMoney >= amount)
        {
            CurrentMoney -= amount;
            Debug.Log("Potrošeno: " + amount + " | Preostalo: " + CurrentMoney);
            return true;
        }
        else
        {
            Debug.Log("Nema dovoljno novca!");
            return false;
        }
    }
    
    
}