using UnityEngine;

public class FolderInventoryTrue : MonoBehaviour
{
    public GameObject inventoryUI;
    public GameObject buttonToHide;  // This gameObject (FolderInventoryTrue)
    public GameObject buttonToShow;  // FolderInventoryFalse

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShowInventory();
        }
    }

    public void ShowInventory()
    {
        inventoryUI.SetActive(true);
        buttonToShow.SetActive(false);  // Show FolderInventoryFalse
        buttonToHide.SetActive(true); // Hide this one (FolderInventoryTrue)

    }
}