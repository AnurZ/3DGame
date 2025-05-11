using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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

    private TextMeshPro countText;

    private void Start()
    {
        rends = GetComponentsInChildren<Renderer>();

        List<Color> colors = new List<Color>();
        foreach (Renderer r in rends)
        {
            if (r.material.HasProperty("_Color"))
            {
                colors.Add(r.material.color);
            }
        }
        originalColors = colors.ToArray();

        // Try to find a child named "Count" with TextMeshPro
        Transform countTransform = transform.Find("Count");
        if (countTransform != null)
        {
            countText = countTransform.GetComponent<TextMeshPro>();
        }

        UpdateCountDisplay();
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
            MoneyPopupManager.Instance.ShowPopup(-87, Input.mousePosition);
        }

        // Delay update by a frame to allow inventory changes to apply
        StartCoroutine(DelayedCountUpdate());
    }

    private IEnumerator DelayedCountUpdate()
    {
        yield return null; // wait one frame
        UpdateCountDisplay();
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

    public void UpdateCountDisplay()
    {
        if (countText == null) return;

        int totalAmount = 0;
        var inv = InventoryManager.Instance;

        foreach (var wp in woodPrices)
        {
            totalAmount += inv.GetTotalCountOfItem(wp.woodItem);
        }

        countText.text = totalAmount.ToString();
    }
}
