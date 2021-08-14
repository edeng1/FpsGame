using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaputrePointFlag : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Flag"))
        {
            Debug.Log("Flag captured");
            other.transform.parent = other.GetComponent<Flag>().FlagSpawn;
            other.transform.position = other.GetComponent<Flag>().FlagSpawn.position;
            other.GetComponent<Flag>().atHomeBase = true;
            other.GetComponent<Flag>().playerHasFlag = false;
        }
    }
}
