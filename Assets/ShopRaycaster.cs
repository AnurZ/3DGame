using UnityEngine;

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
        // ← IGNORE triggers by passing QueryTriggerInteraction.Ignore
        if (Physics.Raycast(
                ray,
                out RaycastHit hit,
                maxDistance,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore
            ))
        {
            //Debug.Log($"ShopRaycaster hit: {hit.collider.name}");

            var buy  = hit.collider.GetComponent<ItemShopInteractable>();
            var sell = hit.collider.GetComponent<SellWoodInteractable>();
            IShopInteractable current = buy != null ? buy : sell;

            if (current != lastHovered)
            {
                ClearHover();
                if (current != null)
                {
                    //Debug.Log("Hover start on " + hit.collider.name);
                    current.OnHoverEnter();
                }
                lastHovered = current;
            }

            if (Input.GetMouseButtonDown(0) && current != null)
            {
                //Debug.Log("Click activate on " + hit.collider.name);
                current.OnActivate();
            }
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
            Debug.Log("Hover exit on " + ((MonoBehaviour)lastHovered).name);
            lastHovered.OnHoverExit();
            lastHovered = null;
        }
    }
}