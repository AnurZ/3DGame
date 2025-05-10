using UnityEngine;
using System.Collections;
using TMPro;

public class MysticNPCTrade : MonoBehaviour
{
    [Header("Trade Settings")]
    public Item requiredWoodItem;
    public string requiredWoodItemName;
    public int requiredWoodAmount = 5;
    public Item potionItem;

    [Header("Dialogue UI")]
    public GameObject floatingTextPrefab;
    public float dialogueDelay = 2f;       // Time between each full line
    public float finalChoiceDuration = 6f; // How long to show final messages

    [Header("Typewriter Effect")]
    public bool useTypewriterEffect = true;
    public float typewriterSpeed = 0.03f;  // Seconds per character

    private GameObject floatingTextInstance;
    private TextMeshPro textMesh;

    private bool playerInRange   = false;
    private bool awaitingChoice  = false;
    private bool hasSeenDialogue = false;
    private bool hasTraded       = false;

    private string[] dialogueLines;

    private void Start()
    {
        dialogueLines = new string[]
        {
            "Ah... A traveler in these forgotten woods...",
            "Few pass through without a purpose...",
            "Perhaps, you could help me.",
            $"Bring me {requiredWoodAmount} {requiredWoodItemName}...",
            "And I shall reward you with this enchanted potion."
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Reset any running dialogue
        StopAllCoroutines();
        HideFloatingText();
        awaitingChoice = false;
        playerInRange = true;

        if (hasTraded)
        {
            // After trade, show a one-time post-trade tease
            StartCoroutine(ShowTemporaryLine(
                "You’ve proven worthy. If you return later, perhaps I’ll offer you something better.",
                finalChoiceDuration
            ));
        }
        else if (!hasSeenDialogue)
        {
            // Full quest intro
            StartCoroutine(PlayDialogue());
        }
        else
        {
            // Already saw intro, just prompt Y/N
            StartCoroutine(ShowOfferPrompt());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        StopAllCoroutines();
        HideFloatingText();
        awaitingChoice = false;
        playerInRange = false;
    }

    private void Update()
    {
        if (!awaitingChoice || !playerInRange)
            return;

        if (Input.GetKeyDown(KeyCode.Y))
        {
            awaitingChoice = false;
            StopAllCoroutines();
            StartCoroutine(HandleTrade());
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            awaitingChoice = false;
            StopAllCoroutines();
            StartCoroutine(ShowTemporaryLine("Very well... Perhaps another time.", finalChoiceDuration));
        }
    }

    private IEnumerator PlayDialogue()
    {
        foreach (string line in dialogueLines)
        {
            yield return StartCoroutine(ShowLine(line));
            yield return new WaitForSeconds(dialogueDelay);
        }

        hasSeenDialogue = true;
        yield return StartCoroutine(ShowOfferPrompt());
    }

    private IEnumerator ShowOfferPrompt()
    {
        yield return StartCoroutine(ShowLine(
            $"[Y]es to accept, [N]o to decline."
        ));
        awaitingChoice = true;
    }

    private IEnumerator HandleTrade()
    {
        // Count wood in inventory
        var inv = InventoryManager.Instance;
        int totalWood = 0;
        foreach (var slot in inv.inventorySlots)
        {
            var invItem = slot.GetComponentInChildren<InventoryItem>();
            if (invItem != null && invItem.item == requiredWoodItem)
                totalWood += invItem.count;
        }

        if (totalWood >= requiredWoodAmount)
        {
            // Remove the required amount
            int toRemove = requiredWoodAmount;
            foreach (var slot in inv.inventorySlots)
            {
                var invItem = slot.GetComponentInChildren<InventoryItem>();
                if (invItem != null && invItem.item == requiredWoodItem && toRemove > 0)
                {
                    int removed = Mathf.Min(invItem.count, toRemove);
                    invItem.count -= removed;
                    toRemove -= removed;
                    if (invItem.count <= 0) Destroy(invItem.gameObject);
                    else invItem.RefreshCount();
                }
                if (toRemove <= 0) break;
            }

            // Give potion and mark traded
            inv.AddItem(potionItem);
            hasTraded = true;
            yield return StartCoroutine(ShowLine("The forest thanks you... Take this potion."));
        }
        else
        {
            yield return StartCoroutine(ShowLine(
                $"You only have {totalWood} {requiredWoodItemName}, but need {requiredWoodAmount}."
            ));
        }

        yield return new WaitForSeconds(finalChoiceDuration);
        HideFloatingText();
    }

    // --- UI Helpers ---

    private IEnumerator ShowLine(string message)
    {
        CreateFloatingText();

        if (useTypewriterEffect)
            yield return StartCoroutine(TypeText(message));
        else
            textMesh.text = message;
    }

    private IEnumerator TypeText(string sentence)
    {
        textMesh.text = "";
        foreach (char c in sentence)
        {
            textMesh.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }

    private IEnumerator ShowTemporaryLine(string message, float duration)
    {
        yield return StartCoroutine(ShowLine(message));
        yield return new WaitForSeconds(duration);
        HideFloatingText();
    }

    private void CreateFloatingText()
    {
        if (floatingTextInstance)
            Destroy(floatingTextInstance);

        floatingTextInstance = Instantiate(
            floatingTextPrefab,
            transform.position + Vector3.up * 2f,
            Quaternion.identity
        );
        floatingTextInstance.transform.SetParent(transform, true);
        textMesh = floatingTextInstance.GetComponentInChildren<TextMeshPro>();
    }

    private void HideFloatingText()
    {
        if (floatingTextInstance)
            Destroy(floatingTextInstance);

        floatingTextInstance = null;
        textMesh = null;
    }
}
