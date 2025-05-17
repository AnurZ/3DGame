using UnityEngine;
using System.Collections;
using TMPro;



public class MysticNPCTrade : MonoBehaviour
{
    public enum QuestStage { First, Second, Third, Completed }
    private QuestStage currentStage = QuestStage.First;

    [Header("Trade Settings")]
    public Item requiredWoodItem;
    public string requiredWoodItemName;
    public int requiredWoodAmount = 20;
    public Item potionItem;

    public Item rarerWoodItem;
    public string rarerWoodItemName;
    public int rarerWoodAmount = 10;
    public int coinReward = 200;

    public int finalCoinCost = 500;
    public Item saplingItem;
    public int saplingAmount = 10;
    public Item upgradedAxe;

    [Header("Dialogue UI")]
    public GameObject floatingTextPrefab;
    public float dialogueDelay = 2f;
    public float finalChoiceDuration = 6f;

    [Header("Typewriter Effect")]
    public bool useTypewriterEffect = true;
    public float typewriterSpeed = 0.03f;
    public AudioSource typewriterAudioSource;
    public AudioClip typewriterClip;
    public AudioClip SellCoinClip;
    public AudioClip BuyCoinClip;

    public float cooldownTimer;

    private GameObject floatingTextInstance;
    private TextMeshPro textMesh;
    private bool playerInRange = false;
    private bool awaitingChoice = false;
    private bool hasSeenDialogue = false;
    private bool hasTraded = false;
    private float questCooldownTimer = 0f;
    private bool isWaitingForNextQuest = false;

    private void Start()
    {
        LoadState();
    }

