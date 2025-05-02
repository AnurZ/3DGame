using UnityEngine;

public class BoatInteraction : MonoBehaviour
{
    public GameObject player;
    public Transform boatPosition;   // Pozicija na brodu na kojoj igrač treba stati
    public KeyCode interactKey = KeyCode.F;   // Tipka za interakciju

    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 20.0f && Input.GetKeyDown(interactKey))
        {
            // Ako igrač je blizu broda i pritisne F, ulazi u brod
            player.transform.position = boatPosition.position;
            player.transform.SetParent(transform);  // Igrač postaje child brodu, prati ga
        }
    }
}