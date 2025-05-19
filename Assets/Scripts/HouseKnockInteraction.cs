using UnityEngine;
using System.Collections;
using TMPro;

public class HouseKnockInteraction : MonoBehaviour
{
    [Header("Knock Settings")]
    public AudioSource knockAudioSource;
    public AudioClip knockClip;
    public AudioClip typeWriterClip;
    public string knockMessage = "Press 'K' to knock";
    public string[] creepyMessages = {
        "No one answers...",
        "You hear the faint sound of something inside... or is it your imagination?",
        "A chill runs down your spine as the silence continues."
    };

    [Header("Text Settings")]
    public GameObject floatingTextPrefab;
    public bool useTypewriterEffect = true;
    public bool useColorizeText = true;

    private GameObject floatingTextInstance;
    private bool playerInRange = false;
    private int messageIndex = 0;  // Variable to track the current message

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowKnockPrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (floatingTextInstance)
            {
                Destroy(floatingTextInstance);
            }
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.K))
        {
            // Start knocking sound and typewriter effect sequentially
            StartCoroutine(HandleKnockAndCreepyText());
        }
    }

    private void ShowKnockPrompt()
    {
        if (floatingTextInstance) return;

        floatingTextInstance = Instantiate(floatingTextPrefab, transform.position + Vector3.up * 6f, Quaternion.identity);  // Raised text to 6f
        floatingTextInstance.transform.SetParent(transform);

        TextMeshPro tmp = floatingTextInstance.GetComponentInChildren<TextMeshPro>();
        if (tmp != null)
        {
            if (useColorizeText)
            {
                knockMessage = $"<color=#FFD700>{knockMessage}</color>"; // Golden tone for the message
            }

            tmp.text = knockMessage;
        }
        else
        {
//             Debug.LogWarning("No TextMeshPro found in floating text prefab.");
        }
    }

    private IEnumerator HandleKnockAndCreepyText()
    {
        // Play knock sound first and wait for it to finish
        if (knockAudioSource != null && knockClip != null)
        {
            knockAudioSource.PlayOneShot(knockClip);
            yield return new WaitForSeconds(knockClip.length); // Wait for the knock sound to finish
        }

        // Cycle through creepy messages sequentially
        string selectedMessage = creepyMessages[messageIndex];

        if (floatingTextInstance) Destroy(floatingTextInstance);  // Remove previous prompt

        floatingTextInstance = Instantiate(floatingTextPrefab, transform.position + Vector3.up * 6f, Quaternion.identity);  // Raised text to 6f
        floatingTextInstance.transform.SetParent(transform);

        TextMeshPro tmp = floatingTextInstance.GetComponentInChildren<TextMeshPro>();
        if (tmp != null)
        {
            if (useColorizeText)
            {
                selectedMessage = $"<color=#FFD700>{selectedMessage}</color>"; // Golden tone for the message
            }

            if (useTypewriterEffect)
            {
                yield return StartCoroutine(TypeText(tmp, selectedMessage));
            }
            else
            {
                tmp.text = selectedMessage;
            }
        }

        // Move to the next message, cycling back to the first one if needed
        messageIndex = (messageIndex + 1) % creepyMessages.Length;
    }

    private IEnumerator TypeText(TextMeshPro tmp, string sentence)
    {
        tmp.text = "";
        int counter = 0;
        foreach (char c in sentence)
        {
            tmp.text += c;

            if (counter % 3 == 0 && knockAudioSource != null && typeWriterClip != null)
            {
                // Play the typewriter sound effect for each character (you can adjust this)
                knockAudioSource.PlayOneShot(typeWriterClip);  // Use typewriter sound clip
            }

            counter++;
            yield return new WaitForSeconds(0.0344f);  // Adjust typing speed here
        }
    }
}
