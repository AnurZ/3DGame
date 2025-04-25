using UnityEngine;
using System.Collections;

public class TreeController : MonoBehaviour
{
    private Material defaultMaterial;
    public Material highlightMaterial;
    private Renderer rend;

    private bool isShaking = false;
    private bool isChopping = false; // Ensure only one chop happens at a time
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    [Header("Shake Settings")]
    public float shakeAmount = 0.1f;
    public float shakeSpeed = 10f;

    [Header("Drop Settings")]
    public GameObject dropPrefab;
    public Transform dropSpawnPoint;

    [Header("Chop Settings")]
    public float baseChopTime = 4f; // Time it takes to chop the tree

    private float shakeTime = 0f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        defaultMaterial = rend.sharedMaterial;
        originalRotation = transform.rotation;
        originalPosition = transform.position;
    }

    void Update()
    {
        if (isShaking)
        {
            ShakeTree();
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * 5f);
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * 5f);
        }
    }

    public void StartHighlighting()
    {
        if (highlightMaterial != null)
        {
            rend.sharedMaterial = highlightMaterial;
        }
    }

    public void StopHighlighting()
    {
        if (defaultMaterial != null)
        {
            rend.sharedMaterial = defaultMaterial;
        }
    }

    public void StartChopping()
    {
        if (!isChopping) // Prevent starting multiple chop actions
        {
            isChopping = true;
            StartCoroutine(ChopCoroutine());
        }
    }

    private IEnumerator ChopCoroutine()
    {
        isShaking = true;

        float chopDuration = baseChopTime;
        float elapsed = 0f;
        float staminaPerSecond = (StaminaController.Instance != null) ? 10f / chopDuration : 0f;

        while (elapsed < chopDuration && StaminaController.Instance != null && StaminaController.Instance.playerStamina > 0)
        {
            StaminaController.Instance.ReduceStamina(staminaPerSecond * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        FinishChopping(); // Finish chopping once the duration is over or stamina runs out
    }

    public void StopChopping()
    {
        isShaking = false;
        StopAllCoroutines(); // Stop the chopping coroutine if interrupted
    }

    public void FinishChopping()
    {
        if (!isChopping) return; // If chopping was already finished, do nothing

        isShaking = false;
        isChopping = false; // Reset chopping state

        // Drop the prefab when chopping is finished
        if (dropPrefab != null)
        {
            Vector3 spawnPosition = dropSpawnPoint != null
                ? dropSpawnPoint.position
                : transform.position + Vector3.up * 1f;

            Instantiate(dropPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} has no dropPrefab assigned!");
        }

        Destroy(gameObject, 0.2f); // Destroy the tree after a short delay
    }

    public float GetChopDuration()
    {
        return baseChopTime;
    }

    void ShakeTree()
    {
        shakeTime += Time.deltaTime * shakeSpeed;
        float shakeOffset = Mathf.Sin(shakeTime) * shakeAmount;

        transform.rotation = originalRotation * Quaternion.Euler(0, 0, shakeOffset);
        transform.position = originalPosition + new Vector3(shakeOffset * 0.1f, 0, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartHighlighting();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopHighlighting();
        }
    }
}
