using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SellSpecificWoodInteractable : MonoBehaviour, IShopInteractable
{
    [Header("Sell Settings")]
    public Item woodItem;
    public int pricePerUnit = 1;

    [Header("Colors")]
    public Color hoverColor = Color.yellow;
    public Color soldColor = Color.green;

    private Renderer[] rends;
    private Color[] originalColors;

    private void Start()
    {
        rends = GetComponentsInChildren<Renderer>();
        originalColors = new Color[rends.Length];
        for (int i = 0; i < rends.Length; i++)
        {
            originalColors[i] = rends[i].material.color;
        }
    }

    public void OnHoverEnter()
    {
        Debug.Log("OnHoverEnter");
        SetAllColors(hoverColor);
    }

    public void OnHoverExit()
    {
        ResetColors();
    }

    public void OnActivate()
    {
        TrySellAll();
    }

    private void TrySellAll()
    {
        var inv = InventoryManager.Instance;
        int amountSold = inv.RemoveAllOfItem(woodItem);

        if (amountSold > 0)
        {
            int totalEarned = amountSold * pricePerUnit;
            CurrencyManager.Instance.AddMoney(totalEarned);
            SetAllColors(soldColor);
            MoneyPopupManager.Instance.ShowPopup(totalEarned, Input.mousePosition);
        }
        else
        {
            Debug.Log("No items of type " + woodItem.itemName + " to sell.");
        }
    }

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