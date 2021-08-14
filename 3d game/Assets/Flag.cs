using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public Transform FlagSpawn;
    public bool atHomeBase = true;
    public bool playerHasFlag=false;
    private void Start()
    {
        FlagSpawn = transform.parent;
        transform.position = FlagSpawn.position;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&& atHomeBase==true)
        {
            atHomeBase = false;
            playerHasFlag = true;
            transform.parent = other.transform;
            transform.position = transform.parent.position;
            
        }
        if (other.CompareTag("Enemy") && atHomeBase == false&&playerHasFlag==false)
        {
            atHomeBase = true;
            
            transform.parent = FlagSpawn;
            transform.position = FlagSpawn.position;
        }

    }
}
