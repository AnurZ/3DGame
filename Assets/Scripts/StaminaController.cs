using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    public static StaminaController Instance;

    // Change this to Image
    public Image staminaBar;
    public float playerStamina = 100f;
    private float maxStamina = 100f;

    // Variables for visual smooth transition
    private float currentFillAmount;
    private float targetFillAmount;
    public float smoothTime = 2f; // Time to smoothly update the stamina bar (seconds)

    // Timer for updating the visual slider
    private float smoothTimer;

    // Stamina reduction rate per second (1 stamina per second)
    public float staminaReductionRate = 1f;

    // Track whether the player is chopping or not
    private bool isChopping = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentFillAmount = playerStamina / maxStamina; // Set initial value
        targetFillAmount = currentFillAmount; // Set target to initial value
        UpdateStaminaBar();
    }

    private void Update()
    {
        // If the player is chopping, reduce stamina
        if (isChopping && playerStamina > 0)
        {
            playerStamina -= staminaReductionRate * Time.deltaTime; // Reduce stamina by 1 per second
            playerStamina = Mathf.Clamp(playerStamina, 0f, maxStamina); // Clamp to prevent negative stamina
        }

        // Update the target fill amount based on the current stamina
        targetFillAmount = playerStamina / maxStamina;

        // If there's a difference between current and target, smooth the transition
        if (Mathf.Abs(currentFillAmount - targetFillAmount) > 0.01f)
        {
            smoothTimer += Time.deltaTime;
            float t = Mathf.Clamp01(smoothTimer / smoothTime); // Calculate time ratio (0 to 1)
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, t); // Smooth transition
            UpdateStaminaBar();
        }
    }

    // Call this method when the player starts chopping
    public void StartChopping()
    {
        isChopping = true; // Player is chopping
    }

    // Call this method when the player stops chopping
    public void StopChopping()
    {
        isChopping = false; // Player is not chopping, so stop stamina reduction
    }

    public void ReduceStamina(float amount)
    {
        // Instantly reduce stamina by the given amount
        playerStamina -= amount; // Instant stamina reduction

        // Clamp the player stamina between 0 and max
        playerStamina = Mathf.Clamp(playerStamina, 0f, maxStamina);

        // Update the target fill amount
        targetFillAmount = playerStamina / maxStamina;

        // Reset smooth timer to start the transition
        smoothTimer = 0f;
    }

    private void UpdateStaminaBar()
    {
        if (staminaBar != null)
        {
            // Update the fill amount of the stamina bar smoothly
            staminaBar.fillAmount = currentFillAmount;

            // Define target color based on the player's stamina
            Color targetColor;

            if (playerStamina > 60)
            {
                // From 100 to 60: Green to Yellow
                float t = Mathf.InverseLerp(60f, 100f, playerStamina); // t goes from 0 (green) to 1 (yellow)
                targetColor = Color.Lerp(Color.yellow, Color.green, t);
            }
            else if (playerStamina > 30)
            {
                // From 60 to 30: Yellow to Red
                float t = Mathf.InverseLerp(30f, 60f, playerStamina); // t goes from 0 (yellow) to 1 (red)
                targetColor = Color.Lerp(Color.red, Color.yellow, t);
            }
            else
            {
                // Below 30: Red
                targetColor = Color.red;
            }

            // Smoothly transition to the target color
            staminaBar.color = Color.Lerp(staminaBar.color, targetColor, Time.deltaTime * 5f); // Adjust the transition speed
        }
    }



}
