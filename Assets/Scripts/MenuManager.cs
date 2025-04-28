using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MenuManager : MonoBehaviour
{
    public Button continueButton;
    public Button newGameButton;
    public Button optionsButton;
    public Button helpButton;
    public Button quitButton;
    public GameObject optionsPanel;
    public GameObject helpPanel;
    public Image blackScreen;
    public float fadeDuration = 1f;

    private string savePath;

    private void Start()
    {
        savePath = Application.persistentDataPath + "/savefile.json";

        // Disable Continue if no save file
        if (!File.Exists(savePath))
        {
            continueButton.interactable = false;
        }
        else
        {
            continueButton.interactable = true;
        }

        // Hide options and help panels at start
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (helpPanel != null) helpPanel.SetActive(false);

        // Start with black screen, fade in
        blackScreen.gameObject.SetActive(true);
        StartCoroutine(FadeFromBlack());
    }

    public void OnContinue()
    {
        if (File.Exists(savePath))
            StartCoroutine(LoadGameWithFade());
    }

    public void OnNewGame()
    {
        if (File.Exists(savePath))
            File.Delete(savePath);

        StartCoroutine(StartNewGameWithFade());
    }

    public void OnOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
            helpPanel?.SetActive(false); // zatvori help ako je otvoren
        }
    }

    public void OnHelp()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(true);
            optionsPanel?.SetActive(false); // zatvori options ako je otvoren
        }
    }

    public void OnBackFromOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    public void OnBackFromHelp()
    {
        if (helpPanel != null) helpPanel.SetActive(false);
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    IEnumerator FadeFromBlack()
    {
        Color color = blackScreen.color;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            blackScreen.color = color;
            yield return null;
        }
        color.a = 0f;
        blackScreen.color = color;
        blackScreen.gameObject.SetActive(false);
    }

    IEnumerator LoadGameWithFade()
    {
        yield return StartCoroutine(FadeToBlack());
        SceneManager.LoadScene("IGRICASCENE"); // zamijeni sa tačnim imenom tvoje scene
    }

    IEnumerator StartNewGameWithFade()
    {
        yield return StartCoroutine(FadeToBlack());
        SceneManager.LoadScene("IGRICASCENE"); // zamijeni sa tačnim imenom tvoje scene
    }

    IEnumerator FadeToBlack()
    {
        blackScreen.gameObject.SetActive(true);
        Color color = blackScreen.color;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            blackScreen.color = color;
            yield return null;
        }
        color.a = 1f;
        blackScreen.color = color;
    }
}
