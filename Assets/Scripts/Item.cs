using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;


[CreateAssetMenu(menuName = "Scriptable object/Item")]
public class Item : ScriptableObject
{
    
    public string itemName;
    
    [Header("Only gameplay")] 
    public ItemType type;
    public ActionType actionType;

    [Header("Only UI")] 
    public bool stackable = true;

    [Header("Both")] 
    public Sprite image;

    [Header("3D Representation")]
    public GameObject itemPrefab; // <-- ADD THIS
    
    [Header("In-hand Settings")]
    public Vector3 inHandPosition = Vector3.zero;
    public Vector3 inHandRotation = Vector3.zero; // Use Euler angles
    public Vector3 inHandScale = Vector3.one;

    [Header("Item Type")]
    public bool isAxe = false;
    public bool isPotion = false;

    public string ItemDisplayName = "ITEM_NAME";
    
    public  AxeTypeState currentAxeType;
    
}
public enum AxeTypeState
{
    Level1 = 1,
    Level2 = 3,
    Level3 = 4,
    Level4 = 5
}

public enum ItemType
{
    Sapling,
    Axe,
    Wood,
    Potion
}

public enum ActionType
{
    Chop
}
