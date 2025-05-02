using UnityEngine;

public class BoatSway : MonoBehaviour
{
    public float bobbingAmount = 0.2f;      // Visina bobanja (Y)
    public float bobbingSpeed = 1.5f;       // Brzina bobanja

    public float tiltAmount = 1.0f;         // Nagib lijevo-desno (Z rotacija)
    public float tiltSpeed = 1.0f;          // Brzina nagiba

    private Vector3 startPos;
    private Quaternion startRot;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    void Update()
    {
        // Bobbing gore-dole po Y osi
        float newY = startPos.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Blagi tilt rotacije (nagib oko Z ose)
        float tiltZ = Mathf.Sin(Time.time * tiltSpeed) * tiltAmount;
        transform.rotation = startRot * Quaternion.Euler(0f, 0f, tiltZ);
    }
}