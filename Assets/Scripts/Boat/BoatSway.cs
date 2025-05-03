using UnityEngine;
using System.Collections;

public class BoatSway : MonoBehaviour
{
    // --- Player refs ---
    private Transform playerTransform;
    private Rigidbody playerRigidbody;
    private PlayerController playerControllerScript;

    private bool playerInside = false;
    private bool movementStarted = false;

    [Header("Boat Teleport Point")]
    public Transform boatTeleportPoint;  // Gdje igrač stoji dok plovi

    [Header("Bridge Event Settings")]
    // Bridge 1
    public Transform walkToBridgePoint1;
    public Transform bridgeLookPoint1;
    public Transform returnToBoatPoint1;
    // Bridge 2
    public Transform walkToBridgePoint2;
    public Transform bridgeLookPoint2;
    public Transform returnToBoatPoint2;

    public Animator playerAnimator;
    public float chopDuration = 3f;

    public GameObject Bridge1;
    public GameObject Bridge2;

    [Header("Boat Movement Settings")]
    public Transform[] waypoints;
    public int[] pauseIndices;     // npr. [1, 3]
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
    }

    void Update()
    {
        ApplyBobbing();
        ApplyTilt();

        if (movementStarted && !isBridgeEventRunning && playerTransform != null)
        {
            // svaki frame teleport igrač na boatTeleportPoint
            playerTransform.position = boatTeleportPoint.position;
            playerTransform.rotation = boatTeleportPoint.rotation;
        }

        if (movementStarted && !reachedEnd && !isPaused)
        {
            RotateTowardsWaypoint();
            MoveTowardsWaypoint();
        }

        if (playerInside && !movementStarted && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(BoardBoat());
        }
    }

    IEnumerator BoardBoat()
    {
        movementStarted = true;

        // 1) teleport na boatTeleportPoint
        playerTransform.SetParent(null, true);
        playerTransform.position = boatTeleportPoint.position;
        playerTransform.rotation = boatTeleportPoint.rotation;

        // 2) disable input + fizika
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
        // 1) teleport s broda na point povratka
        EnablePlayerMovement();
        playerTransform.SetParent(null, true);
        var retEnter = isFirst ? returnToBoatPoint1 : returnToBoatPoint2;
        // prvo teleport na dismount da resetira klizanje
        playerTransform.position = retEnter.position;
        playerTransform.rotation = retEnter.rotation;

        yield return null;

        // 2) hod do mosta
        playerAnimator.SetBool("IsWalking", true);
        var wp = isFirst ? walkToBridgePoint1 : walkToBridgePoint2;
        while (Vector3.Distance(playerTransform.position, wp.position) > 0.1f)
        {
            playerTransform.position = Vector3.MoveTowards(
                playerTransform.position, wp.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        playerAnimator.SetBool("IsWalking", false);

        // 3) okret i sječa
        var look = isFirst ? bridgeLookPoint1 : bridgeLookPoint2;
        playerTransform.LookAt(look.position);
        playerAnimator.SetTrigger("IsChopping");
        yield return new WaitForSeconds(chopDuration);

        // 4) uništi bridge
        if (isFirst) Destroy(Bridge1);
        else Destroy(Bridge2);

        yield return new WaitForSeconds(1f);

        // 5) povratak
        playerAnimator.SetBool("IsWalking", true);
        retEnter = isFirst ? returnToBoatPoint1 : returnToBoatPoint2;
        while (Vector3.Distance(playerTransform.position, retEnter.position) > 0.1f)
        {
            playerTransform.position = Vector3.MoveTowards(
                playerTransform.position, retEnter.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        playerAnimator.SetBool("IsWalking", false);

        // 6) ukrcaj i disable
        playerTransform.SetParent(boatBody, true);
        DisablePlayerMovement();
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
