using TMPro;
using UnityEngine;

public class MoneyPopup : MonoBehaviour
{
    public TextMeshProUGUI popupText;
    public float moveUpSpeed = 30f;
    public float fadeDuration = 1f;

    private float lifetime = 1.5f;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(int amount)
    {
        if (popupText != null)
        {
            popupText.text = (amount > 0 ? "+" : "") + amount.ToString();

            if (amount > 0)
                popupText.color = Color.green;
            else
                popupText.color = Color.red;
        }
    }

    private void Update()
    {
        // Pomjeri popup prema gore
        transform.Translate(Vector3.up * moveUpSpeed * Time.deltaTime);

        // Postepeno izblijedi
        lifetime -= Time.deltaTime;
        if (lifetime <= fadeDuration)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = lifetime / fadeDuration;
            }
        }

        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}