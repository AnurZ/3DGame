using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneTransition : MonoBehaviour
{
    public float fadeDuration = 1.0f;  // Trajanje fade efekta

    void Start()
    {
        // Pokreni fade-in efekat na početku scene
        StartCoroutine(FadeToBlack());
    }

    IEnumerator FadeToBlack()
    {
        // Dodaj fade out efekat ovdje (možeš koristiti UI Image za crni ekran)
        yield return new WaitForSeconds(fadeDuration);
        
        // Kada fade završi, preći ćeš na kreditnu scenu
        SceneManager.LoadScene("CreditsScene");  // Naziv scene sa kreditima
    }
}