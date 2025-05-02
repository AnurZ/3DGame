using UnityEngine;

public class BridgeDestruction : MonoBehaviour
{
    public GameObject bridge;        // Most koji se uništava
    //public ParticleSystem destructionEffect;  // Efekt uništavanja

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DestroyBridge();
        }
    }

    void DestroyBridge()
    {
        // Aktiviraj efekt uništavanja
        //destructionEffect.Play();

        // Deaktiviraj most nakon kratkog vremena
        Destroy(bridge, 1.0f);  // Uništi most nakon 1 sekunde
    }
}