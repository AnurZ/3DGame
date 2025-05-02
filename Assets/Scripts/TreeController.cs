using UnityEngine;
using UnityEngine.UI;

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
    
    public PlayerController playerController;
    
    private void Start()
    {
        treeRenderer = GetComponent<Renderer>();
        if (treeRenderer != null)
            originalMaterial = treeRenderer.material;

        treeSway = GetComponent<TreeSway>(); // Get reference to TreeSway component
        playerController = FindObjectOfType<PlayerController>();
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
        
        playerController.ChoppingGameObject.SetActive(true);
    }

    public void StopChopping()
    {
        isChopping = false;
        choppingTime = 0f;  // Reset chopping time if the player stops chopping

        // Stop swaying when chopping stops
        treeSway?.StopSwaying();
        
        playerController.ChoppingGameObject.SetActive(false);
        if (playerController.choppingImage != null)
        {
            playerController.choppingImage.fillAmount = 0f;
        }

    }

    private void Update()
    {
        if (isChopping)
        {
            choppingTime += Time.deltaTime;

            // Update chopping UI fill
            if (playerController.choppingImage != null)
            {
                playerController.choppingImage.fillAmount = choppingTime / choppingDuration;
            }

            if (StaminaController.Instance != null)
                StaminaController.Instance.ReduceStamina(StaminaController.Instance.staminaReductionRate * Time.deltaTime);

            if (choppingTime >= choppingDuration)
            {
                playerController.ChoppingGameObject.SetActive(false);
                playerController.choppingImage.fillAmount = 0f;
                DropPrefab();
                PlayerController.Local.OnTreeChoppedDown();
                Destroy(gameObject);
            }

            health -= injuryRisk * Time.deltaTime;

            if (health <= 0f)
            {
                DropPrefab();
                PlayerController.Local.OnTreeChoppedDown();
                Destroy(gameObject);
            }
        }
    }


    private void Chop()
    {
        if (StaminaController.Instance != null)
            StaminaController.Instance.ReduceStamina(StaminaController.Instance.staminaReductionRate);

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
