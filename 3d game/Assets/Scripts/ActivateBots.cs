using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBots : MonoBehaviour
{
    public GameObject bots;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            bots.SetActive(true);
        }
    }
}
