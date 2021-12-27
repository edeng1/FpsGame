﻿using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    PhotonView PV;
    Photon.Realtime.Player player;
    public bool awayFlag;
    public Transform FlagSpawn;
    public bool atHomeBase = true;
    public bool playerHasFlag = false;
   
    
    GameObject go;
    Vector3 flagPos = new Vector3(0, 0, 0);
    PlayerController playerController;
    private void Awake()
    {
        FlagSpawn = transform.parent;
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        Debug.Log(transform.parent+" "+FlagSpawn);

        if (gameObject.CompareTag("Away")) { awayFlag = true; }
        
        transform.position = FlagSpawn.position;
    }
    private void Update()
    {

        if (playerController != null)
        {
            if (playerController.isDead && transform.parent != null)
            {
                PV.RPC("RPC_DropFlag", RpcTarget.All);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Debug.Log(awayFlag + " " + other.GetComponent<PlayerController>().awayTeam);

            if (awayFlag != other.GetComponent<PlayerController>().awayTeam)// If the flag isnt your teams.
            {


                Player player = other.GetComponent<PhotonView>().Owner;
                playerController = other.GetComponent<PlayerController>();
                go = player.TagObject as GameObject;


                if(playerHasFlag==false)// changed 
                    PV.RPC("RPC_PickUpFlag", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);



            }
            else //If the flag is your teams, and not at the homebase and the enemy player isnt holding it.
            {
                if (atHomeBase == false && playerHasFlag == false)
                {



                    PV.RPC("RPC_ReturnFlag", RpcTarget.All); //returns flag to homebase


                }
            }

        }


    }

    [PunRPC]
    private void RPC_PickUpFlag(int vID)
    {
        atHomeBase = false;
        playerHasFlag = true;
        transform.parent = PhotonView.Find(vID).transform;
        transform.position = transform.parent.position;
        transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
        UIEventSystem.current.UIUpdateFlagPickUp(gameObject.tag+ " flag has been taken");
    }

    [PunRPC]
    private void RPC_DropFlag()
    {
        atHomeBase = false;
        playerHasFlag = false;
        flagPos = transform.parent.position;
        transform.parent = null;
        transform.position = flagPos;
        UIEventSystem.current.UIUpdateFlagDrop(gameObject.tag + " flag has been dropped");
    }

    [PunRPC]
    private void RPC_ReturnFlag()
    {
        atHomeBase = true;
        transform.parent = FlagSpawn;
        transform.position = FlagSpawn.position;
        UIEventSystem.current.UIUpdateFlagReturn(gameObject.tag + " flag has been returned");
    }

    public void ScoreFlag()
    {
        
            Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
            Manager.Instance.ChangeStat_S(PhotonNetwork.LocalPlayer.ActorNumber, 2, 1);
            
            PV.RPC("RPC_ScoreFlag", RpcTarget.All);
            


    }

    [PunRPC]
    private void RPC_ScoreFlag()
    {
        atHomeBase = true;
        playerHasFlag = false;
        transform.parent = FlagSpawn;
        transform.position = FlagSpawn.position;
        UIEventSystem.current.UIUpdateFlagReturn(gameObject.tag + " flag has been scored");

    }





    public void TrySync()//Called in FlagManager, which is called in Manager by the Master Client whenever a NewPlayer joins the game. Syncs flag positions and parents for the new player.
    {
        if (FlagSpawn == null) { FlagSpawn = transform.parent; }
        if (transform.parent == null)
        {
            PV.RPC("SyncFlagOnGround", RpcTarget.All, flagPos);
        }


        if (transform.parent != FlagSpawn && transform.parent != null)
        {
            PV.RPC("SyncFlagOnPlayer", RpcTarget.All, transform.parent.GetComponent<PhotonView>().ViewID);
        }

        
    }

    [PunRPC]
    private void SyncFlagOnPlayer(int vID)
    {
        transform.parent = PhotonView.Find(vID).transform;
        transform.position = transform.parent.position;
    }
    [PunRPC]
    private void SyncFlagOnGround(Vector3 _flagPos)
    {
        transform.parent = null;
        transform.position = _flagPos;
    }
   

}
