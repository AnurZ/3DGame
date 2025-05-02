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

    private void Awake()
    {
        if (image == null)
            image = GetComponent<Image>(); // Osiguraj da postoji Image
    }

    public void InitialiseItem(Item newItem, int newAmount = 1)
    {
        if (newItem != null)
        {
            item = newItem;
            count = Mathf.Max(1, newAmount); // Osiguraj da count nije manji od 1
            if (newItem.image != null)
                image.sprite = newItem.image;
            RefreshCount();
        }
    }

    public void RefreshCount()
    {
        countText.text = count.ToString();
        countText.gameObject.SetActive(count > 1);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root); // Move to top hierarchy level
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;

        Transform newParent = GetSlotUnderPointer(eventData);
        transform.SetParent(newParent != null ? newParent : parentAfterDrag);
        transform.localPosition = Vector3.zero;
    }

    private Transform GetSlotUnderPointer(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("InventorySlot"))
                return result.gameObject.transform;
        }

        return null;
    }
}
