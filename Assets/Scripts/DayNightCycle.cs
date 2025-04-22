using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayNightCycle : MonoBehaviour
{
    public Light directionalLight;
    public Gradient lightColor;
    public float dayDuration = 300f;  // 5 minuta
    public float nightDuration = 180f; // 3 minute

    public Transform sunPivot;
    public CanvasGroup fadePanel;
    public float fadeSpeed = 0.2f;  // fade in/out brzina (za ~5 sekundi)

    public AudioSource dayAudio;
    public AudioSource nightAudio;
    public GameObject stars;
    public TextMeshProUGUI timeText;  // Prikaz sata
    public TextMeshProUGUI dayText;   // Prikaz dana

    private float cycleTime;
    private bool isDay = true;
    private bool isSleeping = false;
    private bool canSleep = false;
    private int dayCounter = 1;

    private void Start()
    {
        // Učitaj sačuvani broj dana iz PlayerPrefs, default je 0 ako nije sačuvano
        dayCounter = PlayerPrefs.GetInt("CurrentDay", 1);

        // Postavljamo vrijeme tako da počne od 08:00 svakog dana
        cycleTime = GetTimeFromHours(8); // Početak vremena je 08:00 (480 minuta)
        if (fadePanel != null) fadePanel.alpha = 0f;
        UpdateAudio();
        UpdateStars();
        UpdateDayCounter();
    }

    private void Update()
    {
        if (!isSleeping)
        {
            float duration = isDay ? dayDuration : nightDuration;
            cycleTime += Time.deltaTime / duration;

            if (cycleTime >= 1f)
            {
                cycleTime = 0f;  // Resetiranje vremena svakog dana
                isDay = !isDay;

                if (isDay)
                {
                    dayCounter++;
                    UpdateDayCounter();
                    PlayerPrefs.SetInt("CurrentDay", dayCounter);  // Spremi trenutni dan
                    cycleTime = GetTimeFromHours(8); // Postavljanje vremena na 08:00 kada počinje novi dan
                }

                UpdateAudio();
                UpdateStars();
            }

            if (directionalLight != null)
                directionalLight.color = lightColor.Evaluate(cycleTime);

            if (sunPivot != null)
            {
                float totalDegrees = isDay ? 180f : 180f;
                float baseAngle = isDay ? 0f : 180f;
                sunPivot.localRotation = Quaternion.Euler(baseAngle + totalDegrees * cycleTime, 0f, 0f);
            }

            UpdateClock();
        }
        else
        {
            fadePanel.alpha = Mathf.MoveTowards(fadePanel.alpha, 1f, fadeSpeed * Time.deltaTime);
            if (fadePanel.alpha >= 1f)
            {
                cycleTime = 0f;
                isDay = true;
                isSleeping = false;
                dayCounter++;
                UpdateDayCounter();
                PlayerPrefs.SetInt("CurrentDay", dayCounter);  // Spremi trenutni dan nakon spavanja
                UpdateAudio();
                UpdateStars();
            }
        }

        if (canSleep && Input.GetKeyDown(KeyCode.Space))
        {
            isSleeping = true;
        }

        if (!isSleeping && fadePanel.alpha > 0f)
            fadePanel.alpha = Mathf.MoveTowards(fadePanel.alpha, 0f, fadeSpeed * Time.deltaTime);
    }

    public void StartSleep()
    {
        isSleeping = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bed"))
        {
            Debug.Log("Colliding with bed!");
            canSleep = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Bed"))
        {
            canSleep = false;
        }
    }

    private void UpdateAudio()
    {
        if (dayAudio != null && nightAudio != null)
        {
            dayAudio.loop = true;
            nightAudio.loop = true;

            if (isDay)
            {
                if (!dayAudio.isPlaying) dayAudio.Play();
                nightAudio.Stop();
            }
            else
            {
                if (!nightAudio.isPlaying) nightAudio.Play();
                dayAudio.Stop();
            }
        }
    }

    private void UpdateStars()
    {
        if (stars != null)
            stars.SetActive(!isDay);
    }

    private void UpdateClock()
    {
        if (timeText != null)
        {
            // Računanje trenutnog vremena na temelju cycleTime
            float totalMinutes = (isDay ? cycleTime * dayDuration : cycleTime * nightDuration);
            int minutes = Mathf.FloorToInt(totalMinutes) % 60;
            int hours = Mathf.FloorToInt(totalMinutes) / 60;

            // Prikazivanje vremena na ekranu
            timeText.text = string.Format("{0:D2}:{1:D2}", hours, minutes);
        }
    }

    private void UpdateDayCounter()
    {
        if (dayText != null)
        {
            dayText.text = "Day: " + dayCounter.ToString();
        }
    }

    // Funkcija za pretvaranje sati u normalizirani cycleTime
    private float GetTimeFromHours(int hours)
    {
        // 480 minuta = 8 sati
        float minutes = hours * 60f;
        float totalMinutesInDayCycle = dayDuration + nightDuration;
        return minutes / totalMinutesInDayCycle;
    }
}
