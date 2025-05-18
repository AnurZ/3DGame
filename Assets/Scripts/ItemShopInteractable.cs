using cakeslice;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Outline = cakeslice.Outline; // For UI text

[RequireComponent(typeof(Collider))]
public class ItemShopInteractable : MonoBehaviour, IShopInteractable
{
    [SerializeField] private AudioClip hoverSoundClip;
    [SerializeField] private AudioClip buySoundClip;
    [SerializeField] private AudioClip noMoneyClip;


    [SerializeField] private CursorManager CursorManager;
    
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
    
    [Header("Popup Text References")]
    public string itemDisplayName;
    
    public AchievementsController achievementsController;

    void Start()
    {
        achievementsController = FindObjectOfType<AchievementsController>();
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
    private bool cursorChanged = false;
    public void OnHoverEnter()
    {
        
        var playerObj = GameObject.FindWithTag("AudioPlayer");
        // ili ako nemaš tag, po imenu:
        if (playerObj == null)
            playerObj = GameObject.Find("AudioPlayer");

        if (playerObj != null)
        {
            var sfxSource = playerObj.GetComponent<AudioSource>();
            if (sfxSource != null && hoverSoundClip != null)
                sfxSource.PlayOneShot(hoverSoundClip);
        }
        else
        {
            Debug.LogWarning("AudioPlayer GameObject nije pronađen u sceni!");
        }

        
        if (itemToGive.itemName.ToLower().Contains("axe"))
        {
            CursorManager.SetCursorByIndex(2);
            cursorChanged = true;
        }
        else if (itemToGive.itemName.ToLower().Contains("potion"))
        {
            CursorManager.SetCursorByIndex(1);
            cursorChanged = true;
        }

        SetAllColors(hoverColor);
        if (outline != null) outline.enabled = true;

        if (pricePopupPrefab != null)
        {
            pricePopupInstance = Instantiate(pricePopupPrefab, transform.position, Quaternion.identity);

            // Find children with specific names
            var nameText = pricePopupInstance.transform.Find("ItemNameText")?.GetComponent<TMP_Text>();
            var priceText = pricePopupInstance.transform.Find("ItemPriceText")?.GetComponent<TMP_Text>();

            if (nameText != null)
                nameText.text = itemDisplayName;

            if (priceText != null)
                priceText.text = "$" + cost;

            pricePopupInstance.transform.SetParent(GameObject.Find("MoneyUI").transform, false);
            pricePopupInstance.transform.localPosition = Vector3.zero; // Adjust if needed
        }
        else
        {
            Debug.LogWarning("pricePopupPrefab is not assigned in the Inspector.");
        }
    }





    public void OnHoverExit()
    {
        if (cursorChanged)
        {
            CursorManager.SetCursorByIndex(0);
            cursorChanged = false;
        }

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
        bool isPotion = itemToGive.itemName.ToLower().Contains("potion");
        
        // If not a potion and already owned, prevent purchase
        if (!isPotion && InventoryManager.Instance.HasItem(itemToGive))
        {
            Debug.Log("Već posjeduješ ovaj item!");
            var playerObj = GameObject.FindWithTag("AudioPlayer") ?? GameObject.Find("AudioPlayer");
            if (playerObj != null && buySoundClip != null)
            {
                var sfxSource = playerObj.GetComponent<AudioSource>();
                sfxSource?.PlayOneShot(noMoneyClip);
            }
            return;
        }
        if (InventoryManager.Instance.IsFull())
        {
            Debug.Log("Inventory je pun! Ne možeš kupiti više itema.");
            var playerObj = GameObject.FindWithTag("AudioPlayer") ?? GameObject.Find("AudioPlayer");
            if (playerObj != null && noMoneyClip != null)
            {
                var sfxSource = playerObj.GetComponent<AudioSource>();
                sfxSource?.PlayOneShot(noMoneyClip);
            }
            return;
        }

        if (CurrencyManager.Instance.TrySpendMoney(cost))
        {
            
            InventoryManager.Instance.AddItem(itemToGive);
            Debug.Log($"Kupio si {itemToGive.name}!");
            MoneyPopupManager.Instance.ShowPopup(-cost, Input.mousePosition);

            var playerObj = GameObject.FindWithTag("AudioPlayer") ?? GameObject.Find("AudioPlayer");
            if (playerObj != null && buySoundClip != null)
            {
                var sfxSource = playerObj.GetComponent<AudioSource>();
                sfxSource?.PlayOneShot(buySoundClip);
            }

            // If not a potion (i.e., one-time item), gray it out after purchase
            if (!isPotion)
                SetAllColors(grayOutColor);
            if (itemToGive.isAxe)
                achievementsController.UnlockAllAxes++;
        }
        else
        {
            var playerObj = GameObject.FindWithTag("AudioPlayer") ?? GameObject.Find("AudioPlayer");
            if (playerObj != null && buySoundClip != null)
            {
                var sfxSource = playerObj.GetComponent<AudioSource>();
                sfxSource?.PlayOneShot(noMoneyClip);
            }
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
