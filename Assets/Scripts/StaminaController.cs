using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    public static StaminaController Instance;

    [Header("Stamina Parameters")]
    public float playerStamina = 100.0f;
    public float maxStamina = 100.0f;

    [Header("UI")]
    public Image staminaFillImage; // Assign this in the Inspector

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    public void ReduceStamina(float amount)
    {
        playerStamina -= amount;
        playerStamina = Mathf.Clamp(playerStamina, 0f, maxStamina);
        UpdateUI();
    }

    public void RestoreStamina(float amount)
    {
        playerStamina += amount;
        playerStamina = Mathf.Clamp(playerStamina, 0f, maxStamina);
        UpdateUI();
    }

    public bool HasStamina(float cost)
    {
        return playerStamina >= cost;
    }

    void UpdateUI()
    {
        if (staminaFillImage != null)
        {
            staminaFillImage.fillAmount = playerStamina / maxStamina;
        }
    }
}