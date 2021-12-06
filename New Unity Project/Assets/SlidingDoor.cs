using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SlidingDoor : MonoBehaviour
{
    bool nearDoor = false;
    bool isDoorOpen = false;
    public float openDoorPos = 2.5f;
    public float closeDoorPos = 0f;
    public GameObject door;
    PhotonView PV;
   

    private void Start()
    {
        door = transform.GetChild(1).gameObject;
        closeDoorPos = door.transform.localPosition.x;
        PV = GetComponent<PhotonView>();
        
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.E) && nearDoor)
        {
            if (!isDoorOpen)
            {
                PV.RPC("OnDoorWayOpen",RpcTarget.All);
                isDoorOpen = true;
            }
            else if (isDoorOpen)
            {
                PV.RPC("OnDoorWayClose", RpcTarget.All); 
                isDoorOpen = false;
            }



        }



    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nearDoor = true;

        }
        if (other.tag == "Enemy" && !isDoorOpen)
        {

        }


    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nearDoor = false;

        }
    }

    [PunRPC]
    private void OnDoorWayOpen()
    {
        
            LeanTween.moveLocalX(door, openDoorPos, 1f).setEaseInQuad();
        

    }
    [PunRPC]
    private void OnDoorWayClose()
    {
        
            LeanTween.moveLocalX(door, closeDoorPos, 1f).setEaseInQuad();
        
    }
}
