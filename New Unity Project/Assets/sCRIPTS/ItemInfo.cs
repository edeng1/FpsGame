using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : ScriptableObject
{
    public string itemName;
    public GameObject itemModel;
    public GameObject[] itemModels;
    public Vector3 itemPosition;
    public Vector3 itemRotation;
    public Vector3 itemScale;
    public AudioClip itemSound;
}
