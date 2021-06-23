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
        if (PV.IsMine)
        {
            CreateController();
        }
        
    }
    
    void CreateController()
    {
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
        controller=PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPoint.position, spawnPoint.rotation, 0,new object[] { PV.ViewID });
        
       
    }

    

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }

    
    
  

    
}
