using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;
    public Transform handTransform; // Assign handslot.r in Inspector
    private GameObject currentHeldItem; // Tracks what’s currently in hand
    private Item lastSelectedItem = null;
    public PlayerController playerController;

    private int selectedSlot = -1;

    private void Start()
    {
        ChangeSelectedSlot(0);
    }

    private void Update()
    {
        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number < 6)
            {
                ChangeSelectedSlot(number - 1);
            }
        }

        // Call it every frame (safe because it only updates on change)
        CheckForSelectedSlotChange();
    }

    
    public void CheckForSelectedSlotChange()
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        Item currentItem = itemInSlot != null ? itemInSlot.item : null;

        if (currentItem != lastSelectedItem)
        {
            lastSelectedItem = currentItem;
            UpdateHeldItem(); // Call when item changes
        }
    }

    
    void ChangeSelectedSlot(int newValue)
    {
        if (playerController != null && !playerController.isChopping)
        {
            if (selectedSlot >= 0)
                        inventorySlots[selectedSlot].Deselect();
            
                    inventorySlots[newValue].Select();
                    selectedSlot = newValue;
            
                    UpdateHeldItem(); // <-- Add this
        }
    }

    public void RemoveItemFromHand()
    {
        if (currentHeldItem != null)
        {
            Destroy(currentHeldItem); // Destroy the held item in hand
            currentHeldItem = null;
        }
    }

    
    void UpdateHeldItem()
    {
        if (currentHeldItem != null)
            Destroy(currentHeldItem);

        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        if (itemInSlot != null && itemInSlot.item.itemPrefab != null)
        {
            currentHeldItem = Instantiate(itemInSlot.item.itemPrefab, handTransform);

            // Apply custom transform values
            currentHeldItem.transform.localPosition = itemInSlot.item.inHandPosition;
            currentHeldItem.transform.localRotation = Quaternion.Euler(itemInSlot.item.inHandRotation);
            currentHeldItem.transform.localScale = itemInSlot.item.inHandScale;
        }
    }


    
    public bool AddItem(Item item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item)
            {

                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
        }
        
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }
        return false;
    }


    void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
    }

    public Item GetSelectedItem(bool use)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
                
            Item item = itemInSlot.item;

            if (use == true)
            {
                itemInSlot.count--;
                if (itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
            }
            
            return item;
        }

        return null;
    }
    
}
