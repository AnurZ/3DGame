using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public Color selectedColor, notSelectedColor;

    private void Awake()
    {
        Deselect();
    }
    
    public void Select()
    {
        if (image != null)
        {
            image.color = selectedColor;
        }
        else
        {
            Debug.LogWarning($"Image reference is missing on {gameObject.name}");
        }
    }

    public void Deselect()
    {
        if (image != null)
        {
            image.color = notSelectedColor;
        }
        else
        {
            Debug.LogWarning($"Image reference is missing on {gameObject.name}");
        }
    }

    
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0) // Only allow one item per slot
        {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            if (inventoryItem != null)
            {
                inventoryItem.parentAfterDrag = transform; // Set new parent
            }
        }
    }
    
    
}