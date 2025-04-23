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
        // Učitaj sačuvani broj dana iz PlayerPrefs, default je 1 ako nije sačuvano
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
        // Odredi koliko traje dan ili noć
        float duration = isDay ? dayDuration : nightDuration;
        cycleTime += Time.deltaTime / duration;

        // Ako dođe do kraja ciklusa dana/noći, resetiraj cycleTime i prebacivanje na dan/noć
        if (cycleTime >= 1f)
        {
            cycleTime = 0f;  // Resetiraj cycleTime

            isDay = !isDay;  // Prebacivanje iz dana u noć i obrnuto

            // Ako je novi dan, povećaj broj dana
            if (isDay)
            {
                dayCounter++;  // Povećaj broj dana
                UpdateDayCounter();  // Ažuriraj prikaz broja dana
                PlayerPrefs.SetInt("CurrentDay", dayCounter);  // Spremi broj dana u PlayerPrefs
            }

            // Ažuriraj zvukove (dan/noć)
            UpdateAudio();
            // Ažuriraj zvijezde (ako je noć)
            UpdateStars();
        }

        // Ažuriraj boju svjetlosti (dnevno/noćno svjetlo)
        if (directionalLight != null)
            directionalLight.color = lightColor.Evaluate(cycleTime);

        // Rotiraj sunce (sunPivot) u skladu s time
        if (sunPivot != null)
        {
            float totalDegrees = isDay ? 180f : 180f;
            float baseAngle = isDay ? 0f : 180f;

            // Izračunaj rotaciju sunca
            float sunRotation = baseAngle + totalDegrees * cycleTime;

            // Osiguraj da rotacija bude između 0° i 360°
            sunRotation = Mathf.Repeat(sunRotation, 360f); // Nastavi rotaciju bez skakanja u negativne kutove

            // Primijeni rotaciju sunca
            sunPivot.localRotation = Quaternion.Euler(sunRotation, 0f, 0f);
        }

        // Ažuriraj sat (prikaz vremena)
        UpdateClock();
    }
    else
    {
        // Kad igrač spava, prikaži fade efekt
        fadePanel.alpha = Mathf.MoveTowards(fadePanel.alpha, 1f, fadeSpeed * Time.deltaTime);
        if (fadePanel.alpha >= 1f)
        {
            // Kad igrač spava, resetiraj vrijeme na 08:00
            cycleTime = GetTimeFromHours(8);
            isDay = true;
            isSleeping = false;
            dayCounter++;  // Povećaj broj dana kad igrač spava
            UpdateDayCounter();
            PlayerPrefs.SetInt("CurrentDay", dayCounter);  // Spremi broj dana
            UpdateAudio();
            UpdateStars();
        }
    }

    // Ako igrač pritisne spacebar, započni spavanje
    if (canSleep && Input.GetKeyDown(KeyCode.Space))
    {
        isSleeping = true;
    }

    // Fade out efekt kad igrač nije spavao
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

   void UpdateClock()
{
    float totalHours = cycleTime * 24f;
    int hours = Mathf.FloorToInt(totalHours);
    int minutes = Mathf.FloorToInt((totalHours - hours) * 60f);

    string timeString = string.Format("{0:00}:{1:00}", hours, minutes);
    timeText.text = timeString;
}


    private void UpdateDayCounter()
    {
        if (dayText != null)
        {
            dayText.text = "Day: " + dayCounter.ToString();
        }
    }

    // Funkcija za pretvaranje sati u normalizirani cycleTime
    float GetTimeFromHours(int hours)
    {
        float minutes = hours * 60f;
        return minutes / 1440f; // 1440 minuta u pravom danu
    }
}
