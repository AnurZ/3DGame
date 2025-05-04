using System;
using TMPro;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    public bool ticketPurchased = false;

    
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
    public KeyCode nextKey = KeyCode.G;

    private int currentLineIndex = 0;
    private bool isPlayerInRange = false;
    private bool dialogFinished = false;

    [Header("Boat")]
    public BoatSway boatSway; // Referenca na BoatSway

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
                CloseDialog();

                // Automatski prikaži ticket popup
                if (boatSway != null)
                {
                    boatSway.ShowTicketPopup();
                    dialogFinished = false; // Reset ako se koristi više puta
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ticketPurchased) return;
        
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            currentLineIndex = 0;
            dialogFinished = false;
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
        dialogText.text = dialogLines[index];
    }

    public void CloseDialog()
    {
        dialogPanel.SetActive(false);
    }
}
