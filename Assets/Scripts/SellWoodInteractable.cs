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

    public WoodPrice[] woodPrices;
    public Color hoverColor = Color.yellow;
    public Color soldColor = Color.green;

    Renderer[] rends;
    Color[] originalColors;

    void Start()
    {
        rends = GetComponentsInChildren<Renderer>();
        originalColors = rends.Select(r => r.material.color).ToArray();
    }

    // IShopInteractable implementation
    public void OnHoverEnter()
    {
        Debug.Log("Sell Hover Enter");
        SetAllColors(hoverColor);
    }

    public void OnHoverExit() 
    {
        Debug.Log("Sell Hover Exit");
        ResetColors();
    }

    public void OnActivate()
    {
        Debug.Log("Sell Activate");
        TrySell();
    }

    // actual sell logic
    void TrySell()
    {
        var inv = InventoryManager.Instance;
        int total = 0;

        foreach (var wp in woodPrices)
            total += inv.RemoveAllOfItem(wp.woodItem) * wp.pricePerUnit;

        if (total > 0)
        {
            CurrencyManager.Instance.AddMoney(total);
            Debug.Log($"Sold wood for {total}");
            SetAllColors(soldColor);
        }
        else Debug.Log("No wood to sell.");
    }

    void SetAllColors(Color c)
    {
        foreach (var r in rends) r.material.color = c;
    }

    void ResetColors()
    {
        for (int i = 0; i < rends.Length; i++)
            rends[i].material.color = originalColors[i];
    }
}