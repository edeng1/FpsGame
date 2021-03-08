using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAndDeactivate : MonoBehaviour
{
    public GameObject objToActivate;
    public GameObject objToDeactivate;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            objToActivate.SetActive(true);
            objToDeactivate.SetActive(false);
        }
    }
}
