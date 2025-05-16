using UnityEngine;

public class TreeInstance : MonoBehaviour
{
    public enum TreeTypes
    {
        Level1 = 1,
        Level2,
        Level3,
        Level4,
        Level5
    }

    public Vector3 position;
    public Quaternion rotation;
    public TreeTypes treeType;
    public string id; // unique identifier
}