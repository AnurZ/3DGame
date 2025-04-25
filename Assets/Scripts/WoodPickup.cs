using UnityEngine;

public class WoodPickup : MonoBehaviour
{
    public int itemIndexToGive;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Pickup triggered by: " + other.name);

            DemoScript demoScript = other.GetComponent<DemoScript>();
            if (demoScript != null)
            {
                demoScript.PickupItem(itemIndexToGive);
            }

            // Destroy the item immediately after the player picks it up
            Destroy(gameObject);
        }
    }
}