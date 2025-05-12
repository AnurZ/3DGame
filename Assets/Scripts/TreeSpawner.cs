using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public Terrain terrain;
    public GameObject treePrefab;
    public int numberOfTrees = 10;

    // Call this method to spawn trees
    public void SpawnTrees()
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        Debug.Log("sTARTANA FUNKCIJA");
        
        for (int i = 0; i < numberOfTrees; i++)
        {
            // Get random X and Z within terrain bounds
            float x = Random.Range(0, terrainData.size.x);
            float z = Random.Range(0, terrainData.size.z);

            // Get the height at that position
            float worldX = terrainPos.x + x;
            float worldZ = terrainPos.z + z;
            float y = terrain.SampleHeight(new Vector3(worldX, 0, worldZ)) + terrainPos.y;

            Debug.Log("Tree spawned " + i);
        }
    }
}