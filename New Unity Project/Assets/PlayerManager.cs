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

    GameObject controller;
    private void Awake()
    {
      
        PV = GetComponent<PhotonView>();
        


    }
    void Start()
    {

        Manager.Instance.playerList.Add(this);

        if (PV.IsMine)
        {
           
            CreateController();
        }
        
    }

    
    
    void CreateController()
    {
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
        controller=PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPoint.position, spawnPoint.rotation, 0,new object[] { PV.ViewID });
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

 






}
