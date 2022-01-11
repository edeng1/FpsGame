using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class PlayerManager : MonoBehaviour
{

    PhotonView PV;
   
    [SerializeField] private GameObject ragdollModel;
    [SerializeField] private GameObject normalModel;

   public bool die = false;
   public bool awayTeam;

    GameObject controller;
    private void Awake()
    {
      
        PV = GetComponent<PhotonView>();
        


    }
    void Start()
    {


        if (PV.IsMine)
        {
            if (GameSettings.GameMode != GameMode.FFA)
            {
                PV.RPC("SyncTeam", RpcTarget.All, GameSettings.IsAwayTeam);
            }
            CreateController();
        }
        
    }

    
    
    void CreateController()
    {
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
        if (GameSettings.GameMode == GameMode.FFA)
        {
            spawnPoint = SpawnManager.Instance.GetSpawnPoint();
        }
        
        controller=PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPoint.position, spawnPoint.rotation, 0,new object[] { PV.ViewID,GameSettings.IsAwayTeam});
        controller.GetComponent<PhotonView>().Owner.TagObject = controller;
        controller.GetComponent<PlayerController>().isDead = false;
        
       
    }

    public IEnumerator Die()
    {

        Manager.Instance.ChangeStat_S(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);
        yield return new WaitForSeconds(3f);
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
    Tuple<string,string> names;
    public IEnumerator Die(int actorNumber,string gunName)
    {
        Manager.Instance.ChangeStat_S(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1); //ActorNumber is player who is killed
        Manager.Instance.ChangeStat_S(actorNumber, 0, 1); //actorNumber is of player who does the killing
        names = Manager.Instance.GetPlayerNames(PhotonNetwork.LocalPlayer.ActorNumber, actorNumber);
        //" + controller.GetComponent<PlayerController>().GetGunName() +"
        PV.RPC("RPC_UpdatePlayerKilledUI", RpcTarget.All, names.Item1,names.Item2, gunName);
       
        yield return new WaitForSeconds(3f);
        PhotonNetwork.Destroy(controller);
        CreateController();
    }

    [PunRPC]
    private void RPC_UpdatePlayerKilledUI(string name1,string name2,string gun)
    {
        
        UIEventSystem.current.UIUpdatePlayerKilled(name2 +" ["+ gun +"] "+ name1);
        
        Debug.Log(name2 + " killed " + name1);
    }

    public void TrySync()
    {
        if (!PV.IsMine) return;

        if (GameSettings.GameMode != GameMode.FFA)
        {
            PV.RPC("SyncTeam", RpcTarget.All, GameSettings.IsAwayTeam);
        }

    }

    [PunRPC]
    private void SyncTeam(bool _awayTeam)
    {
        awayTeam = _awayTeam;

        if (!awayTeam)
        {

        }
    }

    public PlayerController getController()
    {
        return controller.GetComponent<PlayerController>();
    }

    public int getViewID(int actorNumber)
    {
        return PV.ViewID;
    }

    public void destroyGameObject()
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        Destroy(controller);
    }



}
