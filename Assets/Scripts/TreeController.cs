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
    public GameObject bonusDropPrefab; // Bonus item with 5% drop chance

    
    public enum TreeTypes
    {
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4,
        Level5 = 5,
    }
    
    public TreeTypes treeType;
    
    public AchievementsController achievementsController;

    public TreeSpawner treeSpawner;
    
    private void Start()
    {
        treeRenderer = GetComponent<Renderer>();
        if (treeRenderer != null)
            originalMaterial = treeRenderer.material;

        treeSway = GetComponent<TreeSway>(); // Get reference to TreeSway component
        playerController = FindObjectOfType<PlayerController>();
        achievementsController = FindObjectOfType<AchievementsController>();
        treeSpawner = FindObjectOfType<TreeSpawner>();
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
                //if(achievementsController.TreesChopped < achievementsController.TreesChoppedLevels[achievementsController.TreesChoppedCurrentLevel])
                    achievementsController.TreesChopped++;
                switch (treeType)
                {
                    case TreeTypes.Level1: achievementsController.TreeType1 = 1; break;
                    case TreeTypes.Level2: achievementsController.TreeType2 = 1; break;
                    case TreeTypes.Level3: achievementsController.TreeType3 = 1; break;
                    case TreeTypes.Level4: achievementsController.TreeType4 = 1; break;
                    case TreeTypes.Level5: achievementsController.TreeType5 = 1; break;
                }
                achievementsController.ChopAllTreeTypes = achievementsController.TreeType1 +  achievementsController.TreeType2 + achievementsController.TreeType3
                    +  achievementsController.TreeType4 + achievementsController.TreeType5;
                treeSpawner.RemoveTreeByPosition(gameObject.transform.position);
                Destroy(gameObject);
            }
            
        }
    }

    private void DropPrefab()
    {
        if (dropPrefab != null && !hasDroppedPrefab)
        {
            Vector3 dropPosition = new Vector3(transform.position.x, 0.5f, transform.position.z);
            Instantiate(dropPrefab, dropPosition, Quaternion.identity);
            Debug.Log("Log prefab dropped!");

            // 5% chance to drop the bonus prefab
            if (bonusDropPrefab != null && Random.value <= 1f)
            {
                Vector3 bonusDropPosition = new Vector3(transform.position.x + 1f, 0.5f, transform.position.z); // Offset a bit
                Instantiate(bonusDropPrefab, bonusDropPosition, Quaternion.identity);
                Debug.Log("Bonus prefab dropped!");
            }

            hasDroppedPrefab = true;
        }
    }

}
