using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;
    public Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    // Define boundaries for camera movement
    public float minX = 40f, maxX = 220f;
    public float minY = -40f, maxY = 40f;
    public float minZ = 20f; // Prevent camera from going below Z = 20

    public float nearClippingPlane = 0.3f; // Adjust the near clipping plane

    void Start()
    {
        // Adjust the camera's near clipping plane
        Camera.main.nearClipPlane = nearClippingPlane;
    }

    void Update()
    {
        if (target != null)
        {
            // Calculate the target position based on the player's position and the offset
            Vector3 targetPosition = target.position + offset;

            // Apply X-axis movement boundaries
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);

            // Apply Y boundaries
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

            // Prevent camera from going below the minimum Z value
            targetPosition.z = Mathf.Max(targetPosition.z, minZ);

            // Smoothly move the camera
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}