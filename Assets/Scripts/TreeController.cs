using UnityEngine;

public class TreeController : MonoBehaviour
{
    private Material defaultMaterial;
    public Material highlightMaterial;
    private Renderer rend;

    private bool isShaking = false;
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
        isShaking = true;
    }

    public void StopChopping()
    {
        isShaking = false;
    }

    public void FinishChopping()
    {
        isShaking = false;

        if (dropPrefab != null)
        {
            Vector3 spawnPosition = dropSpawnPoint != null ? dropSpawnPoint.position : transform.position + Vector3.up * 1f;
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
