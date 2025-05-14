using UnityEngine;

public class FolderInventoryFalse : MonoBehaviour
{
    public GameObject inventoryUI;
    public GameObject buttonToHide;  // FolderInventoryFalse
    public GameObject buttonToShow;  // FolderInventoryTrue

    private bool justClosedInventory = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            HideInventory();
        }

        // This prevents the next click from locking the cursor unintentionally
        if (justClosedInventory && Input.GetMouseButtonDown(0))
        {
            justClosedInventory = false; // Reset flag
            EventSystemCleanup();
        }
    }

    public void HideInventory()
    {
        inventoryUI.SetActive(false);
        buttonToShow.SetActive(true);  // Show FolderInventoryTrue
        buttonToHide.SetActive(false); // Hide FolderInventoryFalse

       

        justClosedInventory = true; // Flag to protect from first click
    }

    private void EventSystemCleanup()
    {
        // If needed, you can re-lock or re-show here. In most cases, we just clear input.
        // Optional debug log:
        // Debug.Log("Prevented cursor from hiding on click after inventory close.");
    }
}