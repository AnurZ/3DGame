using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SimpleDayNightCycle : MonoBehaviour
{
    [Header("UI to Hide During Sleep")]
    public GameObject[] uiToHide;

    public Light sunLight;
    public Gradient lightColor;
    public float fullDayLengthInSeconds = 480f;
    
    public AudioSource audioSource;
    public AudioClip yawningSound;
    //Anur
    private List<string> goodQueue;
    private List<string> badQueue;
    [Header("Dream Sequence")]
    public TextMeshProUGUI dreamText;             // TMP polje u canvasu, isključeno na startu
    public AudioSource typewriterAudioSource;     // AudioSource za typewriter klik
    public AudioClip typewriterClip;
    public AudioSource thunderAudioSource;        // AudioSource za grmljavinu
    public AudioClip thunderClip;
    public float typingSpeed = 0.05f;             // brzina slova

    [TextArea(2,5)]
    public string[] goodDreamLines;               // popuni u Inspectoru
    [TextArea(2,5)]
    public string[] badDreamLines;
    
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
    
    public AchievementsController achievementsController;

    public int lastHour = -1;
    public StaminaController staminaRegenController;
    public TreeSpawner treeSpawner;
    
    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    public void SetFirstDay()
    {
        currentDay = 1;
        UpdateDayText();
        PlayerPrefs.SetInt("Day", currentDay);
    }
    
    private void Awake()
    {
        // … postojeći Awake kod …

        // Napuni i promiješaj queue-e
        goodQueue = new List<string>(goodDreamLines);
        Shuffle(goodQueue);

        badQueue  = new List<string>(badDreamLines);
        Shuffle(badQueue);
    }
    
    private string GetNextDream(bool isBad)
    {
        var queue = isBad ? badQueue : goodQueue;
        if (queue.Count == 0)
        {
            // Queue je prazan → refill i shuffle
            var source = isBad ? badDreamLines : goodDreamLines;
            queue.AddRange(source);
            Shuffle(queue);
        }
        // Uzmi prvi i ukloni ga
        string line = queue[0];
        queue.RemoveAt(0);
        return line;
    }
    
    void Start()
    {
        timeOfDay = GetTimeNormalizedFromHour(8);
        currentDay = PlayerPrefs.GetInt("Day", 1);
        potionManager = FindObjectOfType<PotionManager>();
        achievementsController = FindObjectOfType<AchievementsController>();
        treeSpawner = FindObjectOfType<TreeSpawner>();
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

    /*private IEnumerator SleepRoutine()
    {
        isSleeping = true;
        StaminaController.Instance.RestoreFullStamina();
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
    */
    private IEnumerator TypeText(string sentence)
    {
        dreamText.text = "";
        for (int i = 0; i < sentence.Length; i++)
        {
            dreamText.text += sentence[i];
            if (typewriterAudioSource != null && typewriterClip != null && i % 3 == 0)
                typewriterAudioSource.PlayOneShot(typewriterClip);
            yield return new WaitForSeconds(typingSpeed);
        }
    }
    public AudioSource MusicPlayer;
    private IEnumerator SleepRoutine()
    {
        isSleeping = true;
        foreach (var go in uiToHide)
            if (go != null) go.SetActive(false);

        StaminaController.Instance.RestoreFullStamina();
        treeSpawner.SpawnTrees();

        bool isBadDream = Random.value < 0.5f;
        
        // Stop background music BEFORE typing starts
        if (isBadDream && MusicPlayer.isPlaying)
        {
            MusicPlayer.Pause();
        }
        
        // 1) Fade to black
        yield return StartCoroutine(FadeCanvasGroup(0f, 1f, 2.5f));

        // 2) --- DREAM SEQUENCE START ---
        if (dreamText != null)
            dreamText.gameObject.SetActive(true);

        // Advance time to 8 AM and increment day
        timeOfDay = GetTimeNormalizedFromHour(8);
        currentDay++;
        PlayerPrefs.SetInt("Day", currentDay);
        UpdateDayText();

        // Snap sun & color
        RotateSun();
        UpdateLightColor();

        // Determine dream type
        

        // Play dream text
        string line = GetNextDream(isBadDream);
        yield return StartCoroutine(TypeText(line));

        // Play thunder if bad dream
        if (isBadDream && thunderAudioSource != null && thunderClip != null)
        {
            yield return new WaitForSeconds(0.5f);
            thunderAudioSource.PlayOneShot(thunderClip);
        }

        // Wait and resume background music
        if (isBadDream)
        {
            yield return new WaitForSeconds(1.5f);
            MusicPlayer.UnPause();
        }

        yield return new WaitForSeconds(2f);
        if (dreamText != null) dreamText.gameObject.SetActive(false);
        // 2) --- DREAM SEQUENCE END ---

        yield return new WaitForSeconds(0.5f);

        // 3) Fade back in
        yield return StartCoroutine(FadeCanvasGroup(1f, 0f, 2.5f));

        foreach (var go in uiToHide)
            if (go != null) go.SetActive(true);

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
        if (lastHour != hours)
        {
            if(potionManager.UpgradePotionHours>0)
                potionManager.UpgradePotionHours--;
            if(potionManager.ShieldPotionHours>0)
                potionManager.ShieldPotionHours--;
            if(potionManager.FocusPotionHours>0)
                potionManager.FocusPotionHours--;
        }
        lastHour = hours;
        int minutes = Mathf.FloorToInt((totalHours - hours) * 60f);
        timeText.text = string.Format("{0:00}:{1:00}", hours, minutes);
    }

    void UpdateDayText()
    {
        dayText.text = "Day: " + currentDay.ToString();
        Debug.Log("Novi dan");
        if (achievementsController != null)
        {
            if (achievementsController.DrinkPotionsDaily > achievementsController.PotionsDrankUntilYesterday)
            {
                achievementsController.PotionsDrankUntilYesterday++;
            }
            else
            {
                achievementsController.DrinkPotionsDaily = 0;
                achievementsController.PotionsDrankUntilYesterday = 0;
            }

        }
        
        //currentDay++;
        if (playerInjurySystem != null)
            playerInjurySystem.OnDayPassed();
    }

    float GetTimeNormalizedFromHour(int hour)
    {
        return (hour % 24) / 24f;
    }
    
    
}
