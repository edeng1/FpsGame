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
        
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        if (gameObject.CompareTag("Away")) { awayFlag = true; }
        FlagSpawn = transform.parent;
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


                //Manager.Instance.FlagPickUp_S(other.transform as Object);
                PV.RPC("RPC_PickUpFlag", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);



            }
            else
            {
                if (atHomeBase == false && playerHasFlag == false)
                {



                    PV.RPC("RPC_ReturnFlag", RpcTarget.All);


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

    }

    [PunRPC]
    private void RPC_DropFlag()
    {
        atHomeBase = false;
        playerHasFlag = false;
        flagPos = transform.parent.position;
        transform.parent = null;
        transform.position = flagPos;
    }

    [PunRPC]
    private void RPC_ReturnFlag()
    {
        atHomeBase = true;
        transform.parent = FlagSpawn;
        transform.position = FlagSpawn.position;
    }

    public void ScoreFlag()
    {
        PV.RPC("RPC_ScoreFlag", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_ScoreFlag()
    {
        atHomeBase = true;
        transform.parent = FlagSpawn;
        transform.position = FlagSpawn.position;
        Manager.Instance.ChangeStat_S(PhotonNetwork.LocalPlayer.ActorNumber, 2, 1);
    }





    public void TrySync()
    {
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
