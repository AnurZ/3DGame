using cakeslice;
using UnityEngine;

// Make sure you have this interface somewhere in your project:


[RequireComponent(typeof(Collider))]
public class ItemShopInteractable : MonoBehaviour, IShopInteractable
{
    [Header("Buy Settings")]
    public Item  itemToGive;
    public int   cost = 10;

    [Header("Highlight Colors")]
    public Color hoverColor = Color.green;
    public Color grayOutColor = Color.gray;

    private Renderer[] renderers;
    private Color[]    originalColors;
    private Outline    outline;


    
    void Start()
    {
        // Cache all renderers & their original colors
        renderers      = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].material.color;

        // If you have an Outline component on the root, disable it by default
        outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;

        // If player already owns this item, gray it out immediately
        if (InventoryManager.Instance.HasItem(itemToGive))
            SetAllColors(grayOutColor);
    }

    // IShopInteractable ↓

    public void OnHoverEnter()
    {
        // Highlight: yellow + outline
        SetAllColors(hoverColor);
        if (outline != null) outline.enabled = true;
    }

    public void OnHoverExit()
    {
        // If owned, keep gray; otherwise revert to original
        if (InventoryManager.Instance.HasItem(itemToGive))
            SetAllColors(grayOutColor);
        else
            RestoreOriginalColors();

        if (outline != null) outline.enabled = false;
    }

  
    
    public void OnActivate()
    {
        // Try to buy
        if (InventoryManager.Instance.HasItem(itemToGive))
        {
            Debug.Log("Već posjeduješ ovaj item!");
            return;
        }

        if (CurrencyManager.Instance.TrySpendMoney(cost))
        {
            InventoryManager.Instance.AddItem(itemToGive);
            Debug.Log($"Kupio si {itemToGive.name}!");
            MoneyPopupManager.Instance.ShowPopup(-cost, Input.mousePosition);
        }
        else
        {
            Debug.Log("Nemaš dovoljno novca.");
        }
    }

    // Helpers ↓

    private void SetAllColors(Color c)
    {
        foreach (var r in renderers)
            r.material.color = c;
    }

    private void RestoreOriginalColors()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = originalColors[i];
    }
}