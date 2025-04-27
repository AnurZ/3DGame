using UnityEngine;

public class TreeSway : MonoBehaviour
{
    public float swaySpeed = 1f; // Speed of the sway motion
    public float swayAmount = 0.1f; // Amount of sway (how far the tree sways)
    public float swayRandomness = 0.2f; // Randomness factor to add variation

    private float swayOffsetX; // Random offset for X sway
    private float swayOffsetZ; // Random offset for Z sway
    private bool isSwaying = false; // Whether the tree is swaying or not

    void Start()
    {
        // Randomize the direction of sway at start
        swayOffsetX = Random.Range(-swayRandomness, swayRandomness);
        swayOffsetZ = Random.Range(-swayRandomness, swayRandomness);
    }

    void Update()
    {
        // If the tree is swaying (i.e., player is chopping), apply sway motion
        if (isSwaying)
        {
            float swayX = Mathf.Sin(Time.time * swaySpeed + swayOffsetX) * swayAmount;
            float swayZ = Mathf.Sin(Time.time * swaySpeed + swayOffsetZ) * swayAmount;

            transform.rotation = Quaternion.Euler(swayX, transform.rotation.eulerAngles.y, swayZ);
        }
    }

    // Start swaying when chopping begins
    public void StartSwaying()
    {
        isSwaying = true;
    }

    // Stop swaying when chopping stops
    public void StopSwaying()
    {
        isSwaying = false;
    }
}