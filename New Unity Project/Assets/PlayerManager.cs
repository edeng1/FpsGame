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
    int usedSpawnIndex=-1;
    Dictionary<int, bool> usedSpawns;

   public bool die = false;
   public bool awayTeam;
    public float spawnRadius = 5f;

    GameObject controller;
    private void Awake()
    {
      
        PV = GetComponent<PhotonView>();
        usedSpawns = new Dictionary<int, bool>();


    }
    void Start()
    {
        
            
        
       

        if (PV.IsMine)
        {
            for (int i = 0; i < SpawnManager.Instance.transform.childCount; i++)//initializes used spawn dictionary to false
            {
                usedSpawns.Add(i, false);

            }
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
        int spawnIndex= spawnPoint.GetSiblingIndex();


        float angle = UnityEngine.Random.Range(0f, 360f);
        float x = spawnRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = spawnRadius * Mathf.Sin(angle * Mathf.Deg2Rad);

        if (usedSpawns[spawnIndex] == true)//check if spawn is in use, gets new random spawn
        {
            CreateController();
        }
        controller =PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(x, 0, y) + spawnPoint.position, spawnPoint.rotation, 0,new object[] { PV.ViewID,GameSettings.IsAwayTeam});
        PV.RPC("RPC_SpawnInUse", RpcTarget.All, spawnIndex); 
        controller.GetComponent<PhotonView>().Owner.TagObject = controller; //Stores this controller into TagObject, is used in Flag script
        controller.GetComponent<PlayerController>().isDead = false;
        
       
    }

    [PunRPC]
    void RPC_SpawnInUse(int index) //spawn is in use
    {
        usedSpawnIndex = index;
        usedSpawns[index] = true;
        StartCoroutine(SpawnInUse(index));
    }
    IEnumerator SpawnInUse(int index) //waits 3 seconds for spawn not in use
    {
        yield return new WaitForSeconds(3f);
        usedSpawns[index] = false;
        usedSpawnIndex = -1;
    }

    public IEnumerator Die()
    {

        Manager.Instance.ChangeStat_S(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);
        yield return new WaitForSeconds(3f);
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
    Tuple<string,string> names;
    public IEnumerator Die(int actorNumber,string gunName, bool headshot)
    {
        Manager.Instance.ChangeStat_S(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1); //ActorNumber is player who is killed updates their deaths
        Manager.Instance.ChangeStat_S(actorNumber, 0, 1); //actorNumber is of player who does the killing
        names = Manager.Instance.GetPlayerNames(PhotonNetwork.LocalPlayer.ActorNumber, actorNumber);
        //" + controller.GetComponent<PlayerController>().GetGunName() +"
        PV.RPC("RPC_UpdatePlayerKilledUI", RpcTarget.All, names.Item1,names.Item2, gunName, headshot);
       
        yield return new WaitForSeconds(3f);
        PhotonNetwork.Destroy(controller);
        CreateController();
    }

    [PunRPC]
    private void RPC_UpdatePlayerKilledUI(string name1,string name2,string gun, bool headshot)
    {
       
            UIEventSystem.current.UIUpdatePlayerKilled(name2 + " [" + gun + "] " + name1,headshot);
      
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
