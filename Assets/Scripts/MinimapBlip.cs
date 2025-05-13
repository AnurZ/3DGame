using UnityEngine;

public class MinimapBlip : MonoBehaviour
{
    [Range(-180f, 180f)]
    public float rotationOffset = 0f;

    public Vector2 positionOffset = Vector2.zero;

    public Transform player;
    public RectTransform blip;
    public RectTransform minimapRect;

    public float mapSizeWorldUnits = 100f;

    void Update()
    {
        Vector2 playerPos = new Vector2(player.position.x, player.position.z);

        float normalizedX = playerPos.x / mapSizeWorldUnits;
        float normalizedY = playerPos.y / mapSizeWorldUnits;

        float blipX = (normalizedX * minimapRect.sizeDelta.x) - (minimapRect.sizeDelta.x / 2f);
        float blipY = (normalizedY * minimapRect.sizeDelta.y) - (minimapRect.sizeDelta.y / 2f);

        // ✅ Apply offset here
        blip.anchoredPosition = new Vector2(blipX, blipY) + positionOffset;

        blip.localEulerAngles = new Vector3(0, 0, -player.eulerAngles.y + rotationOffset);
    }
}