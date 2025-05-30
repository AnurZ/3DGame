using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Mono.Cecil.Cil;

[RequireComponent(typeof(Collider))]
public class SellWoodInteractable : MonoBehaviour, IShopInteractable
{
    [SerializeField] private AudioClip sellSoundClip;

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

        originalColors = new Color[rends.Length];
        for (int i = 0; i < rends.Length; i++)
        {
            if (rends[i].material.HasProperty("_Color"))
            {
                originalColors[i] = rends[i].material.color;
            }
            else
            {
                originalColors[i] = Color.white; // default fallback boja
            }
        }


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
            Transform countTransform = transform.Find("Count");
            if (countTransform != null)
            {
                countText = countTransform.GetComponent<TextMeshPro>();
            }

            StartCoroutine(DelayedCountUpdate());

            var playerObj = GameObject.Find("AudioPlayer");
            if (playerObj != null && sellSoundClip != null)
            {
                var sfx = playerObj.GetComponent<AudioSource>();
                sfx?.PlayOneShot(sellSoundClip);
            }
            
            // Ažuriraj sve druge objekte sa SellWoodInteractable skriptom
            SellWoodInteractable[] allWoodObjects = FindObjectsOfType<SellWoodInteractable>();
            foreach (var woodObject in allWoodObjects)
            {
                woodObject.StartCoroutine(woodObject.DelayedCountUpdate());

            }
            StartCoroutine(DelayedCountUpdate());

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