    private void Update()
    {
        if (isWaitingForNextQuest)
        {
            questCooldownTimer -= Time.deltaTime;
            if (questCooldownTimer <= 0f)
            {
                isWaitingForNextQuest = false;
                AdvanceQuestStage();
            }
        }


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

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        StopAllCoroutines();
        HideFloatingText();
        awaitingChoice = false;
        playerInRange = true;

        if (isWaitingForNextQuest)
        {
            StartCoroutine(ShowTemporaryLine("You’ve proven worthy. If you return later, perhaps I’ll offer you something better.", finalChoiceDuration));
        }
        else if (!hasSeenDialogue)
        {
            StartCoroutine(PlayDialogue());
        }
        else
        {
            if (currentStage == QuestStage.Completed)
            {
                StartCoroutine(ShowTemporaryLine("I have nothing more to offer, traveler.", finalChoiceDuration));
            }
            else
            {
                StartCoroutine(ShowOfferPrompt());
            }
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

        SaveState(); // Save when player leaves NPC
    }

    private IEnumerator PlayDialogue()
    {
        string[] lines = GetDialogueLines();
        foreach (string line in lines)
        {
            yield return StartCoroutine(ShowLine(line));
            yield return new WaitForSeconds(dialogueDelay);
        }

        hasSeenDialogue = true;
        yield return StartCoroutine(ShowOfferPrompt());
    }

    private IEnumerator ShowOfferPrompt()
    {
        if (currentStage == QuestStage.Completed)
        {
            yield break; // ne prikazuj ništa ako je završeno
        }

        yield return StartCoroutine(ShowLine("[Y]es to accept, [N]o to decline."));
        awaitingChoice = true;
    }


    private IEnumerator HandleTrade()
    {
        var inv = InventoryManager.Instance;

        switch (currentStage)
        {
            case QuestStage.First:
                if (TryRemoveItems(inv, requiredWoodItem, requiredWoodAmount, requiredWoodItemName, out string msg1))
                {
                    inv.AddItem(potionItem);
                    yield return StartCoroutine(ShowLine("The forest thanks you... Take this potion."));
                    //TransitionToNextQuest();
                    hasTraded = true;
                    isWaitingForNextQuest = true;
                    questCooldownTimer = cooldownTimer; // koristi public float cooldownTimer iz Inspector-a
                    SaveState(); // Save after trade
                    yield return new WaitForSeconds(finalChoiceDuration);
                    HideFloatingText();
                    yield break;
                }
                else yield return StartCoroutine(ShowLine(msg1));
                break;

            case QuestStage.Second:
                if (TryRemoveItems(inv, rarerWoodItem, rarerWoodAmount, rarerWoodItemName, out string msg2))
                {
                    CurrencyManager.Instance.AddMoney(coinReward);
                    yield return StartCoroutine(ShowLine("A fair trade... Here are your coins."));
                    GetComponent<AudioSource>().PlayOneShot(SellCoinClip);
                   // TransitionToNextQuest();
                    hasTraded = true;
                    isWaitingForNextQuest = true;
                    questCooldownTimer = cooldownTimer; // koristi public float cooldownTimer iz Inspector-a
                    SaveState(); // Save after trade
                    yield return new WaitForSeconds(finalChoiceDuration);
                    HideFloatingText();
                    yield break;
                }
                else yield return StartCoroutine(ShowLine(msg2));
                break;

            case QuestStage.Third:
                if (HasEnoughItem(inv, saplingItem, 10)) // Player must have 10 saplings
                {
                    RemoveItems(inv, saplingItem, 10); // Remove 10 saplings from inventory
                    CurrencyManager.Instance.AddMoney(200); // Give 200 coins as reward
                    GetComponent<AudioSource>().PlayOneShot(BuyCoinClip); // Play coin sound
                    yield return StartCoroutine(ShowLine("Thanks for the saplings! Here's 200 coins for you."));
                    currentStage = QuestStage.Completed;
                }
                else
                {
                    yield return StartCoroutine(ShowLine("You need to bring me 10 saplings to complete this trade."));
                }
                break;

        }

        hasTraded = true;
        SaveState(); // Save after trade
        yield return new WaitForSeconds(finalChoiceDuration);
        HideFloatingText();
    }

    private void TransitionToNextQuest()
    {
        hasTraded = false;
        hasSeenDialogue = false;
        AdvanceQuestStage();
    }

    private void AdvanceQuestStage()
    {
        if (currentStage == QuestStage.First)
            currentStage = QuestStage.Second;
        else if (currentStage == QuestStage.Second)
            currentStage = QuestStage.Third;

        hasTraded = false;
        hasSeenDialogue = false;

        SaveState(); // Save on stage advance
    }

    private string[] GetDialogueLines()
    {
        switch (currentStage)
        {
            case QuestStage.First:
                return new string[]
                {
                    "Ah... A traveler in these forgotten woods...",
                    "Few pass through without a purpose...",
                    "Perhaps, you could help me.",
                    $"Bring me {requiredWoodAmount} {requiredWoodItemName}...",
                    "And I shall reward you with this enchanted potion."
                };

            case QuestStage.Second:
                return new string[]
                {
                    "You returned... Impressive.",
                    $"I now require {rarerWoodAmount} {rarerWoodItemName}.",
                    $"In return, I shall give you {coinReward} coins.",
                    "Do we have a deal?"
                };

            case QuestStage.Third:
                return new string[]
                {
                    "You've done well to come this far.",
                    "Bring me 10 saplings, and I will reward you with 200 coins.",
                    "Do we have a deal?"
                };


            default:
                return new string[] { "I have nothing more to offer... for now. Perhaps in time, our paths will cross again." };
        }
    }

    private bool TryRemoveItems(InventoryManager inv, Item item, int amount, string name, out string failMessage)
    {
        int total = 0;
        foreach (var slot in inv.inventorySlots)
        {
            var invItem = slot.GetComponentInChildren<InventoryItem>();
            if (invItem != null && invItem.item == item)
                total += invItem.count;
        }

        if (total >= amount)
        {
            RemoveItems(inv, item, amount);
            failMessage = "";
            return true;
        }
        else
        {
            failMessage = $"You only have {total} {name}, but need {amount}.";
            return false;
        }
    }

    private void RemoveItems(InventoryManager inv, Item item, int amount)
    {
        int toRemove = amount;
        foreach (var slot in inv.inventorySlots)
        {
            var invItem = slot.GetComponentInChildren<InventoryItem>();
            if (invItem != null && invItem.item == item && toRemove > 0)
            {
                int removed = Mathf.Min(invItem.count, toRemove);
                invItem.count -= removed;
                toRemove -= removed;
                if (invItem.count <= 0) Destroy(invItem.gameObject);
                else invItem.RefreshCount();
            }
            if (toRemove <= 0) break;
        }
    }

    private bool HasEnoughItem(InventoryManager inv, Item item, int amount)
    {
        int total = 0;
        foreach (var slot in inv.inventorySlots)
        {
            var invItem = slot.GetComponentInChildren<InventoryItem>();
            if (invItem != null && invItem.item == item)
                total += invItem.count;
        }
        return total >= amount;
    }

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
        int counter = 0;
        foreach (char c in sentence)
        {
            textMesh.text += c;
            if (typewriterAudioSource && typewriterClip && counter % 3 == 0)
                typewriterAudioSource.PlayOneShot(typewriterClip);
            counter++;
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
        if (floatingTextInstance) Destroy(floatingTextInstance);
        floatingTextInstance = Instantiate(floatingTextPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
        floatingTextInstance.transform.SetParent(transform, true);
        textMesh = floatingTextInstance.GetComponentInChildren<TextMeshPro>();

        if (textMesh != null)
            textMesh.color = Color.black;
    }

    private void HideFloatingText()
    {
        if (floatingTextInstance) Destroy(floatingTextInstance);
        floatingTextInstance = null;
        textMesh = null;
    }

    private void SaveState()
    {
        PlayerPrefs.SetInt("MysticNPC_QuestStage", (int)currentStage);
        PlayerPrefs.SetInt("MysticNPC_HasSeenDialogue", hasSeenDialogue ? 1 : 0);
        PlayerPrefs.SetInt("MysticNPC_HasTraded", hasTraded ? 1 : 0);
        //PlayerPrefs.SetFloat("MysticNPC_Cooldown", isWaitingForNextQuest ? questCooldownTimer : 0f);
        PlayerPrefs.Save();
    }



    private void LoadState()
    {
        if (PlayerPrefs.HasKey("MysticNPC_QuestStage"))
        {
            currentStage = (QuestStage)PlayerPrefs.GetInt("MysticNPC_QuestStage");
            hasSeenDialogue = PlayerPrefs.GetInt("MysticNPC_HasSeenDialogue") == 1;
            hasTraded = PlayerPrefs.GetInt("MysticNPC_HasTraded") == 1;

            
        }
    }


}
