using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D[] cursorTextures; // Assign in Inspector
    public Vector2 hotspot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    private void Start()
    {
        if (cursorTextures != null && cursorTextures.Length > 0)
        {
            SetCursorByIndex(0); // Set first icon on start
        }
    }

    public void SetCursorByIndex(int index)
    {
        if (index >= 0 && index < cursorTextures.Length)
        {
            Cursor.SetCursor(cursorTextures[index], hotspot, cursorMode);
        }
        else
        {
//             Debug.LogWarning("Cursor index out of range: " + index);
        }
    }
}
