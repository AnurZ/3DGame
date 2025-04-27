using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    
    
    [Header("UI")]
    public Image image;
    public Text countText;
    
    public Item item;
    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;

    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }
    
    private void Start()
    {
        InitialiseItem(item);
    }
    public void InitialiseItem(Item newItem)
    {
        item = newItem;
        image.sprite = newItem.image;
        RefreshCount();
    }
    private void Awake()
    {
        if (image == null)
            image = GetComponent<Image>(); // Ensure there's an image component
    }

    
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false; // Prevents blocking raycasts while dragging
        parentAfterDrag = transform.parent; // Store original parent
        transform.SetParent(transform.root); // Move to top level in hierarchy
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; // Follow mouse position
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true; // Re-enable raycasts

        Transform newParent = GetSlotUnderPointer(eventData);
        if (newParent != null)
        {
            transform.SetParent(newParent); // Move item to the detected slot
        }
        else
        {
            transform.SetParent(parentAfterDrag); // Return to original slot
        }

        transform.localPosition = Vector3.zero; // Reset position inside slot
    }

    private Transform GetSlotUnderPointer(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results); // Get all objects under cursor

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("InventorySlot")) // Check for slots
            {
                return result.gameObject.transform; // Return the first valid slot
            }
        }

        return null; // No valid slot found
    }
}