using UnityEngine;

public class TreeMaterialSwapper : MonoBehaviour
{
    public Material originalMaterial;
    public Material transparentMaterial;

    private Renderer rend;
    private bool isTransparent = false;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null && originalMaterial != null)
        {
            rend.material = originalMaterial;
        }
    }

    public void SetTransparent()
    {
        if (!isTransparent && transparentMaterial != null && rend != null)
        {
            rend.material = transparentMaterial;
            isTransparent = true;
        }
    }

    public void SetOriginal()
    {
        if (isTransparent && originalMaterial != null && rend != null)
        {
            rend.material = originalMaterial;
            isTransparent = false;
        }
    }
}