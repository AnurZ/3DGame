using UnityEngine;
using System.Collections.Generic;

public class TransparencyController : MonoBehaviour
{
    public Transform player;
    public Transform cameraTransform;
    public float detectionRange = 50f;

    public List<TreeMaterialSwapper> currentlyTransparentTrees = new List<TreeMaterialSwapper>();

    public void resetTrees()
    {
        currentlyTransparentTrees = null;
        currentlyTransparentTrees = new List<TreeMaterialSwapper>();
    }

    // New function to reset and reapply transparency
    public void ResetAndReapplyTransparency()
    {
        // Reset the currently transparent trees
        resetTrees();

        // Apply transparency again based on the current state
        List<TreeMaterialSwapper> treesInTheWay = DetectTreesBetweenCameraAndPlayer();

        // Revert trees that are no longer in the way
        foreach (TreeMaterialSwapper tree in currentlyTransparentTrees)
        {
            if (!treesInTheWay.Contains(tree))
                tree.SetOriginal();
        }

        // Apply transparency to new trees
        foreach (TreeMaterialSwapper tree in treesInTheWay)
        {
            if (!currentlyTransparentTrees.Contains(tree))
                tree.SetTransparent();
        }

        // Update the list
        currentlyTransparentTrees = treesInTheWay;
    }
    
    void Update()
    {
        List<TreeMaterialSwapper> treesInTheWay = DetectTreesBetweenCameraAndPlayer();

        // Revert trees that are no longer in the way
        foreach (TreeMaterialSwapper tree in currentlyTransparentTrees)
        {
            if (!treesInTheWay.Contains(tree))
                tree.SetOriginal();
        }

        // Apply transparency to new trees
        foreach (TreeMaterialSwapper tree in treesInTheWay)
        {
            if (!currentlyTransparentTrees.Contains(tree))
                tree.SetTransparent();
        }

        // Update the list
        currentlyTransparentTrees = treesInTheWay;
    }

    public List<TreeMaterialSwapper> DetectTreesBetweenCameraAndPlayer()
    {
        Vector3 direction = player.position - cameraTransform.position;
        float distance = Vector3.Distance(player.position, cameraTransform.position);

        RaycastHit[] hits = Physics.RaycastAll(cameraTransform.position, direction.normalized, distance);
        List<TreeMaterialSwapper> foundTrees = new List<TreeMaterialSwapper>();

        foreach (RaycastHit hit in hits)
        {
            TreeMaterialSwapper swapper = hit.collider.GetComponent<TreeMaterialSwapper>();
            if (swapper != null && !foundTrees.Contains(swapper))
            {
                foundTrees.Add(swapper);
            }
        }

        return foundTrees;
    }
}
