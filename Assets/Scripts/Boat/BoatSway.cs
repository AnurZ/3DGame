using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BoatSway : MonoBehaviour
{
    public TMP_Text popupText2;
    
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
        startPos = transform.position;
        if (waypoints.Length == 0 || pauseIndices.Length < 2)
        {
            Debug.LogError("Assign at least two waypoints and two pauseIndices!");
            enabled = false;
        }

        // Hide popup at start and wire up buttons
        ticketPopup.SetActive(false);
        yesButton.onClick.AddListener(OnTicketYes);
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

        // Ulazak u brod i F za kupnju ulaznice
        if (playerInside && !movementStarted && !awaitingPurchase && Input.GetKeyDown(KeyCode.F))
        {
            ShowTicketPopup();
        }
        
        if (reachedEnd && !endingStarted)
        {
            endingStarted = true;
            StartCoroutine(PlayEndingSequence());
        }
    }

    IEnumerator PlayEndingSequence()
    {
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
        // 2) Prikaži linije teksta
        for (int i = 0; i < endingLines.Length; i++)
        {
            popupText2.text = endingLines[i];           // iskoristi postojeći popupText ili dodaj novi TextMeshProUGUI
            popupText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(lineDuration);
        }
        // sakrij text
        popupText2.gameObject.SetActive(false);

        // 3) Pokaži credits panel i scroll
        creditsPanel.SetActive(true);
        // postavi creditsText na početnu poziciju (ispod ekrana)
        creditsText.anchoredPosition = new Vector2(0, -creditsPanel.GetComponent<RectTransform>().rect.height);
    
        // scroll dok ne dođe do vrha
        float endPos = creditsText.rect.height;
        while (creditsText.anchoredPosition.y < endPos)
        {
            creditsText.anchoredPosition += Vector2.up * (creditsScrollSpeed * Time.deltaTime);
            yield return null;
        }
    }

    
    public void ShowTicketPopup()
    {
        awaitingPurchase = true;
        ticketPopup.SetActive(true);
        popupText.text = $"Do you want to buy the ticket for {ticketPrice} coins?";
    }

    public void OnTicketYes()
    {
        // Koristi CurrencyManager za potrošnju
        if (CurrencyManager.Instance.TrySpendMoney(ticketPrice))
        {
            CurrencyManager.Instance.SetMoneyText();
            ticketPopup.SetActive(false);
            awaitingPurchase = false;
            FindObjectOfType<DialogSystem>()?.gameObject.SetActive(false);

            StartCoroutine(BoardBoat());
        }
        else
        {
            popupText.text = "Not enough coins!";
        }
    }

    public void OnTicketNo()
    {
        ticketPopup.SetActive(false);
        awaitingPurchase = false;
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
        // Swap to ending controller
        playerAnimator.runtimeAnimatorController = endingAnimatorController;

        // 1) teleport sa broda na obalu
        playerTransform.SetParent(null, true);
        var exitPoint = isFirst ? enterPoint1 : enterPoint2;
        playerTransform.position = exitPoint.position;
        playerTransform.rotation = exitPoint.rotation;
        EnablePlayerMovement();
        yield return null;

        // 2) hod do mosta
        playerAnimator.SetBool("IsWalking", true);
        yield return null; // da Animator uhvati parametar
        var wp = isFirst ? walkToBridgePoint1 : walkToBridgePoint2;
        while (Vector3.Distance(playerTransform.position, wp.position) > 0.01f)
        {
            playerTransform.position = Vector3.MoveTowards(
                playerTransform.position, wp.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        playerAnimator.SetBool("IsWalking", false);

        // 3) rotacija i sječa
        var look = isFirst ? bridgeLookPoint1 : bridgeLookPoint2;
        Vector3 dir = (look.position - playerTransform.position).normalized;
        dir.y = 0f;
        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation,
            Quaternion.LookRotation(dir), Time.deltaTime * 10f);

        for (int i = 0; i < 3; i++)
        {
            playerAnimator.SetTrigger("IsChopping");
            yield return new WaitForSeconds(chopDuration);
            
        }

        // 4) uništi most
        if (isFirst) Destroy(Bridge1);
        else Destroy(Bridge2);
        yield return new WaitForSeconds(0.13f);

        // 5) povratak
        playerAnimator.SetBool("IsWalking", true);
        yield return null;
        var ret = isFirst ? returnToBoatPoint1 : returnToBoatPoint2;
        while (Vector3.Distance(playerTransform.position, ret.position) > 0.01f)
        {
            playerTransform.position = Vector3.MoveTowards(
                playerTransform.position, ret.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        playerAnimator.SetBool("IsWalking", false);

        // 6) ukrcaj i disable
        playerTransform.SetParent(boatBody, true);
        DisablePlayerMovement();

        // Restore original controller
        playerAnimator.runtimeAnimatorController = normalAnimatorController;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerTransform = other.transform;
            playerRigidbody = other.GetComponent<Rigidbody>();
            playerControllerScript = other.GetComponent<PlayerController>();
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
        if (playerControllerScript != null) playerControllerScript.enabled = false;
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.isKinematic = true;
        }
    }

    void EnablePlayerMovement()
    {
        if (playerControllerScript != null) playerControllerScript.enabled = true;
        if (playerRigidbody != null) playerRigidbody.isKinematic = false;
    }
}
