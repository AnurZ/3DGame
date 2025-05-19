using UnityEngine;

public class SaplingPickup : MonoBehaviour
{
    public int itemIndexToGive;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
//             Debug.Log("Pickup triggered by: " + other.name);

            DemoScript demoScript = other.GetComponent<DemoScript>();
            if (demoScript != null)
            {
                if (!demoScript.PickupItem(itemIndexToGive))
                    return;
            }

            // Disable the collider and make the item inactive to prevent further pickups
            Collider itemCollider = GetComponent<Collider>();
            if (itemCollider != null)
            {
                itemCollider.enabled = false;  // Disable the collider to prevent further triggers
            }

            // Destroy the item after a short delay to allow the pickup action to complete
            Destroy(gameObject, 0.1f);  // Destroy after a brief moment to avoid immediate re-triggering
        }
    }

}
