using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class NPCProximityInteraction : MonoBehaviour
{
    [Header("Typewriter Sound")]
    public AudioSource typewriterAudioSource;
    public AudioClip typewriterClip;
    public int clicksPerCharacter = 3;  // play one click every N chars

    
    [Header("Dialogue Settings")]
    public string[] sentences;
    public GameObject floatingTextPrefab;
    public bool interactable = true;

    [Header("Text Effects")]
    public bool useTypewriterEffect = true;
    public bool useWobbleEffect = false;
    public bool useColorizeText = true;

    [Header("Cooldown Settings")]
    public float cooldownTime = 30f;
    private float lastInteractionTime = -Mathf.Infinity;

    private GameObject floatingTextInstance;
    private int lastIndex = -1;
    private List<int> shuffledIndices = new List<int>();
    private int shufflePointer = 0;

    public AchievementsController achievementsController;

    private void Start()
    {
        achievementsController = FindObjectOfType<AchievementsController>();
        ShuffleIndices();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (sentences.Length == 0) return;
        if (other.CompareTag("Player") && Time.time - lastInteractionTime >= cooldownTime)
        {
            lastInteractionTime = Time.time;

           if (achievementsController.InteractWithNPCs < achievementsController.InteractWithNPCsGoal)
               achievementsController.InteractWithNPCs++;

            ShowRandomSentence();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && floatingTextInstance)
        {
            Destroy(floatingTextInstance);
        }
    }

    void ShowRandomSentence()
    {
        if (floatingTextInstance) return;

        int index = GetNextShuffledIndex();
        string selectedSentence = sentences[index];

        floatingTextInstance = Instantiate(floatingTextPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
        floatingTextInstance.transform.SetParent(transform);

        TextMeshPro tmp = floatingTextInstance.GetComponentInChildren<TextMeshPro>();
        if (tmp != null)
        {
            if (useColorizeText)
                selectedSentence = $"<color=#FFD700>{selectedSentence}</color>"; // Golden sarcastic tone

            if (useTypewriterEffect)
            {
                StartCoroutine(TypeText(tmp, selectedSentence));
            }
            else
            {
                tmp.text = selectedSentence;
            }

            if (useWobbleEffect)
            {
                StartCoroutine(WobbleText(tmp));
            }
        }
        else
        {
//             Debug.LogWarning("No TextMeshPro found in floating text prefab.");
        }
    }

    int GetNextShuffledIndex()
    {
        if (sentences.Length <= 1)
            return 0;

        if (shufflePointer >= shuffledIndices.Count)
        {
            ShuffleIndices();
        }

        return shuffledIndices[shufflePointer++];
    }

    void ShuffleIndices()
    {
        shuffledIndices.Clear();
        for (int i = 0; i < sentences.Length; i++)
        {
            if (i != lastIndex) shuffledIndices.Add(i);
        }

        for (int i = 0; i < shuffledIndices.Count; i++)
        {
            int rand = Random.Range(i, shuffledIndices.Count);
            int temp = shuffledIndices[i];
            shuffledIndices[i] = shuffledIndices[rand];
            shuffledIndices[rand] = temp;
        }

        shufflePointer = 0;
        if (shuffledIndices.Count > 0)
        {
            lastIndex = shuffledIndices[shuffledIndices.Count - 1];
        }
    }

    IEnumerator TypeText(TextMeshPro tmp, string sentence)
    {
        tmp.text = "";
        int counter = 0;
        foreach (char c in sentence)
        {
            tmp.text += c;

            if (typewriterAudioSource != null 
                && typewriterClip != null 
                && counter % clicksPerCharacter == 0)
            {
                typewriterAudioSource.PlayOneShot(typewriterClip);
            }

            counter++;
            yield return new WaitForSeconds(0.03f);
        }
    }


    IEnumerator WobbleText(TextMeshPro tmp)
    {
        Vector3 baseScale = tmp.transform.localScale;
        float t = 0;
        while (t < 2.5f)
        {
            tmp.transform.localScale = baseScale + new Vector3(
                Mathf.Sin(Time.time * 1f) * 0.05f,
                Mathf.Cos(Time.time * 1f) * 0.05f,
                0f
            );
            t += Time.deltaTime;
            yield return null;
        }
        tmp.transform.localScale = baseScale;
    }
}
