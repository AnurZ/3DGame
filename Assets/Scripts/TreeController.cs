using UnityEngine;
using System.Collections;

public class TreeController : MonoBehaviour
{
    private Material defaultMaterial;
    public Material highlightMaterial;
    private Renderer rend;

    private bool isShaking = false;
    private bool isChopping = false;
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    [Header("Shake Settings")]
    public float shakeAmount = 0.1f;
    public float shakeSpeed = 10f;

    [Header("Drop Settings")]
    public GameObject dropPrefab;
    public Transform dropSpawnPoint;

    [Header("Chop Settings")]
    public float baseChopTime = 4f;

    private Coroutine chopCoroutine;

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
        if (!isChopping)
        {
            isChopping = true;
            chopCoroutine = StartCoroutine(ChopCoroutine());
        }
    }

    public void StopChopping()
    {
        if (isChopping)
        {
            isShaking = false;
            isChopping = false;

            if (chopCoroutine != null)
            {
                StopCoroutine(chopCoroutine);
            }
        }
    }

    private IEnumerator ChopCoroutine()
    {
        isShaking = true;

        float elapsed = 0f;
        float staminaPerSecond = (StaminaController.Instance != null) ? 10f / baseChopTime : 0f;

        while (elapsed < baseChopTime)
        {
            if (StaminaController.Instance != null)
            {
                StaminaController.Instance.ReduceStamina(staminaPerSecond * Time.deltaTime);
                if (StaminaController.Instance.playerStamina <= 0f)
                {
                    StopChopping(); // Stop if stamina runs out
                    yield break;
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        FinishChopping();
    }

    public void FinishChopping()
    {
        isShaking = false;
        isChopping = false;

        if (dropPrefab != null)
        {
            Vector3 spawnPosition = dropSpawnPoint != null
                ? dropSpawnPoint.position
                : transform.position + Vector3.up * 1f;

            Instantiate(dropPrefab, spawnPosition, Quaternion.identity);
        }

        Destroy(gameObject, 0.2f);
    }

    public float GetChopDuration()
    {
        return baseChopTime;
    }

    void ShakeTree()
    {
        float shakeOffset = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;

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
