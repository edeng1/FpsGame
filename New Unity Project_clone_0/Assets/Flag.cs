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
            if (playerController.isDead && transform.parent != null && transform.parent != FlagSpawn)
            {
                PV.RPC("RPC_DropFlag", RpcTarget.All);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))//Player touched this flag
        {
            Debug.Log(awayFlag + " " + other.GetComponent<PlayerController>().awayTeam);

            if (awayFlag != other.GetComponent<PlayerController>().awayTeam)// If the flag isnt your teams.
            {


                Player player = other.GetComponent<PhotonView>().Owner;
                playerController = other.GetComponent<PlayerController>();
                go = player.TagObject as GameObject; //I might not use this tbh


                if(playerHasFlag==false)// changed 
                    PV.RPC("RPC_PickUpFlag", RpcTarget.All, other.GetComponent<PhotonView>().ViewID); //ViewID is of the player that touches the flag



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
    private void RPC_PickUpFlag(int vID) //When a flag is picked up, info is sent to all players
    {
        atHomeBase = false;
        playerHasFlag = true;
        transform.parent = PhotonView.Find(vID).transform; //Finds ViewID of the player that touches the flag and sets them as the flags parent. Flag sticks to player.
        transform.position = transform.parent.position; //Sets position of flag to player.
        transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));//Rotates it
        UIEventSystem.current.UIUpdateFlagPickUp(gameObject.tag+ " flag has been taken!"); //Displays what teams flag is taken to all players.
    }

    [PunRPC]
    private void RPC_DropFlag()
    {
        if (transform.position.y < -40f) //if flag falls off the map
        {
            atHomeBase = true;
            playerHasFlag = false;
            transform.parent = FlagSpawn;
            transform.position = FlagSpawn.position;
            UIEventSystem.current.UIUpdateFlagReturn(gameObject.tag + " flag fell off the map!");
        }
        else
        {
            atHomeBase = false;
            playerHasFlag = false;
            flagPos = transform.parent.position;
            transform.parent = null;
            transform.position = flagPos;
            UIEventSystem.current.UIUpdateFlagDrop(gameObject.tag + " flag has been dropped!");
        }
       }

    [PunRPC]
    private void RPC_ReturnFlag()
    {
        atHomeBase = true;
        transform.parent = FlagSpawn;
        transform.position = FlagSpawn.position;
        UIEventSystem.current.UIUpdateFlagReturn(gameObject.tag + " flag has been returned!");
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
        UIEventSystem.current.UIUpdateFlagReturn(gameObject.tag + " flag has been scored!");

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
