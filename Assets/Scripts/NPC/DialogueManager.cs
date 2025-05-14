using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public TextMeshProUGUI dialogueText;
    public GameObject dialogueUI;

    public AudioClip typeSound; // Dodaj u Inspectoru zvuk kucanja
    private AudioSource audioSource;

    private string[] dialogues = {
        "Ah, a face I haven’t seen in ages! Or maybe I’ve seen it just 10 minutes ago... Time is weird in here!",
        "Weapons, potions... pick your poison. Literally."
    };

    private int currentDialogueIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Only respond to F key if dialogueUI is active
        if (dialogueUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (isTyping)
                {
                    // Prekini efekat i prikaži pun tekst odmah
                    StopCoroutine(typingCoroutine);
                    dialogueText.text = dialogues[currentDialogueIndex - 1];
                    isTyping = false;
                }
                else if (currentDialogueIndex < dialogues.Length)
                {
                    ShowDialogue(dialogues[currentDialogueIndex]);
                    currentDialogueIndex++;
                }
                else
                {
                    EndDialogue();
                }
            }
        }
    }


    public void StartDialogue()
    {
        dialogueUI.SetActive(true);
        currentDialogueIndex = 0;
        ShowDialogue(dialogues[currentDialogueIndex]);
        currentDialogueIndex++;
    }

    public void ShowDialogue(string text)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(text));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        int charIndex = 0;

        foreach (char letter in sentence)
        {
            dialogueText.text += letter;

            if (typeSound != null && letter != ' ' && charIndex % 2 == 0)
                audioSource.PlayOneShot(typeSound);

            charIndex++;
            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;
    }


    public void EndDialogue()
    {
        dialogueUI.SetActive(false);
        currentDialogueIndex = 0;
    }
}
