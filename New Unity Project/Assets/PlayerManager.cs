using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
            if (GameSettings.GameMode == GameMode.TDM)
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
        
        controller=PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPoint.position, spawnPoint.rotation, 0,new object[] { PV.ViewID});
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

    public IEnumerator Die(int actorNumber)
    {
        Manager.Instance.ChangeStat_S(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);
        Manager.Instance.ChangeStat_S(actorNumber, 0, 1);

        yield return new WaitForSeconds(3f);
        PhotonNetwork.Destroy(controller);
        CreateController();
    }

    public void TrySync()
    {
        if (!PV.IsMine) return;

        if (GameSettings.GameMode == GameMode.TDM)
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






}
