using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.IO;


[Serializable]
public class TreeSaverList
{
    public List<TreeSpawner.TreeSaver> trees;
}

public class TreeSpawner : MonoBehaviour
{
    public Terrain terrain;
    public GameObject[] treePrefabs; // Index 0 = level 1, index 4 = level 5
    public int desiredTreeCount = 40;
    public float minDistanceBetweenTrees = 10f;
    public AchievementsController achievementsController;
    [Serializable] 
    public class TreeSaver
    {
        public Vector3 position;
        public Quaternion rotation;
        public GameObject treePrefab;
    }
    
    public List<TreeSaver> SpawnedTrees = new List<TreeSaver>();
    
    public void SaveSpawnedTrees()
    {
        TreeSaverList container = new TreeSaverList();
        container.trees = SpawnedTrees;

        string json = JsonUtility.ToJson(container, true); // pretty print

        string path = Path.Combine(Application.persistentDataPath, "spawnedTrees.json");
        File.WriteAllText(path, json);

        Debug.Log($"Saved {SpawnedTrees.Count} trees to {path}");
    }
    
    public void Start()
    {
        achievementsController = FindObjectOfType<AchievementsController>();
    }

    public void SpawnTrees()
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        GameObject[] existingTrees = GameObject.FindGameObjectsWithTag("Tree");
        int currentTreeCount = existingTrees.Length;
        int treesToSpawn = desiredTreeCount - currentTreeCount;

        if (treesToSpawn <= 0)
        {
            Debug.Log("No need to spawn trees.");
            return;
        }

        Debug.Log($"Spawning {treesToSpawn} trees...");

        int spawnedCount = 0;
        List<GameObject> newlySpawned = new List<GameObject>();

        for (int i = 0; i < treesToSpawn; i++)
        {
            bool treeSpawned = false;

            for (int attempt = 0; attempt < 10; attempt++)
            {
                float x = Random.Range(0f, terrainData.size.x);
                float z = Random.Range(0f, terrainData.size.z);
                float worldX = terrainPos.x + x;
                float worldZ = terrainPos.z + z;
                float y = terrain.SampleHeight(new Vector3(worldX, 0, worldZ)) + terrainPos.y;

                if (y < -1) continue;

                Vector3 candidate = new Vector3(worldX, 0.1f, worldZ); // Force y to 0.1f
                Vector2 candidate2D = new Vector2(candidate.x, candidate.z);

                bool tooClose = false;

                // Check distance from existing trees
                foreach (GameObject tree in existingTrees)
                {
                    Vector3 treePos = tree.transform.position;
                    Vector2 tree2D = new Vector2(treePos.x, treePos.z);
                    if (Vector2.Distance(candidate2D, tree2D) < minDistanceBetweenTrees)
                    {
                        tooClose = true;
                        break;
                    }
                }

                // Check distance from newly spawned trees in this session
                if (!tooClose)
                {
                    foreach (GameObject tree in newlySpawned)
                    {
                        Vector3 treePos = tree.transform.position;
                        Vector2 tree2D = new Vector2(treePos.x, treePos.z);
                        if (Vector2.Distance(candidate2D, tree2D) < minDistanceBetweenTrees)
                        {
                            tooClose = true;
                            break;
                        }
                    }
                }

                if (tooClose) continue;

                // Spawn the tree with random Y-axis rotation
                GameObject selectedTree = GetTreePrefabBasedOnChopped();
                Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                GameObject newTree = Instantiate(selectedTree, candidate, randomRotation);
                newTree.tag = "Tree";

                newlySpawned.Add(newTree);
                spawnedCount++;
                treeSpawned = true;
                TreeSaver Tree = new TreeSaver()
                {
                    position = candidate,
                    treePrefab = selectedTree,
                    rotation = randomRotation,
                };
                SpawnedTrees.Add(Tree);
                break;
            }

            if (!treeSpawned)
            {
                Debug.Log("CANT SPAWN THE TREE");
            }
        }

        Debug.Log($"Spawn complete. Successfully spawned {spawnedCount} trees.");
    }

    public void LoadTrees()
    {
        string path = Path.Combine(Application.persistentDataPath, "spawnedTrees.json");

        if (!File.Exists(path))
        {
            Debug.LogWarning("No saved trees file found at " + path);
            return;
        }

        string json = File.ReadAllText(path);
        TreeSaverList container = JsonUtility.FromJson<TreeSaverList>(json);

        if (container != null && container.trees != null)
        {
            SpawnedTrees = container.trees;
            Debug.Log($"Loaded {SpawnedTrees.Count} trees from {path}");

            // Instantiate all trees
            foreach (var tree in SpawnedTrees)
            {
                GameObject newTree = Instantiate(tree.treePrefab, tree.position, tree.rotation);
                newTree.tag = "Tree";
            }
        }
        else
        {
            Debug.LogWarning("Failed to load trees from JSON.");
        }
    }

    
    public void RemoveTreeByPosition(Vector3 position)
    {
        // Find the first tree with the exact matching position
        TreeSaver treeToRemove = SpawnedTrees.Find(tree => tree.position == position);

        if (treeToRemove != null)
        {
            SpawnedTrees.Remove(treeToRemove);
            Debug.Log($"Removed tree at position {position} from SpawnedTrees." + $"{SpawnedTrees.Count}");
        }
        else
        {
            Debug.LogWarning($"No tree found at position {position} in SpawnedTrees.");
        }
    }

    GameObject GetTreePrefabBasedOnChopped()
    {
        int chopped = achievementsController.TreesChopped;
        float chance = Random.Range(0f, 1f);
        int level = 1;

        if (chopped <= 50)
        {
            level = 1;
        }
        else if (chopped <= 150)
        {
            if (chance < 0.8f) level = 1;
            else level = 2;
        }
        else if (chopped <= 300)
        {
            if (chance < 0.5f) level = 2;
            else if (chance < 0.85f) level = 3;
            else level = 1;
        }
        else if (chopped <= 500)
        {
            if (chance < 0.3f) level = 2;
            else if (chance < 0.6f) level = 3;
            else if (chance < 0.85f) level = 4;
            else level = 5;
        }
        else
        {
            if (chance < 0.5f) level = 4;
            else level = 5;
        }

        return treePrefabs[level - 1]; // 0-based array
    }
}
