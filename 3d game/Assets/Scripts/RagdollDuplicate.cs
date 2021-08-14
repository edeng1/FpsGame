using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollDuplicate : MonoBehaviour
{
    [SerializeField] private GameObject ragdollModel;
    [SerializeField] private GameObject normalModel;
    public Enemy enemy;
    void Awake()
    {
        ragdollModel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleDead()
    {
        CopyTransformData(sourceTransform: normalModel.transform, destinationTransform: ragdollModel.transform);
        ragdollModel.gameObject.SetActive(true);
        normalModel.gameObject.SetActive(false);
    }


    private void CopyTransformData(Transform sourceTransform,Transform destinationTransform)
    {
        if (sourceTransform.childCount != destinationTransform.childCount)
        {
            Debug.LogWarning("Invalid transform copy");
        }
        for(int i=0; i<sourceTransform.childCount;i++)
        {
            var source = sourceTransform.GetChild(i);
            var destination = destinationTransform.GetChild(i);
            destination.position = source.position;
            destination.rotation = source.rotation;
            var rb = destination.GetComponent<Rigidbody>();
            
     
            CopyTransformData(source, destination);
        }
        
       // CopyTransformData(source, destination);
    }
}
