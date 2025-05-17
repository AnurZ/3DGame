using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    public static StaminaController Instance;

    [Header("UI")]
    public Image staminaBar;

    [Header("Stamina Settings")]
    public float playerStamina = 100f;
    private float maxStamina = 100f;
    public float staminaReductionRate = 0.25f;

    [Header("Smooth Settings")]
    private float currentFillAmount;
    private float targetFillAmount;
    public float smoothTime = 2f;
    private float smoothTimer;

    private float previousStamina;

    private bool isChopping = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentFillAmount = playerStamina / maxStamina;
        targetFillAmount = currentFillAmount;
        previousStamina = playerStamina;
        UpdateStaminaBar(forceInstantColor: true);
    }

    private void Update()
    {
        // Handle stamina drain while chopping
        if (isChopping && playerStamina > 0)
        {
            playerStamina -= staminaReductionRate * Time.deltaTime;
            playerStamina = Mathf.Clamp(playerStamina, 0f, maxStamina);
        }

        // Update target fill amount
        targetFillAmount = playerStamina / maxStamina;

        // Smooth fill transition
        if (Mathf.Abs(currentFillAmount - targetFillAmount) > 0.01f)
        {
            smoothTimer += Time.deltaTime;
            float t = Mathf.Clamp01(smoothTimer / smoothTime);
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, t);
            UpdateStaminaBar(forceInstantColor: false);
        }

        // Check if stamina was instantly changed (e.g. after sleeping)
        if (Mathf.Abs(playerStamina - previousStamina) > 20f)
        {
            currentFillAmount = targetFillAmount;
            UpdateStaminaBar(forceInstantColor: true);
        }

        previousStamina = playerStamina;
    }

    public void StartChopping()
    {
        isChopping = true;
    }

    public void StopChopping()
    {
        isChopping = false;
    }

    public void ReduceStamina(float amount)
    {
        playerStamina -= amount;
        playerStamina = Mathf.Clamp(playerStamina, 0f, maxStamina);
        targetFillAmount = playerStamina / maxStamina;
        smoothTimer = 0f;
    }

    public void RestoreFullStamina()
    {
        playerStamina = maxStamina;
        targetFillAmount = 1f;
        smoothTimer = 0f;
    }

    private void UpdateStaminaBar(bool forceInstantColor)
    {
        if (staminaBar == null) return;

        staminaBar.fillAmount = currentFillAmount;

        // Determine target color
        Color targetColor;
        if (playerStamina > 60)
        {
            float t = Mathf.InverseLerp(60f, 100f, playerStamina);
            targetColor = Color.Lerp(Color.yellow, Color.green, t);
        }
        else if (playerStamina > 30)
        {
            float t = Mathf.InverseLerp(30f, 60f, playerStamina);
            targetColor = Color.Lerp(Color.red, Color.yellow, t);
        }
        else
        {
            targetColor = Color.red;
        }

        // Smooth or instant color update
        if (forceInstantColor)
        {
            staminaBar.color = targetColor;
        }
        else
        {
            staminaBar.color = Color.Lerp(staminaBar.color, targetColor, Time.deltaTime * 5f);
        }
    }
}
