using System;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    public float secondsInFullDay = 10f;
    public float currentTimeOfDay = 0f;
    public int dayCounter = 1;
    public int startHour = 8;

    [Header("Lighting Settings")]
    public Light directionalLight;
    public Gradient lightColor;
    public AnimationCurve lightIntensity;
    public Material daySkybox;
    public Material nightSkybox;
    public GameObject stars;

    [Header("Time Display")]
    public Text timeDisplay;
    public Text dayCounterText;

    [Header("Fade Panel")]
    public CanvasGroup fadePanel;

    [Header("Sleeping")]
    public bool isSleeping = false;
    public bool isDay = true;

    [Header("Reference to Injury System")]
    public PlayerController playerInjurySystem;

    private float cycleTime;

    public PotionManager potionManager;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Load current day from PlayerPrefs
        dayCounter = PlayerPrefs.GetInt("CurrentDay", 1);

        // Start time at 8:00 AM
        cycleTime = GetTimeFromHours(startHour);
        
        
    }

    private void Update()
    {
        // Time update
        if (!isSleeping)
        {
            float timeRate = 1f / secondsInFullDay;
            cycleTime += timeRate * Time.deltaTime;
            Debug.Log("cycle time:"  + cycleTime);
            if (cycleTime >= 1f)
            {
                cycleTime = 0f;
                AdvanceDay();
            }
        }

        // Light and skybox
        UpdateLighting(cycleTime);

        // Sleep fade
        if (isSleeping)
        {
            fadePanel.alpha = Mathf.MoveTowards(fadePanel.alpha, 1f, Time.deltaTime);
            if (fadePanel.alpha >= 1f)
            {
                // Reset time when waking up
                cycleTime = GetTimeFromHours(8);
                isDay = true;
                isSleeping = false;

                AdvanceDay();
                UpdateAudio();
                UpdateStars();
            }
        }
        else
        {
            fadePanel.alpha = Mathf.MoveTowards(fadePanel.alpha, 0f, Time.deltaTime);
        }

        // Time UI
        int hours = Mathf.FloorToInt(cycleTime * 24);
        int minutes = Mathf.FloorToInt((cycleTime * 24 - hours) * 60);
        timeDisplay.text = string.Format("{0:00}:{1:00}", hours, minutes);

        // Track day/night
        isDay = hours >= 6 && hours < 18;
    }

    float GetTimeFromHours(int hour)
    {
        return hour / 24f;
    }

    void UpdateDayCounter()
    {
        dayCounterText.text = "Day: " + dayCounter;
    }

    void UpdateLighting(float timePercent)
    {
        directionalLight.intensity = lightIntensity.Evaluate(timePercent);
        directionalLight.color = lightColor.Evaluate(timePercent);

        if (isDay)
        {
            RenderSettings.skybox = daySkybox;
            stars.SetActive(false);
        }
        else
        {
            RenderSettings.skybox = nightSkybox;
            stars.SetActive(true);
        }
    }

    void UpdateAudio()
    {
        // Optional: Update ambient audio when day/night changes
    }

    void UpdateStars()
    {
        // Optional: Animate stars or effects
    }

    public void Sleep()
    {
        isSleeping = true;
    }

    void AdvanceDay()
    {
        dayCounter++;
        UpdateDayCounter();
        Debug.Log("POTIONMANAGER: " + potionManager.ShieldPotionHours);
        PlayerPrefs.SetInt("CurrentDay", dayCounter);
        Debug.Log("Novi dan");
        if (playerInjurySystem != null)
            playerInjurySystem.OnDayPassed();
    }
}
