using UnityEngine;

public class BridgeDestruction : MonoBehaviour
{
    public GameObject bridge1;        // Most koji se uništava
    

    public float timeToDestroyBridge1;
    
    
    // Most koji se uništava
    //public ParticleSystem destructionEffect;  // Efekt uništavanja

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
//             Debug.Log("Collsion with " + other.name);
            DestroyBridge();
        }
    }

    void DestroyBridge()
    {
        // Aktiviraj efekt uništavanja
        //destructionEffect.Play();

        // Deaktiviraj most nakon kratkog vremena
        Destroy(bridge1, timeToDestroyBridge1);  // Uništi most nakon 1 sekunde
          // Uništi most nakon 1 sekunde
    }
}
