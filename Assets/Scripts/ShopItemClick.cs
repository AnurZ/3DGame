using UnityEngine;

public class ShopItemClick : MonoBehaviour
{
    public Camera playerCamera;  // Kamera iz koje će se raditi Raycast
    public float raycastDistance = 5f;  // Maksimalna udaljenost za interakciju

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Levo dugme miša
        {
            RaycastHit hit;
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);  // Pravimo ray iz pozicije miša na ekranu

            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                if (hit.collider != null)
                {
                    // Ako je hitovani objekat item shopa
                    ItemShopInteractable item = hit.collider.GetComponent<ItemShopInteractable>();
                    if (item != null)
                    {
                        BuyItem(item);  // Pozivamo metodu za kupovinu
                    }
                }
            }
        }
    }

    void BuyItem(ItemShopInteractable item)
    {
        if (CurrencyManager.Instance.CurrentMoney >= item.cost)  // Proveri da li ima dovoljno novca
        {
            CurrencyManager.Instance.CurrentMoney -= item.cost;  // Oduzmi novac
            InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();  // Pronađi instancu InventoryManager
            inventoryManager.AddItem(item.itemToGive);  // Dodaj item u inventar
            Debug.Log("Item kupljen: " + item.itemToGive.name);
        }
        else
        {
            Debug.Log("Nemate dovoljno novca za kupovinu ovog itema.");
        }
    }

}