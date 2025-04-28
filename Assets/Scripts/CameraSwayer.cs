
using UnityEngine;
public class CameraSwayer : MonoBehaviour
{
    public float swayAmount = 0.5f;
    public float swaySpeed = 0.5f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        transform.position = startPos + new Vector3(
            Mathf.Sin(Time.time * swaySpeed) * swayAmount,
            Mathf.Cos(Time.time * swaySpeed * 0.5f) * swayAmount * 0.5f,
            0f
        );
    }
}
