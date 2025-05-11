using System.Collections.Generic;
using UnityEngine;

public class TreeGenerationManager : MonoBehaviour
{
    [System.Serializable]
    public class TreePrefab
    {
        public GameObject prefab;
        public int level; // 1 to 5
    }

    public List<TreePrefab> treePrefabs;
    public GameObject groundObject; // Merged planes GameObject
    public int treeCap = 30;
    public int currentDay = 1;
    public float minSpawnDistance = 5f;

    private List<GameObject> spawnedTrees = new List<GameObject>();

    void Start()
    {
        // Optional: load saved trees here
    }

    public void SpawnTreesAtSleep()
    {
        // Clean nulls (in case trees were chopped)
        spawnedTrees.RemoveAll(tree => tree == null);

        int existingTreeCount = spawnedTrees.Count;
        int treesToSpawn = Mathf.Max(0, treeCap - existingTreeCount);

        if (treesToSpawn == 0) return;

        List<float> weights = GetTreeSpawnWeights(currentDay);
        for (int i = 0; i < treesToSpawn; i++)
        {
            int level = GetWeightedRandomTreeLevel(weights);
            TreePrefab selectedTree = treePrefabs.Find(t => t.level == level);

            Vector3? spawnPos = FindValidSpawnPosition();
            if (spawnPos != null)
            {
                GameObject tree = Instantiate(selectedTree.prefab, spawnPos.Value, Quaternion.identity);
                spawnedTrees.Add(tree);
            }
        }
    }

    List<float> GetTreeSpawnWeights(int day)
    {
        day = Mathf.Min(day, 40);
        float[] weights = new float[5];

        if (day < 3)
        {
            weights[0] = 1f;
        }
        else
        {
            weights[0] = Mathf.Max(1f - (day - 2) * 0.03f, 0.1f);
            weights[1] = Mathf.Min((day - 2) * 0.05f, 0.4f);
            weights[2] = (day >= 10) ? Mathf.Min((day - 10) * 0.04f, 0.3f) : 0f;
            weights[3] = (day >= 20) ? Mathf.Min((day - 20) * 0.03f, 0.15f) : 0f;
            weights[4] = (day >= 30) ? Mathf.Min((day - 30) * 0.02f, 0.05f) : 0f;
        }

        float total = 0f;
        foreach (float w in weights) total += w;
        for (int i = 0; i < weights.Length; i++) weights[i] /= total;

        return new List<float>(weights);
    }

    int GetWeightedRandomTreeLevel(List<float> weights)
    {
        float rand = Random.value;
        float cumulative = 0f;

        for (int i = 0; i < weights.Count; i++)
        {
            cumulative += weights[i];
            if (rand < cumulative)
                return i + 1; // Level is index + 1
        }

        return 1; // fallback
    }

    Vector3? FindValidSpawnPosition()
    {
        for (int attempts = 0; attempts < 50; attempts++)
        {
            // Generate random x and z positions within the bounds of the ground
            float x = Random.Range(0, groundObject.GetComponent<MeshRenderer>().bounds.size.x);
            float z = Random.Range(0, groundObject.GetComponent<MeshRenderer>().bounds.size.z);

            // Set the y position to -0.8 (flat ground height)
            Vector3 spawnPos = new Vector3(x, -0.8f, z);

            // Check if the spawn position is too close to existing trees
            bool tooClose = false;
            foreach (GameObject tree in spawnedTrees)
            {
                if (Vector3.Distance(tree.transform.position, spawnPos) < minSpawnDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
                return spawnPos;
        }

        return null; // No valid spot found
    }
}
