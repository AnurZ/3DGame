using UnityEngine;

public class MoneyPopupManager : MonoBehaviour
{
    public static MoneyPopupManager Instance;

    [Header("Popup Settings")]
    public GameObject moneyPopupPrefab;
    public Transform popupParent;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowPopup(int amount, Vector3 screenPosition)
    {
        if (moneyPopupPrefab == null)
        {
            Debug.LogWarning("MoneyPopup Prefab nije postavljen!");
            return;
        }

        if (amount == 0)
        {
            Debug.LogWarning("Popup za 0 amount ne treba da se prikazuje!");
            return;
        }

        // Instantiate the popup
        GameObject popupObj = Instantiate(moneyPopupPrefab, popupParent);

        // Convert the screen position to local position within the Canvas
        RectTransform rectTransform = popupObj.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            // Convert screen space position to local space relative to the canvas
            Vector2 localPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(popupParent.GetComponent<RectTransform>(), screenPosition, null, out localPosition);
            rectTransform.localPosition = localPosition;

            // Optional: If you don't want rotation, ensure it's reset
            rectTransform.localRotation = Quaternion.identity;
        }

        var popup = popupObj.GetComponent<MoneyPopup>();
        if (popup != null)
        {
            popup.Setup(amount);
        }
    }

}