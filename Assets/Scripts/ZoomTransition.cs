using System.Collections;
using UnityEngine;

public class ZoomTransition : MonoBehaviour
{
    [SerializeField] public Camera playerCamera;
    [SerializeField] public Camera shopCamera;
    [SerializeField] private CursorManager cursorManager;
    public float transitionSpeed = 2.0f;

    private bool isTransitioning = false;
    private bool isInShopView = false;

    private void Start()
    {
        if (playerCamera != null)
        {
            playerCamera.enabled = true;
            playerCamera.gameObject.SetActive(true);
        }
        if (shopCamera != null)
        {
            shopCamera.enabled = false;
            shopCamera.gameObject.SetActive(false);
        }
    }

    public void ZoomToShop()
    {
        if (isTransitioning || isInShopView || playerCamera == null || shopCamera == null)
            return;

        StartCoroutine(SmoothCameraSwitch(playerCamera, shopCamera, true));
    }

    public void ZoomBackToPlayer()
    {
        if (isTransitioning || !isInShopView || playerCamera == null || shopCamera == null)
            return;

        StartCoroutine(SmoothCameraSwitch(shopCamera, playerCamera, false));
        cursorManager.SetCursorByIndex(0);
    }

    private IEnumerator SmoothCameraSwitch(Camera fromCam, Camera toCam, bool goingToShop)
    {
        isTransitioning = true;

        // Temp camera for transition
        GameObject tempCamObj = new GameObject("TempCam");
        Camera tempCam = tempCamObj.AddComponent<Camera>();
        tempCam.CopyFrom(fromCam);

        tempCamObj.AddComponent<AudioListener>();
        
        // Remove AudioListener to avoid duplication warning
        AudioListener listener = tempCam.GetComponent<AudioListener>();
        if (listener != null)
            Destroy(listener);

        fromCam.enabled = false;
        fromCam.gameObject.SetActive(false);

        tempCam.enabled = true;
        tempCam.gameObject.SetActive(true);

        Vector3 startPos = fromCam.transform.position;
        Quaternion startRot = fromCam.transform.rotation;

        Vector3 endPos = toCam.transform.position;
        Quaternion endRot = toCam.transform.rotation;

        float elapsed = 0f;

        while (elapsed < transitionSpeed)
        {
            float t = elapsed / transitionSpeed;
            tempCam.transform.position = Vector3.Lerp(startPos, endPos, t);
            tempCam.transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Finalize transition
        tempCam.transform.position = endPos;
        tempCam.transform.rotation = endRot;

        Destroy(tempCamObj);

        toCam.gameObject.SetActive(true);
        toCam.enabled = true;

        isInShopView = goingToShop;
        isTransitioning = false;
    }
}
