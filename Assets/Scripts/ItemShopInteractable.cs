using cakeslice;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Outline = cakeslice.Outline; // For UI text

[RequireComponent(typeof(Collider))]
public class ItemShopInteractable : MonoBehaviour, IShopInteractable
{
    [Header("Buy Settings")]
    public Item itemToGive;
    public int cost = 10;

    [Header("Highlight Colors")]
    public Color hoverColor = Color.green;
    public Color grayOutColor = Color.gray;

    private Renderer[] renderers;
    private Color[] originalColors;
    private Outline outline;

    // For showing price
    public GameObject pricePopupPrefab;  // Assign prefab with a Text component
    private GameObject pricePopupInstance;

    void Start()
    {
        // Cache all renderers & their original colors
        renderers = GetComponentsInChildren<Renderer>();
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

        // Show the price popup
        if (pricePopupPrefab != null)
        {
            // Instantiate the price popup at the item's position
            pricePopupInstance = Instantiate(pricePopupPrefab, transform.position, Quaternion.identity);

            // Find the Text (or TMP_Text) component inside the popup instance
            var priceText = pricePopupInstance.GetComponentInChildren<TMP_Text>();  // or TMP_Text if you're using TextMesh Pro
        
            if (priceText != null)
            {
                priceText.text = "$" + cost.ToString();  // Set the price text
            }
            else
            {
                Debug.LogWarning("Text component not found in pricePopupPrefab.");
            }

            // Set the parent to the MoneyUI's transform (replace 'MoneyUI' with your actual GameObject name or reference)
            pricePopupInstance.transform.SetParent(GameObject.Find("MoneyUI").transform);  // Set the parent to MoneyUI

            // Optionally adjust the local position if needed
            pricePopupInstance.transform.localPosition = Vector3.zero;  // Adjust this as needed
        }
        else
        {
            Debug.LogWarning("pricePopupPrefab is not assigned in the Inspector.");
        }
    }



    public void OnHoverExit()
    {
        // If owned, keep gray; otherwise revert to original
        if (InventoryManager.Instance.HasItem(itemToGive))
            SetAllColors(grayOutColor);
        else
            RestoreOriginalColors();

        if (outline != null) outline.enabled = false;

        // Hide the price popup
        if (pricePopupInstance != null)
        {
            Destroy(pricePopupInstance);  // Destroy the price popup when hover ends
        }
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
