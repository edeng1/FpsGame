using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerTriggerArea : MonoBehaviour
{
    int id;
    bool nearComputer=false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)&&nearComputer==true)
        {
            GameEvents.current.ComputerEnabled(this);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            nearComputer = true;
            

        }


    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            nearComputer = false;


        }


    }
}
