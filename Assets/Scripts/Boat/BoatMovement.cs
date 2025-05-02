using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    public Transform[] waypoints;    // Putovi kroz koje brod treba prolaziti
    public float speed = 5.0f;       // Brzina kretanja broda
    private int currentWaypoint = 0;

    void Update()
    {
        // Provjera ako brod treba da ide prema sljedećem waypointu
        if (currentWaypoint < waypoints.Length)
        {
            Transform target = waypoints[currentWaypoint];
            Vector3 direction = target.position - transform.position;

            // Kretanje broda prema trenutnom waypointu
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            // Kada dođe do waypointa, ide na sljedeći
            if (Vector3.Distance(transform.position, target.position) < 0.5f)
            {
                currentWaypoint++;
            }
        }
    }
}