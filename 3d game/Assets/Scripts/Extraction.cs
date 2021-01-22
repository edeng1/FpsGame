using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extraction : MonoBehaviour
{
    Manager man;
    public void Awake()
    {
        man=FindObjectOfType<Manager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            man.GameWon();
        }
    }
   

}
