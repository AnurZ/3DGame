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

        GameObject popupObj = Instantiate(moneyPopupPrefab, popupParent);
        popupObj.transform.position = screenPosition;

        var popup = popupObj.GetComponent<MoneyPopup>();
        if (popup != null)
        {
            popup.Setup(amount);
        }
    }
}