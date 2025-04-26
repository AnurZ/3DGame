using UnityEngine;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public TMP_Text moneyText;
    public InventoryManager inventoryManager;
    public Item woodItem; // Poveži prefab itema za drvo
    public int woodPrice = 10;
    public CurrencyManager currencyManager;

    void Update()
    {
        moneyText.text = "" + CurrencyManager.Instance.CurrentMoney;

    }

    public void SellWood()
    {
        int count = 0;

        for (int i = 0; i < inventoryManager.inventorySlots.Length; i++)
        {
            var slot = inventoryManager.inventorySlots[i];
            var itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == woodItem)
            {
                count += itemInSlot.count;
                Destroy(itemInSlot.gameObject);
            }
        }

        if (count > 0)
        {
            CurrencyManager.Instance.AddMoney(count * woodPrice);
        }
    }

    public void BuyAxe(Item axeItem, int price)
    {
        if (CurrencyManager.Instance.TrySpendMoney(price))
        {
            inventoryManager.AddItem(axeItem);
            MoneyPopupManager.Instance.ShowPopup(-price, Input.mousePosition); 
        }
    }
}