using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoorTriggerArea : MonoBehaviour
{
    bool nearDoor=false;
    bool isDoorOpen=false;
    public int id;
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.E)&&nearDoor)
        {
            if(!isDoorOpen)
            {
                GameEvents.current.DoorwayTriggerOpen(id);
                isDoorOpen = true;
            }
            else if (isDoorOpen)
            {
                GameEvents.current.DoorwayTriggerClose(id);
                isDoorOpen = false;
            }
               


        }

        

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            nearDoor = true;

        }
        if(other.tag=="Enemy"&& !isDoorOpen)
        {
            GameEvents.current.DoorwayTriggerOpen(id);
        }
        

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            nearDoor = false;

        }
    }
}