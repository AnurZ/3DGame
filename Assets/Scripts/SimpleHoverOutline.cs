using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Renderer))]
public class SimpleHoverOutline : MonoBehaviour
{
    public Material outlineMaterial;   // assign M_Sword_Base_Outline here
    private Material originalMaterial;
    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        originalMaterial = rend.material;
    }

    void OnMouseEnter()
    {
        rend.material = outlineMaterial;
    }

    void OnMouseExit()
    {
        rend.material = originalMaterial;
    }
}
