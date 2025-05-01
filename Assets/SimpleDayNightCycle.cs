using UnityEngine;
using TMPro;
using System.Collections;

public class SimpleDayNightCycle : MonoBehaviour
{
    public Light sunLight;
    public Gradient lightColor;
    public float fullDayLengthInSeconds = 480f;
    
    public AudioSource audioSource;
    public AudioClip yawningSound;

    
    public GameObject sleepIcon;
    
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dayText;

    public CanvasGroup sleepFader; // Dodaj referencu u Inspectoru na UI fader (CanvasGroup)

    private float timeOfDay = 0f;
    private int currentDay = 1;
    private bool isSleeping = false;
    
    [Header("Reference to Injury System")]
    public PlayerController playerInjurySystem;

    public PotionManager potionManager;

    void Start()
    {
        timeOfDay = GetTimeNormalizedFromHour(8);
        currentDay = PlayerPrefs.GetInt("Day", 1);
        potionManager = FindObjectOfType<PotionManager>();
        UpdateDayText();
    }
    private bool yawningPlayed = false; // Flag to track if yawning has played
    public AudioClip notificationSound;  // The notification sound clip
    

    void Update()
    {
        if (isSleeping) return;

        timeOfDay += Time.deltaTime / fullDayLengthInSeconds;

        if (timeOfDay >= 1f)
        {
            timeOfDay -= 1f;
            currentDay++;
            PlayerPrefs.SetInt("Day", currentDay);
            UpdateDayText();
        }

        if (sleepIcon != null)
        {
            sleepIcon.SetActive(IsSleepTime());
        }

        // Play notification sound followed by yawning sound
        if (IsSleepTime() && !yawningPlayed)
        {
            StartCoroutine(PlaySoundsInSequence());
            yawningPlayed = true;  // Prevent further sound plays
        }
        else if (!IsSleepTime())
        {
            yawningPlayed = false;  // Reset flag when it's not sleep time
        }

        RotateSun();
        UpdateLightColor();
        UpdateTimeText();
    }

    private IEnumerator PlaySoundsInSequence()
    {
        // Play notification sound first
        audioSource.PlayOneShot(notificationSound);
    
        // Wait for the notification sound to finish
        yield return new WaitForSeconds(notificationSound.length);
    
        // After notification sound, play yawning sound
        audioSource.PlayOneShot(yawningSound);
    }


    void PlayYawningSound()
    {
        // Pusti zvuk yawning
        if (audioSource != null && yawningSound != null)
        {
            audioSource.PlayOneShot(yawningSound);
        }
    }


    public void Sleep()
    {
        if (!isSleeping)
        {
            StartCoroutine(SleepRoutine());
        }
    }

    private IEnumerator SleepRoutine()
    {
        isSleeping = true;

        // Fade to black
        yield return StartCoroutine(FadeCanvasGroup(0f, 1f, 2.5f));

        // Simulacija spavanja: nova jutarnja scena
        timeOfDay = GetTimeNormalizedFromHour(8);

        currentDay++;
        PlayerPrefs.SetInt("Day", currentDay);
        UpdateDayText();

        yield return new WaitForSeconds(0.5f);

        // Fade from black
        yield return StartCoroutine(FadeCanvasGroup(1f, 0f, 2.5f));

        isSleeping = false;
    }

    public bool IsSleepTime()
    {
        float hour = timeOfDay * 24f;
        return (hour >= 21f || hour < 5f);
    }
    
    private IEnumerator FadeCanvasGroup(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            sleepFader.alpha = alpha;
            yield return null;
        }

        sleepFader.alpha = to;
    }

    void RotateSun()
    {
        float sunRotation = Mathf.Lerp(-90f, 270f, timeOfDay);
        sunLight.transform.rotation = Quaternion.Euler(sunRotation, 0f, 0f);
    }

    void UpdateLightColor()
    {
        sunLight.color = lightColor.Evaluate(timeOfDay);
    }

    void UpdateTimeText()
    {
        float totalHours = timeOfDay * 24f;
        int hours = Mathf.FloorToInt(totalHours);
        int minutes = Mathf.FloorToInt((totalHours - hours) * 60f);
        timeText.text = string.Format("{0:00}:{1:00}", hours, minutes);
    }

    void UpdateDayText()
    {
        dayText.text = "Day: " + currentDay.ToString();
        Debug.Log("Novi dan");
        if (playerInjurySystem != null)
            playerInjurySystem.OnDayPassed();
        if(potionManager.ShieldPotionDays>0)
            potionManager.ShieldPotionDays--;
        if(potionManager.FocusPotionDays>0)
            potionManager.FocusPotionDays--;
    }

    float GetTimeNormalizedFromHour(int hour)
    {
        return (hour % 24) / 24f;
    }
    
    
}
