using UnityEngine;

public class WaterWaveEffect : MonoBehaviour
{
    public float waveStrength = 0.1f;  // Height of the waves
    public float waveSpeed = 1f;       // Speed of wave movement
    public float waveFrequency = 0.5f; // How fast the waves repeat
    public float tilingX = 1f;         // Tiling for X axis noise
    public float tilingZ = 1f;         // Tiling for Z axis noise

    private Vector3[] originalVertices;
    private Mesh mesh;

    void Start()
    {
        // Get the Mesh Filter and mesh from the object
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices;
    }

    void Update()
    {
        // Create a new array to hold the modified vertex positions
        Vector3[] vertices = new Vector3[originalVertices.Length];

        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];
            
            // Calculate wave effect based on x and z coordinates
            float x = vertex.x * tilingX;
            float z = vertex.z * tilingZ;

            // Apply sine wave to the Y-axis based on x and z
            float wave = Mathf.Sin(Time.time * waveSpeed + x + z) * waveStrength;

            // Modify only the Y component to create the wave effect
            vertex.y += wave;

            // Assign the modified vertex back to the array
            vertices[i] = vertex;
        }

        // Update the mesh with the new vertices
        mesh.vertices = vertices;
        mesh.RecalculateNormals();  // Recalculate the normals to avoid lighting issues
        mesh.RecalculateBounds();   // Recalculate bounds to avoid culling issues
    }
}
