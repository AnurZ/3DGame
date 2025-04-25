using UnityEngine;

public class ZoomTransition : MonoBehaviour
{
   [SerializeField] public Transform playerCameraTransform;
    [SerializeField]public Transform shopCameraTransform;

    public float transitionSpeed = 2.0f;
    private bool isZoomingToShop = false;
    private bool isZoomingBack = false;

    private Vector3 originalPos;
    private Quaternion originalRot;

    void Start()
    {
        if (playerCameraTransform != null)
        {
            originalPos = playerCameraTransform.position;
            originalRot = playerCameraTransform.rotation;
        }
    }

    void Update()
    {
        if (isZoomingToShop)
        {
            playerCameraTransform.position = Vector3.Lerp(playerCameraTransform.position, shopCameraTransform.position, Time.deltaTime * transitionSpeed);
            playerCameraTransform.rotation = Quaternion.Lerp(playerCameraTransform.rotation, shopCameraTransform.rotation, Time.deltaTime * transitionSpeed);
        }
        else if (isZoomingBack)
        {
            playerCameraTransform.position = Vector3.Lerp(playerCameraTransform.position, originalPos, Time.deltaTime * transitionSpeed);
            playerCameraTransform.rotation = Quaternion.Lerp(playerCameraTransform.rotation, originalRot, Time.deltaTime * transitionSpeed);
        }
    }

    public void ZoomToShop()
    {
        
    
        if (playerCameraTransform == null || shopCameraTransform == null)
        {
            
            return;
        }

        playerCameraTransform.gameObject.SetActive(false);
        shopCameraTransform.gameObject.SetActive(true);

    }


    public void ZoomBackToPlayer()
    {
        Debug.Log(">> Deaktiviram shop kameru");
        shopCameraTransform.gameObject.SetActive(false);

        Debug.Log(">> Aktiviram player kameru");
        playerCameraTransform.gameObject.SetActive(true);

        Debug.Log("ZoomBackToPlayer() pozvan!");
    }
}