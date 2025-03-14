using UnityEngine;

public class TreeSway : MonoBehaviour
{
    public float swaySpeed = 1f; // Speed of the sway motion
    public float swayAmount = 0.1f; // Amount of sway (how far the tree sways)
    public float swayRandomness = 0.2f; // Randomness factor to add variation
    private Vector3 initialRotation;
    private float swayOffsetX; // Random offset for X sway
    private float swayOffsetZ; // Random offset for Z sway

    void Start()
    {
        initialRotation = transform.rotation.eulerAngles; // Store initial rotation

        // Randomize the direction of sway at start
        swayOffsetX = Random.Range(-swayRandomness, swayRandomness);
        swayOffsetZ = Random.Range(-swayRandomness, swayRandomness);
    }

    void Update()
    {
        // Calculate sway motion with side-to-side effect
        float swayX = Mathf.Sin(Time.time * swaySpeed + swayOffsetX) * swayAmount; // Swaying on X axis with randomness
        float swayZ = Mathf.Sin(Time.time * swaySpeed + swayOffsetZ) * swayAmount; // Swaying on Z axis with randomness

        // Apply sway to the tree's rotation, keeping Y axis fixed
        transform.rotation = Quaternion.Euler(initialRotation.x + swayX, initialRotation.y, initialRotation.z + swayZ);
    }
}