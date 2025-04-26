using UnityEngine;
using System;

public class ShopRaycaster : MonoBehaviour
{
    public float maxDistance = 10f;
    private IShopInteractable lastHovered;

    void Update()
    {
        if (!PlayerController.Local.isInShop)
        {
            ClearHover();
            return;
        }

        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        // 1) Gather all hits (ignoring triggers)
        RaycastHit[] hits = Physics.RaycastAll(
            ray,
            maxDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore
        );

        if (hits.Length > 0)
        {
            // 2) Sort by distance
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            IShopInteractable current = null;
            // 3) Find the first non-player interactable
            foreach (var h in hits)
            {
                // skip the Player collider
                if (h.collider.CompareTag("Player")) 
                    continue;

                // try buy or sell component
                var buy  = h.collider.GetComponent<ItemShopInteractable>();
                var sell = h.collider.GetComponent<SellWoodInteractable>();
                current = buy != null ? buy : sell;

                // if we found an interactable, stop searching
                if (current != null)
                    break;
            }

            // 4) Hover logic
            if (current != lastHovered)
            {
                ClearHover();
                if (current != null)
                    current.OnHoverEnter();
                lastHovered = current;
            }

            // 5) Click logic
            if (Input.GetMouseButtonDown(0) && current != null)
                current.OnActivate();
        }
        else
        {
            ClearHover();
        }
    }

    void ClearHover()
    {
        if (lastHovered != null)
        {
            lastHovered.OnHoverExit();
            lastHovered = null;
        }
    }
}
