using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueUI;

    // Lista dijaloga koje želimo prikazivati
    private string[] dialogues =  {"Ah, a face I haven’t seen in ages! Or maybe I’ve seen it just 10 minutes ago... Time is weird in here!" , "Weapons, seeds, potions... pick your poison. Literally."};
    private int currentDialogueIndex = 0;  // Trenutni indeks dijaloga

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        // Provjera za pritisak tipke F
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Ako imamo više dijaloga, promijeni dijalog
            if (currentDialogueIndex < dialogues.Length)
            {
                ShowDialogue(dialogues[currentDialogueIndex]);
                currentDialogueIndex++;
            }
            else
            {
                // Ako su svi dijalozi ispisani, zatvori dijalog
                EndDialogue();
            }
        }
    }

    public void StartDialogue()
    {
        dialogueUI.SetActive(true);  // Aktiviraj UI
        currentDialogueIndex = 0;  // Resetiraj na početni dijalog
        ShowDialogue(dialogues[currentDialogueIndex]);  // Prikazivanje prvog dijaloga
        currentDialogueIndex++;  // Povećaj indeks za sljedeći dijalog
    }

    public void ShowDialogue(string text)
    {
        dialogueText.text = text;  // Prikazivanje teksta u UI
    }

    public void EndDialogue()
    {
        dialogueUI.SetActive(false);  // Sakrij dijalog UI
        currentDialogueIndex = 0;  // Resetiraj dijalog za sljedeći put
    }
}