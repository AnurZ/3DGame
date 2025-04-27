using UnityEngine;

public class TreeController : MonoBehaviour
{
    public float health = 100f;
    public Material highlightMaterial;
    private Material originalMaterial;
    private Renderer treeRenderer;
    private bool isChopping = false;
    public float timeBetweenChops = 1f;

    private float injuryRisk = 0f;

    public GameObject dropPrefab;  // The prefab to drop when the tree is chopped down

    public float choppingDuration = 6f;  // Default duration for chopping
    private float choppingTime = 0f;  // Track continuous chopping time

    private bool hasDroppedPrefab = false;  // To check if prefab has been already dropped
    private TreeSway treeSway;

    private void Start()
    {
        treeRenderer = GetComponent<Renderer>();
        if (treeRenderer != null)
            originalMaterial = treeRenderer.material;

        treeSway = GetComponent<TreeSway>(); // Get reference to TreeSway component
    }

    public void StartHighlighting()
    {
        if (treeRenderer != null && highlightMaterial != null)
            treeRenderer.material = highlightMaterial;
    }

    public void StopHighlighting()
    {
        if (treeRenderer != null && originalMaterial != null)
            treeRenderer.material = originalMaterial;
    }

    public void StartChopping(float risk, float adjustedChopDuration)
    {
        isChopping = true;
        injuryRisk = risk;

        // Adjust the chopping duration based on injury
        choppingDuration = adjustedChopDuration;

        // Start swaying when chopping starts
        treeSway?.StartSwaying();
    }

    public void StopChopping()
    {
        isChopping = false;
        choppingTime = 0f;  // Reset chopping time if the player stops chopping

        // Stop swaying when chopping stops
        treeSway?.StopSwaying();
    }

    private void Update()
    {
        if (isChopping)
        {
            choppingTime += Time.deltaTime;  // Track the continuous chopping time

            // Drain stamina while chopping
            if (StaminaController.Instance != null)
                StaminaController.Instance.ReduceStamina(1f * Time.deltaTime);  // Reduces stamina per second

            // Check if the tree has been chopped for the adjusted duration
            if (choppingTime >= choppingDuration)
            {
                Debug.Log("Tree chopped down after " + choppingDuration + " seconds of continuous chopping!");
                DropPrefab();
                PlayerController.Local.OnTreeChoppedDown(); // <-- Injury roll here
                Destroy(gameObject);
            }

            // Apply health damage over time
            health -= injuryRisk * Time.deltaTime;

            if (health <= 0f)
            {
                Debug.Log("Tree chopped down!");
                DropPrefab();
                PlayerController.Local.OnTreeChoppedDown(); // <-- Injury roll here
                Destroy(gameObject);
            }
        }
    }

    private void Chop()
    {
        if (StaminaController.Instance != null)
            StaminaController.Instance.ReduceStamina(5);

        float injuryRoll = Random.Range(0f, 100f);

        health -= 10f;
        if (health <= 0f)
        {
            Debug.Log("Tree chopped down!");
            DropPrefab();
            PlayerController.Local.OnTreeChoppedDown(); // <-- Injury roll here too (just in case this path is ever used)
            Destroy(gameObject);
        }
    }

    private void DropPrefab()
    {
        if (dropPrefab != null && !hasDroppedPrefab)
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
            Debug.Log("Prefab dropped!");
            hasDroppedPrefab = true;  // Set flag to true to ensure item is dropped only once
        }
    }
}
