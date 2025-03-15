using UnityEngine;

public class TreeController : MonoBehaviour
{
    private Material defaultMaterial;
    public Material highlightMaterial;
    private Renderer rend;

    private bool isShaking = false;
    private Quaternion originalRotation;
    private Vector3 originalPosition;
    public float shakeAmount = 0.1f; // Shake intensity
    public float shakeSpeed = 10f; // Speed of shaking

    private float shakeTime = 0f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        defaultMaterial = rend.material;
        originalRotation = transform.rotation;
        originalPosition = transform.position;
    }

    void Update()
    {
        if (isShaking)
        {
            ShakeTree(); // Shake when chopping
        }
        else
        {
            // Smoothly reset rotation and position
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * 5f);
            transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * 5f);
        }
    }

    public void StartHighlighting()
    {
        if (highlightMaterial != null)
        {
            rend.material = highlightMaterial; // Highlight tree when near
        }
    }

    public void StopHighlighting()
    {
        if (highlightMaterial != null)
        {
            rend.material = defaultMaterial; // Revert highlight when leaving
        }
    }

    public void StartChopping()
    {
        isShaking = true; // Start shaking when chopping
    }

    public void StopChopping()
    {
        isShaking = false; // Stop shaking when chopping stops
    }

    void ShakeTree()
    {
        shakeTime += Time.deltaTime * shakeSpeed;
        float shakeOffset = Mathf.Sin(shakeTime) * shakeAmount; // Shake value based on sine wave for smooth oscillation

        // Apply shake to tree's rotation and position
        transform.rotation = originalRotation * Quaternion.Euler(0, 0, shakeOffset);
        transform.position = originalPosition + new Vector3(shakeOffset * 0.1f, 0, 0); // Small movement to simulate shaking
    }
}
