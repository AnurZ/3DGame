using TMPro;
using UnityEngine;

public class MoneyPopup : MonoBehaviour
{
    public TextMeshProUGUI popupText;   // Reference to the TextMeshProUGUI component for the amount text
    public float moveUpSpeed = 30f;     // Speed at which the popup moves upwards
    public float fadeDuration = 1f;     // Duration for the fading effect

    [Header("SFX Clips")]
    [SerializeField] private AudioClip errorSoundClip;  // Assign your “no wood” SFX here

    private float lifetime = 1.5f;      // How long the popup will stay visible
    private CanvasGroup canvasGroup;    // To control the fading effect

    private void Awake()
    {
        // Get the CanvasGroup component attached to the popup
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(int amount)
    {
        // Check if popupText is assigned
        if (popupText != null)
        {
            if (amount == -87)
            {
                popupText.text = "No wood to sell!";
                PlayErrorSfx();  // <- pozivamo error zvuk
            }
            else if (amount > 0)
            {
                popupText.text = "+" + amount.ToString();
            }
            else if (amount < 0)
            {
                popupText.text = amount.ToString();
            }

            // Set the color to green for positive amounts, red for negative
            popupText.color = (amount > 0) ? Color.green : Color.red;
        }
        else
        {
//             Debug.LogError("Popup text is not assigned!");
        }
    }

    private void PlayErrorSfx()
    {
        if (errorSoundClip == null) return;

        // Pronađemo AudioPlayer object u sceni
        var playerObj = GameObject.FindWithTag("AudioPlayer") ?? GameObject.Find("AudioPlayer");
        if (playerObj != null)
        {
            var sfxSource = playerObj.GetComponent<AudioSource>();
            if (sfxSource != null)
                sfxSource.PlayOneShot(errorSoundClip);
        }
        else
        {
//             Debug.LogWarning("AudioPlayer GameObject nije pronađen u sceni za error SFX!");
        }
    }

    private void Update()
    {
        // Move the popup upwards
        transform.Translate(Vector3.up * moveUpSpeed * Time.deltaTime);

        // Gradually fade out
        lifetime -= Time.deltaTime;
        if (lifetime <= fadeDuration)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = lifetime / fadeDuration; // Adjust alpha based on lifetime
            }
        }

        // Destroy the popup when the lifetime reaches 0
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
