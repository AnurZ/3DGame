using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    private PlayerController player;

    
    [Header("UI References")]
    public GameObject pauseCanvas;      // Referenca na PauseCanvas
    public Button resumeButton;         // (opcionalno) fokus nakon pauze

    private bool isPaused = false;


    private void Start()
    {
        pauseCanvas.SetActive(false);
        player = FindObjectOfType<PlayerController>();

    }

    void Update()
    {
        if (player != null && player.isInShop)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        // TimeScale
        Time.timeScale = isPaused ? 0f : 1f;

        // CanvasGroup ili GameObject aktivacija
        pauseCanvas.SetActive(isPaused);

        // Ako koristimo CanvasGroup:
        /*
        var cg = pauseCanvas.GetComponent<CanvasGroup>();
        cg.alpha = isPaused ? 1f : 0f;
        cg.interactable = isPaused;
        cg.blocksRaycasts = isPaused;
        */

        // postavi fokus na Resume (ako želiš da Space/Enter automatski odabere)
        if (isPaused && resumeButton != null)
        {
            resumeButton.Select();
        }
    }

    // Pozovi iz UI gumba
    public void OnResumeButton()
    {
        if (isPaused) TogglePause();
    }

    public void OnMainMenuButton()
    {
        // Vrati timeScale prije promjene scene
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnQuitButton()
    {
        FindAnyObjectByType<SaveManager>().SaveGame();
        Application.Quit();
    }
}