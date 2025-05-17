using UnityEngine;

public class FolderInventoryTrue : MonoBehaviour
{
    public GameObject inventoryUI;
    public GameObject buttonToHide;  // This gameObject (FolderInventoryTrue)
    public GameObject buttonToShow;  // FolderInventoryFalse
    public AudioClip open;
    public AudioSource audioSource;
    //public AudioClip close;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ShowInventory();
        }
    }

    public void ShowInventory()
    {
        audioSource.PlayOneShot(open);
        inventoryUI.SetActive(true);
        buttonToShow.SetActive(false);  // Show FolderInventoryFalse
        buttonToHide.SetActive(true); // Hide this one (FolderInventoryTrue)

    }
}