using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float bounceHeight = 0.25f;
    public float bounceSpeed = 2f;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        // Rotate
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Bounce up and down
        float newY = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        transform.position = new Vector3(initialPosition.x, initialPosition.y + newY, initialPosition.z);
    }
}