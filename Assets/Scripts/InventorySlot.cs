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
//             Debug.LogWarning($"Image reference is missing on {gameObject.name}");
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
//             Debug.LogWarning($"Image reference is missing on {gameObject.name}");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
        if (inventoryItem != null)
        {
            // Check if this slot already has an item
            if (transform.childCount > 0)
            {
                Transform existingItem = transform.GetChild(0);
                existingItem.SetParent(inventoryItem.parentAfterDrag);
                existingItem.localPosition = Vector3.zero;
            }

            inventoryItem.parentAfterDrag = transform;
        }
    }

    
    
    
}
