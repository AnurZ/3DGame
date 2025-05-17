using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;

public class BoatSway : MonoBehaviour
{
    public TMP_Text popupText2;
    private bool declinedPurchase = false;
    [Header("Ending Sequence")]
    public CanvasGroup fadeCanvas;           // CanvasGroup za fade in/out
    public float fadeDuration = 1f;          // trajanje fadea
    [TextArea]
    public string[] endingLines;             // par linija koje će se prikazivati
    public float lineDuration = 3f;          // koliko svaka linija ostaje na ekranu

    public GameObject creditsPanel;          // Panel koji sadrži sve credits
    public RectTransform creditsText;        // RectTransform teksta unutar scroll panela
    public float creditsScrollSpeed = 20f;   // brzina skrolanja creditsa
    
    private bool endingStarted = false;
    
    [Header("Ending Cutscene Audio")]
    public AudioSource backgroundMusicSource;  // assign your background music AudioSource here
    public AudioSource endingMusicSource;      // assign your ending music AudioSource here

    [Header("UI Root GameObjects")]
    public GameObject[] uiRoots;                // assign all UI root objects here in inspector

    // --- Ticket UI ---
    [Header("Ticket UI")]
    public GameObject ticketPopup;
    public TMP_Text popupText;
    public Button yesButton;
    public Button noButton;
    public int ticketPrice = 100;

    // --- Animator Controllers ---
    [Header("Animator Controllers")]
    public RuntimeAnimatorController normalAnimatorController;
    public RuntimeAnimatorController endingAnimatorController;

    // --- Player refs ---
    private Transform playerTransform;
    private Rigidbody playerRigidbody;
    private PlayerController playerControllerScript;

    private bool playerInside = false;
    private bool movementStarted = false;
    private bool awaitingPurchase = false;

    [Header("Boat Teleport Point")]
    public Transform boatTeleportPoint; // Gdje igrač stoji dok plovi

    [Header("Bridge Event Settings")]
    public Transform enterPoint1;
    public Transform walkToBridgePoint1;
    public Transform bridgeLookPoint1;
    public Transform returnToBoatPoint1;

    public Transform enterPoint2;
    public Transform walkToBridgePoint2;
    public Transform bridgeLookPoint2;
    public Transform returnToBoatPoint2;

    public Animator playerAnimator;
    public float chopDuration = 0.03f;

    public GameObject Bridge1;
    public GameObject Bridge2;

    [Header("Boat Movement Settings")]
    public Transform[] waypoints;
    public int[] pauseIndices;    // npr. [1, 3]
    public float moveSpeed = 2f;
    public float rotationSpeed = 90f;
    public float waypointThreshold = 0.5f;

    [Header("Bobbing & Tilt")]
    public float bobbingAmount = 0.2f, bobbingSpeed = 1.5f;
    public float tiltAmount = 1f, tiltSpeed = 1f;

    [Header("Boat Parts")]
    public Transform boatBody;

    private Vector3 startPos;
    private int currentWaypointIndex = 0;
    private bool reachedEnd = false;
    private bool isPaused = false;
    private bool isBridgeEventRunning = false;

    void Start()
    {
        Debug.Log("BoatSway Start called");
        startPos = transform.position;
        if (waypoints.Length == 0 || pauseIndices.Length < 2)
        {
            Debug.LogError("Assign at least two waypoints and two pauseIndices!");
            enabled = false;
        }

        // Hide popup at start and wire up buttons
        ticketPopup.SetActive(false);
        yesButton.onClick.RemoveAllListeners(); // prevent stacking
        yesButton.onClick.AddListener(OnTicketYes);

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(OnTicketNo);

    }

    void Update()
    {
        ApplyBobbing();
        ApplyTilt();

        // Dok ploviš, teleportiraj igrača na deck svakog framea
        if (movementStarted && !isBridgeEventRunning && playerTransform != null)
        {
            playerTransform.position = boatTeleportPoint.position;
            playerTransform.rotation = boatTeleportPoint.rotation;
        }

        // Plovidba
        if (movementStarted && !reachedEnd && !isPaused)
        {
            RotateTowardsWaypoint();
            MoveTowardsWaypoint();
        }

        // Ulazak u brod i F za kupnju ulaznic
        
        
        if (reachedEnd && !endingStarted)
        {
            endingStarted = true;
            StartCoroutine(PlayEndingSequence());
        }
    }

    private void setupScene()
    {
        foreach (var uiRoot in uiRoots)
        {
            uiRoot.SetActive(false);
        }
        Debug.Log("Disabled all ui");

        // Disable player input (you already have this in DisablePlayerMovement)
        DisablePlayerMovement();
        Debug.Log("Disabled playermovement");

        // Stop background music
        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
            backgroundMusicSource.Stop();

        // Play ending music loop
        if (endingMusicSource != null)
        {
            endingMusicSource.loop = true;
            endingMusicSource.Play();
        }
    }
    
    IEnumerator PlayEndingSequence()
    {
        // Disable all UI
        

        // 1) Fade to black
        float t = 0f;
        fadeCanvas.gameObject.SetActive(true);
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            fadeCanvas.alpha = t;
            yield return null;
        }

        creditsPanel.SetActive(false);
        // 2) Show ending lines text
        for (int i = 0; i < endingLines.Length; i++)
        {
            popupText2.text = endingLines[i];
            popupText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(lineDuration);
        }
        popupText2.gameObject.SetActive(false);

