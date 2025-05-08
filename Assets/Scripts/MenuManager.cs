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
    public Image blackScreenNewGame;
    public Image blackScreenOnLoad;
    public float fadeDuration = 1f;
    public NewGameInventoryManager newGameInventoryManager;
    public Image logoImage;

    
    private string savePath;

    private void Start()
    {
        savePath = Application.persistentDataPath + "/savefile.json";

        continueButton.interactable = File.Exists(savePath);

        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (helpPanel != null) helpPanel.SetActive(false);

        blackScreen.gameObject.SetActive(false);
        blackScreenNewGame.gameObject.SetActive(false);

        // Only activate and fade out the on-load screen here
        if (blackScreenOnLoad != null)
        {
            blackScreenOnLoad.gameObject.SetActive(true);
            StartCoroutine(FadeFromBlack());
        }
    }


    public void OnContinue()
    {
        if (File.Exists(savePath))
            StartCoroutine(LoadGameWithFade());
    }

    public void OnNewGame()
    {
        SaveManager.IsNewGame = true;
        if (File.Exists(savePath))
            File.Delete(savePath);

        // Ovdje pozovi CreateDefaultInventory odmah nakon što izbrišeš save file
        var newInvManager = FindObjectOfType<NewGameInventoryManager>();
        if (newInvManager != null)
        {
            newInvManager.CreateDefaultInventory();
        }
        else
        {
            Debug.LogWarning("⚠️ NewGameInventoryManager nije pronađen!");
        }

        SceneManager.sceneLoaded += OnSceneLoadedNewGame;
        StartCoroutine(StartNewGameWithFade());
    }

    
    private void OnSceneLoadedNewGame(Scene scene, LoadSceneMode mode)
    {
        // Dodaj čekanje na potpunu inicijalizaciju scene
        StartCoroutine(InitializeInventoryAfterSceneLoaded());
    }

    private IEnumerator InitializeInventoryAfterSceneLoaded()
    {
        yield return new WaitForEndOfFrame(); // Čekaj do kraja frame-a da scena bude potpuno inicijalizirana

        var newInvManager = FindObjectOfType<NewGameInventoryManager>();
        if (newInvManager != null)
        {
            newInvManager.CreateDefaultInventory();
            Debug.Log("✅ Default inventar kreiran nakon učitavanja nove scene.");
        }
        else
        {
            Debug.LogWarning("⚠️ NewGameInventoryManager nije pronađen u novoj sceni!");
        }

        // Otkaži registraciju metode nakon prvog poziva
        SceneManager.sceneLoaded -= OnSceneLoadedNewGame;
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
        // Fade out black screen
        Color blackColor = blackScreenOnLoad.color;
        logoImage.gameObject.SetActive(true);
        Color logoColor = logoImage.color;
        logoColor.a = 0f;
        logoImage.color = logoColor;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            blackColor.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            logoColor.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            logoImage.color = logoColor;
            blackScreenOnLoad.color = blackColor;
            yield return null;
        }
        blackColor.a = 0f;
        blackScreenOnLoad.color = blackColor;
        blackScreenOnLoad.gameObject.SetActive(false);

        // Fade in logo
      
            

            
            logoColor.a = 1f;
            logoImage.color = logoColor;
        
    }


    IEnumerator LoadGameWithFade()
    {
        yield return StartCoroutine(FadeToBlack());
        SceneManager.LoadScene("IGRICASCENE"); // zamijeni sa tačnim imenom tvoje scene
    }

    IEnumerator StartNewGameWithFade()
    {
        yield return StartCoroutine(FadeToBlackNewGame());
        Debug.Log("🌑 Fade završen. Učitavanje scene...");
        SceneManager.LoadScene("IGRICASCENE");
    }


    IEnumerator FadeToBlack()
    {
        blackScreen.gameObject.SetActive(true);
        Color color = blackScreen.color;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            blackScreenOnLoad.color = color;
            yield return null;
        }
        color.a = 1f;
        blackScreenOnLoad.color = color;
    }IEnumerator FadeToBlackNewGame()
    {
        blackScreenNewGame.gameObject.SetActive(true);
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
