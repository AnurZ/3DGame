using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Collider))]
public class SellWoodInteractable : MonoBehaviour, IShopInteractable
{
    [System.Serializable]
    public struct WoodPrice
    {
        public Item woodItem;
        public int pricePerUnit;
    }

    [Header("Sell Settings")]
    public WoodPrice[] woodPrices;

    [Header("Colors")]
    public Color hoverColor = Color.yellow;
    public Color soldColor = Color.green;

    private Renderer[] rends;
    private Color[] originalColors;

    private void Start()
    {
        rends = GetComponentsInChildren<Renderer>();
        originalColors = rends.Select(r => r.material.color).ToArray();
    }

    // IShopInteractable implementation
    public void OnHoverEnter()
    {
        SetAllColors(hoverColor);
    }

    public void OnHoverExit()
    {
        ResetColors();
    }

    public void OnActivate()
    {
        TrySell();
    }

    // Actual sell logic
    private void TrySell()
    {
        var inv = InventoryManager.Instance;
        int totalEarned = 0;

        foreach (var wp in woodPrices)
        {
            int amountSold = inv.RemoveAllOfItem(wp.woodItem);
            if (amountSold > 0)
            {
                totalEarned += amountSold * wp.pricePerUnit;
            }
        }

        if (totalEarned > 0)
        {
            CurrencyManager.Instance.AddMoney(totalEarned);
            SetAllColors(soldColor);
            MoneyPopupManager.Instance.ShowPopup(totalEarned, Input.mousePosition);
        }
        else
        {
            Debug.Log("No wood to sell.");
        }
    }

    // Helpers
    private void SetAllColors(Color color)
    {
        foreach (var r in rends)
            r.material.color = color;
    }

    private void ResetColors()
    {
        for (int i = 0; i < rends.Length; i++)
            rends[i].material.color = originalColors[i];
    }
}