        // 3) Show credits panel and scroll
        creditsPanel.SetActive(true);
        creditsText.anchoredPosition = new Vector2(0, -creditsPanel.GetComponent<RectTransform>().rect.height);
    
        float endPos = creditsText.rect.height;
        while (creditsText.anchoredPosition.y < endPos)
        {
            creditsText.anchoredPosition += Vector2.up * (creditsScrollSpeed * Time.deltaTime);
            yield return null;
        }
    }


    public AudioClip Clip;
    
    private bool listenersAdded = false;

    public void ShowTicketPopup()
    {
        awaitingPurchase = true;
        ticketPopup.SetActive(true);
        popupText.text = $"Do you want to buy the ticket for 6500 coins?";

        if (!listenersAdded)
        {
            yesButton.onClick.RemoveAllListeners();
            noButton.onClick.RemoveAllListeners();

            yesButton.onClick.AddListener(OnTicketYes);
            noButton.onClick.AddListener(OnTicketNo);

            listenersAdded = true;
        }
    }


    public void OnTicketYes()
    {
        // Koristi CurrencyManager za potrošnju
        if (CurrencyManager.Instance.TrySpendMoney(ticketPrice))
        {
            Debug.Log("Ticket YES clicked from: " + this.name);
            yesButton.interactable = false;
            Debug.Log("Ticket price: " + ticketPrice);
            CurrencyManager.Instance.SetMoneyText();
            ticketPopup.SetActive(false);
            awaitingPurchase = false;
            FindObjectOfType<DialogSystem>()?.gameObject.SetActive(false);
            setupScene();
            StartCoroutine(BoardBoat());
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(Clip);
            popupText.text = "Not enough coins!";
        }
    }

    public void OnTicketNo()
    {
        ticketPopup.SetActive(false);
        awaitingPurchase = false;
        //declinedPurchase = true;
    }

    IEnumerator BoardBoat()
    {
        movementStarted = true;

        // Disable player movement and physics
        DisablePlayerMovement();
        yield return null;
    }

    void ApplyBobbing()
    {
        float y = startPos.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    void ApplyTilt()
    {
        float tiltZ = Mathf.Sin(Time.time * tiltSpeed) * tiltAmount;
        transform.localRotation = Quaternion.Euler(0f, transform.localEulerAngles.y, tiltZ);
    }

    void RotateTowardsWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Length) return;
        Vector3 dir = waypoints[currentWaypointIndex].position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;
        float targetY = Quaternion.LookRotation(dir).eulerAngles.y;
        float newY = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetY, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, newY, transform.localEulerAngles.z);
    }

    void MoveTowardsWaypoint()
    {
        Vector3 tgt = waypoints[currentWaypointIndex].position;
        tgt.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, tgt, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, tgt) < waypointThreshold)
        {
            if (currentWaypointIndex == pauseIndices[0] ||
                currentWaypointIndex == pauseIndices[1])
            {
                StartCoroutine(PauseAtWaypoint());
            }
            else
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length) reachedEnd = true;
            }
        }
    }

    IEnumerator PauseAtWaypoint()
    {
        isPaused = true;
        isBridgeEventRunning = true;

        bool first = (currentWaypointIndex == pauseIndices[0]);
        yield return StartCoroutine(HandleBridgeEvent(first));

        isPaused = false;
        isBridgeEventRunning = false;
        currentWaypointIndex++;
        if (currentWaypointIndex >= waypoints.Length) reachedEnd = true;
    }

    IEnumerator HandleBridgeEvent(bool isFirst)
    {
        // Freeze movement & physics
        playerControllerScript.enabled = false;
        playerRigidbody.isKinematic = true;

        // Zamijeni animator kontroler
        playerAnimator.runtimeAnimatorController = endingAnimatorController;

        // Pokreni animaciju sjekire direktno na brodu
        for (int i = 0; i < 3; i++)
        {
            playerAnimator.SetTrigger("IsChopping");
            yield return new WaitForSeconds(chopDuration);
        }

        // Uništi odgovarajući most
        if (isFirst) Destroy(Bridge1);
        else Destroy(Bridge2);
        yield return new WaitForSeconds(0.13f);

        // Vrati animator kontroler na normalni
        playerAnimator.runtimeAnimatorController = normalAnimatorController;

        // Unfreeze movement & physics
        playerRigidbody.isKinematic = false;
        playerControllerScript.enabled = true;
    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerTransform = other.transform;
            playerRigidbody = other.GetComponent<Rigidbody>();
            playerControllerScript = other.GetComponent<PlayerController>();
        
            // If player declined before, allow asking again now
            DialogSystem dialog = FindObjectOfType<DialogSystem>();
            if (dialog != null && dialog.hasFinishedDialogue && !movementStarted)
            {
                if (declinedPurchase)
                {
                    declinedPurchase = false;
                }

                ShowTicketPopup();
            }

        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            EnablePlayerMovement();
            playerTransform = null;
        }
    }

    void DisablePlayerMovement()
    {
        Debug.Log("DISABLING movement");

        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = false;
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.isKinematic = true;
        }

        CharacterController cc = playerTransform.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }

        PlayerInput playerInput = playerTransform.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }
    }




    void EnablePlayerMovement()
    {
        if (playerControllerScript != null) playerControllerScript.enabled = true;
        if (playerRigidbody != null) playerRigidbody.isKinematic = false;
    }
}
