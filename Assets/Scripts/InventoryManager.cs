using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;
    public Transform handTransform; // Assign handslot.r in Inspector
    private GameObject currentHeldItem; // Tracks what’s currently in hand
    private Item lastSelectedItem = null;
    public PlayerController playerController;

    private int selectedSlot = -1;
    public TMP_Text textMeshPro;
    public GameObject uipanel;

    public TMP_Text ItemNameText;
    public GameObject ItemNamePanel;
    
    public AchievementsController achievementsController;
    
    public bool IsFull()
    {
        foreach (var slot in inventorySlots)
        {
            var itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                return false; // Pronađen prazan slot
            }
        }
        return true; // Nema praznih slotova
    }

    private void Start()
    {
        achievementsController = FindObjectOfType<AchievementsController>();
        ChangeSelectedSlot(0);
        if (InventoryIsEmpty())
        {
            AddDefaultItemsToInventory();
        }
    }

    private bool InventoryIsEmpty()
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.transform.childCount > 0) // If the slot has items, return false
                return false;
        }
        return true;
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

    public int GetSelectedSlotIndex()
    {
        return selectedSlot;
    }

    public void ChangeSelectedSlotExternally(int index)
    {
        ChangeSelectedSlot(index);
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
    
    private IEnumerator ShowTextCoroutine(string text)
    {
        uipanel.SetActive(true);
        textMeshPro.text = text;
        textMeshPro.color = Color.white;

        yield return new WaitForSeconds(2f); // change 2f to your desired display time

        textMeshPro.text = "";
        uipanel.SetActive(false);

        currentTextCoroutine = null; // clear reference when done
    }

    void Awake()
    {
        Instance = this;
    }

    private Coroutine currentTextCoroutine;
    
    void ChangeSelectedSlot(int newValue)
    {
        if (playerController != null && !playerController.isChopping)
        {
            if (selectedSlot >= 0)
                inventorySlots[selectedSlot].Deselect();

            inventorySlots[newValue].Select();
            selectedSlot = newValue;

            Item currentItem = GetSelectedItem(false);
            if (currentItem != null)
            {
                string message = currentItem.isPotion
                    ? currentItem.ItemDisplayName + " press [SPACEBAR] TO DRINK"
                    : currentItem.ItemDisplayName;

                // Stop the currently running coroutine if it exists
                if (currentTextCoroutine != null)
                    StopCoroutine(currentTextCoroutine);

                // Start and track the new coroutine
                currentTextCoroutine = StartCoroutine(ShowTextCoroutine(message));
            }

            UpdateHeldItem();
        }
    }

    public int GetTotalCountOfItem(Item item)
    {
        int total = 0;

        foreach (var slot in inventorySlots)
        {
            var invItem = slot.GetComponentInChildren<InventoryItem>();
            if (invItem != null && invItem.item == item)
            {
                total += invItem.count;
            }
        }

        return total;
    }

    
    public void RemoveItemFromHand()
    {
        if (currentHeldItem != null)
        {
            if (GetSelectedItem(false).isPotion)
            {
                if(achievementsController.DrinkPotionsDaily == achievementsController.PotionsDrankUntilYesterday) 
                    achievementsController.DrinkPotionsDaily ++;
                achievementsController.UsePotions ++;
            }
            Destroy(currentHeldItem); // Remove the held item visually
            currentHeldItem = null;
        }

        // Also remove the item from the currently selected inventory slot
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            Destroy(itemInSlot.gameObject);
        }

        lastSelectedItem = null; // Reset the last selected item so held item updates on next change
    }

    private void AddDefaultItemsToInventory()
    {
        // Define default items to be added to the inventory
        Item woodItem = Resources.Load<Item>("Items/Wood");
        Item axeItem = Resources.Load<Item>("Items/Axe");
        Item saplingItem = Resources.Load<Item>("Items/Sapling");

        // Add default items to the inventory
        InventoryManager.Instance.AddItem(woodItem);
        InventoryManager.Instance.AddItem(axeItem);
        InventoryManager.Instance.AddItem(saplingItem);

        // Optionally set a starting amount
        InventoryManager.Instance.AddItem(woodItem);
        InventoryManager.Instance.AddItem(axeItem);
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

    public bool HasItem(Item itemToCheck)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            var invItem = inventorySlots[i].GetComponentInChildren<InventoryItem>();
            if (invItem != null && invItem.item == itemToCheck)
                return true;
        }
        return false;
    }

    public int RemoveAllOfItem(Item itemToRemove)
    {
        int totalRemoved = 0;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            var slot = inventorySlots[i];
            var invItem = slot.GetComponentInChildren<InventoryItem>();
            if (invItem != null && invItem.item == itemToRemove)
            {
                totalRemoved += invItem.count;
                Destroy(invItem.gameObject);
            }
        }

        return totalRemoved;
    }

    public bool AddItem(Item item)
    {
        Debug.Log("Start adding item");
        bool isStackable = !(item.isAxe || item.isPotion);

        // First, try to stack in existing slot
        if (isStackable)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                InventorySlot slot = inventorySlots[i];
                InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < 50)
                {
                    itemInSlot.count++;
                    itemInSlot.RefreshCount();
                    return true;
                }
            }
        }

        // Then, try to add to a new slot
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
