using UnityEngine;
using System.Collections;

public class BoatSway : MonoBehaviour
{
    private Transform playerTransform;
    private CharacterController playerController;
    private Rigidbody playerRigidbody;  // Dodano za pristup Rigidbody-u
    private bool playerInside = false;
    private bool movementStarted = false;

    [Header("Bobbing Settings")]
    public float bobbingAmount = 0.2f;
    public float bobbingSpeed = 1.5f;

    [Header("Tilt Settings")]
    public float tiltAmount = 1.0f;
    public float tiltSpeed = 1.0f;

    [Header("Navigation")]
    public Transform[] waypoints;
    public float rotationSpeed = 90f;
    public float moveSpeed = 2f;
    public float waypointThreshold = 0.5f;

    [Header("Pause Settings")]
    public int[] pauseIndices;
    public float pauseDuration = 15f;

    [Header("Direction References")]
    public Transform boatHead;
    public Transform boatTail;
    public Transform boatBody;  // Referenca na samo brod, bez jedra

    private Vector3 startPos;
    private int currentWaypointIndex = 0;
    private bool reachedEnd = false;
    private bool isPaused = false;

    void Start()
    {
        startPos = transform.position;

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("BoatSway: Waypoints not assigned!");
            this.enabled = false;
        }
    }

    void Update()
    {
        ApplyBobbing();
        ApplyTilt();

        if (playerTransform != null)
        {
            playerTransform.SetParent(this.transform);
        }

        if (!reachedEnd && !isPaused && movementStarted)
        {
            RotateYTowardsWaypoint();
            MoveTowardsWaypoint();
        }

        if (playerInside && !movementStarted && Input.GetKeyDown(KeyCode.F))
        {
            movementStarted = true;
            Debug.Log("Boat movement started!");

            // Onemogući kretanje igrača
            DisablePlayerMovement();
        }
    }

    void ApplyBobbing()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void ApplyTilt()
    {
        float tiltZ = Mathf.Sin(Time.time * tiltSpeed) * tiltAmount;
        transform.localRotation = Quaternion.Euler(0f, transform.localEulerAngles.y, tiltZ);
    }

    void RotateYTowardsWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Length) return;

        Vector3 targetDir = (waypoints[currentWaypointIndex].position - transform.position);
        targetDir.y = 0f;

        if (targetDir.sqrMagnitude < 0.001f) return;

        float targetY = Quaternion.LookRotation(targetDir).eulerAngles.y;
        float newY = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetY, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, newY, transform.localEulerAngles.z);
    }

    void MoveTowardsWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Length) return;

        Vector3 targetPos = waypoints[currentWaypointIndex].position;
        targetPos.y = transform.position.y;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < waypointThreshold)
        {
            foreach (int pauseIndex in pauseIndices)
            {
                if (currentWaypointIndex == pauseIndex)
                {
                    StartCoroutine(PauseAtWaypoint());
                    return;
                }
            }

            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                reachedEnd = true;
                Debug.Log("Boat reached final destination.");
            }
        }
    }

    IEnumerator PauseAtWaypoint()
    {
        isPaused = true;
        Debug.Log($"Pausing at waypoint {currentWaypointIndex} for {pauseDuration} seconds...");
        yield return new WaitForSeconds(pauseDuration);
        isPaused = false;

        currentWaypointIndex++;

        if (currentWaypointIndex >= waypoints.Length)
        {
            reachedEnd = true;
            Debug.Log("Boat reached final destination.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerTransform = other.transform;
            playerController = other.GetComponent<CharacterController>();
            playerRigidbody = other.GetComponent<Rigidbody>();  // Dodano za pristup Rigidbody-u

            if (playerController != null)
            {
                // Onemogući kretanje i rigidbody
                DisablePlayerMovement();
            }

            // Parentaj direktno na brod (boatBody)
            playerTransform.SetParent(boatBody);  

            Debug.Log("Player entered boat trigger.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            playerTransform = null;

            // Ponovno omogućavanje Rigidbody-a kada igrač izađe iz broda
            if (playerRigidbody != null)
            {
                playerRigidbody.isKinematic = false;  // Ponovno omogućite fizičko kretanje
            }

            if (playerController != null)
            {
                playerController.enabled = true;
            }

            Debug.Log("Player exited boat trigger.");
        }
    }

    // Onemogućavanje kretanja i rigidbody komponente
    void DisablePlayerMovement()
    {
        if (playerController != null)
        {
            playerController.enabled = false;  // Onemogućava PlayerController
            playerController.Move(Vector3.zero);  // Zaustavi bilo kakvo kretanje
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = true;  // Onemogućite fiziku (rigidbody)
        }
    }

    // Ponovno omogućavanje kretanja i rigidbody komponente
    void EnablePlayerMovement()
    {
        if (playerController != null)
        {
            playerController.enabled = true;  // Ponovno omogućava PlayerController
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false;  // Ponovno omogućite fiziku (rigidbody)
        }
    }
}
