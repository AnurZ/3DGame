using UnityEngine;
using System.Collections;
public class TreeSway : MonoBehaviour
{
    public float swaySpeed = 1f; // Speed of the sway motion
    public float swayAmount = 0.1f; // Amount of sway (how far the tree sways)
    public float swayRandomness = 0.2f; // Randomness factor to add variation
    //public Transform treeTop; // Reference to the top part of the tree (Green part)
    //public Transform treeBase; // Reference to the bottom part of the tree (Base part)
    public float swayInterval = 1.5f; // Interval for the sway in seconds

    private Vector3 initialRotation;
    private float swayOffsetX; // Random offset for X sway
    private float swayOffsetZ; // Random offset for Z sway
    private float swayTimer = 0f; // Timer for interval-based sway
    private bool isSwaying = false; // Whether the tree is currently swaying

    void Start()
    {
        //initialRotation = treeTop.rotation.eulerAngles; // Store initial rotation of the top part of the tree

        // Randomize the direction of sway at start
        swayOffsetX = Random.Range(-swayRandomness, swayRandomness);
        swayOffsetZ = Random.Range(-swayRandomness, swayRandomness);
    }

    void Update()
    {
        // Timer logic to trigger sway at fixed intervals
        swayTimer += Time.deltaTime;

        if (swayTimer >= swayInterval)
        {
            // Trigger sway and reset timer
            isSwaying = true;
            swayTimer = 0f;
        }

        if (isSwaying)
        {
            // Calculate sway motion with side-to-side effect
            float swayX = Mathf.Sin(Time.time * swaySpeed + swayOffsetX) * swayAmount; // Swaying on X axis with randomness
            float swayZ = Mathf.Sin(Time.time * swaySpeed + swayOffsetZ) * swayAmount; // Swaying on Z axis with randomness

            // Calculate the distance from the top to the base of the tree
            //float distanceToBase = Vector3.Distance(treeTop.position, treeBase.position);
            // Apply a factor to reduce sway intensity based on distance (top sways more, bottom sways less)
            //float swayFactor = Mathf.Clamp01(1f - distanceToBase / 10f); // Adjust '10f' to control how quickly the sway reduces

            // Apply sway only to the top part of the tree
            //if (treeTop != null)
            //{
              //  treeTop.rotation = Quaternion.Euler(initialRotation.x + swayX * swayFactor, initialRotation.y, initialRotation.z + swayZ * swayFactor);
            //}

            // Reset the sway after a short period
            StartCoroutine(StopSwayAfterInterval());
        }
    }

    // Coroutine to stop the sway after the interval
    private IEnumerator StopSwayAfterInterval()
    {
        yield return new WaitForSeconds(1f); // Adjust duration of the sway movement

        isSwaying = false;
    }
}
