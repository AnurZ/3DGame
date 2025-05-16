using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    public bool ticketPurchased = false;
    private bool hasShownTicketPopup = false;
    public bool hasFinishedDialogue = false;

    [Header("Typewriter Settings")]
    public float typewriterSpeed = 0.05f;
    public AudioSource typewriterAudioSource;
    public AudioClip typewriterClip;

    private Coroutine typeTextCoroutine;

    
    [Header("Dialog Settings")]
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;
    [TextArea]
    public string[] dialogLines = {
        "Hey you, I’m down here!",
        "Oh look, another hero in shabby armor—just what I needed today.",
        "Funny thing—my boat only sails for paying customers.",
        "Got exactly 15 thousand coins, or shall we keep chatting?"
    };
    public KeyCode nextKey = KeyCode.F;

    private int currentLineIndex = 0;
    private bool isPlayerInRange = false;
    private bool dialogFinished = false;

    [Header("Boat")]
    public BoatSway boatSway; // Referenca na BoatSway

    private IEnumerator TypeText(string sentence)
    {
        dialogText.text = "";
        int counter = 0;

        foreach (char c in sentence)
        {
            dialogText.text += c;

            // Play the typewriter sound every 3rd character
            if (typewriterAudioSource != null && typewriterClip != null && counter % 3 == 0)
            {
                typewriterAudioSource.PlayOneShot(typewriterClip);
            }

            counter++;
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }

    private void Start()
    {
        dialogPanel.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerInRange && dialogPanel.activeSelf && Input.GetKeyDown(nextKey))
        {
            currentLineIndex++;
            if (currentLineIndex < dialogLines.Length)
            {
                ShowDialogLine(currentLineIndex);
            }
            else
            {
                dialogFinished = true;
                hasFinishedDialogue = true;

                CloseDialog();

                if (boatSway != null && !hasShownTicketPopup)
                {
                    boatSway.ShowTicketPopup();
                    hasShownTicketPopup = true;
                }
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!ticketPurchased && other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            currentLineIndex = 0;
            dialogFinished = false;
            hasShownTicketPopup = false;
            ShowDialogLine(currentLineIndex);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            CloseDialog();
        }
    }

    private void ShowDialogLine(int index)
    {
        dialogPanel.SetActive(true);

        if (typeTextCoroutine != null)
        {
            StopCoroutine(typeTextCoroutine);
        }

        typeTextCoroutine = StartCoroutine(TypeText(dialogLines[index]));
    }


    public void CloseDialog()
    {
        dialogPanel.SetActive(false);
    }
}
