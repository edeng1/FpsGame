using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : MonoBehaviourPunCallbacks
{

    public static RoomManager Instance;
    GameObject PM;
    PhotonView PV;
    public GameObject launcher;
    private void Awake()
    {
        
        if(Instance)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        
        
        Instance = this;
        PV = GetComponent<PhotonView>();
        
        Hashtable hash = new Hashtable();
        hash.Add("PlayerReady", true);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);


    }
    void Start()
    {
        launcher = Launcher.instance.gameObject;
    }

   public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }



    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex != 0) //were in game scene
        {
            
            
            if (PV.IsMine)
            {
                Hashtable hash = new Hashtable();
                hash.Add("PlayerReady", false);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
           
        }
        if (scene.buildIndex == 0&&Launcher.inRoom)
        {
            Launcher.instance.OnJoinedRoom();
            if (PV.IsMine)
            {
                Hashtable hash = new Hashtable();
                hash.Add("PlayerReady", true);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
            

        }
    }
    public void Spawn()
    {
        PM=PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        //Manager.Instance.NewPlayer_S();
    }

    public PlayerManager getPlayerManager()
    {
        return PM.GetComponent<PlayerManager>();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {

        if (PV.IsMine&&targetPlayer==PV.Owner)
        {

            Debug.Log("props been called");
            if (changedProps.ContainsKey("PlayerReady"))
            {
                foreach (var a in changedProps)
                {
                    Debug.Log(a.Key + " " + a.Value);
                }
            }
            else
            {
                Debug.Log("Doesnt contain key");
                foreach (var a in changedProps)
                {
                    Debug.Log(a.Key);
                }
            }

            if (changedProps.ContainsKey("PlayerReady"))
            {
                if ((bool)changedProps["PlayerReady"] == true)
                {
                    //PhotonNetwork.AutomaticallySyncScene = true;
                }
                else if ((bool)changedProps["PlayerReady"] == false)
                {

                }
            }
        }
    }

    public bool AllPlayersReady()
    {

        foreach (var photonPlayer in PhotonNetwork.PlayerList)
        {
            if ((bool)photonPlayer.CustomProperties["PlayerReady"] == false)
            {
                return false;
            }


        }
        if (PV.IsMine)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        
        return true;



    }



    protected void OnApplicationQuit() { PhotonNetwork.Disconnect(); }

  

    // Update is called once per frame
    void Update()
    {
        if(AllPlayersReady()&& !PhotonNetwork.AutomaticallySyncScene)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }
}
