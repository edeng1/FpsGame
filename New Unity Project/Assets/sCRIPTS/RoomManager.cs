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
using Proyecto26;
using UnityEngine.Networking;


public class RoomManager : MonoBehaviourPunCallbacks
{

    public static RoomManager Instance;
    public static PlayerData playerData;
    GameObject PM;
    public List<PlayerManager> playerManagers;
    PhotonView PV;
    public GameObject launcher;
    
    private void Awake()
    {
        playerManagers = new List<PlayerManager>();
        if(Instance)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        //Data.Save2(new PlayerData(true));
        playerData = Data.Load2();
            

        Instance = this;
        PV = GetComponent<PhotonView>();

        Hashtable hash = new Hashtable();
        hash.Add("PlayerReady", false);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);


    }
    public void LoadFromDataBase()
    {
        RestClient.Get<PlayerData>("https://multiplayer-game-258e6-default-rtdb.firebaseio.com/Player0000.json").Then(response =>
        {

            PhotonNetwork.NickName = response.username.ToString();
        }).Catch(err => {
            Debug.Log(err);

        } ) ;

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
        if (scene.buildIndex == 0&& PhotonNetwork.InRoom)
        {
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Launcher.instance.OnJoinedRoom();
            if (PV.IsMine)
            {
                Hashtable hash = new Hashtable();
                hash.Add("PlayerReady", true);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                
            }
            if (PhotonNetwork.IsMasterClient)
            {

            }
            
            Debug.Log("Im ready");

        }
        if (scene.buildIndex == 0 && !PhotonNetwork.InRoom)
        {
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Launcher.instance.OnLeftRoom();
            


        }
    }
    private bool SceneSynced()
    {
        return PhotonNetwork.AutomaticallySyncScene;
    }
    public void Spawn()
    {
        
        PM=PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        playerManagers.Add(PM.GetComponent<PlayerManager>());
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

    public void destroyPM()
    {
        PM.GetComponent<PlayerManager>().destroyGameObject();
    }

    protected void OnApplicationQuit() {
        //PV.RPC("QuitGame", RpcTarget.All);
        
        
        PhotonNetwork.Disconnect();
       
       
    }
    [PunRPC]
    void QuitGame()
    {
        Debug.Log("I quit");
    }


    void updateUserInfoUI()
    {
        if(Launcher.instance.usernameText.text != "Username: " + Data.playerData.username)
            Launcher.instance.usernameText.text = "Username: " + Data.playerData.username;
        if(PhotonNetwork.NickName != Data.playerData.username)
            PhotonNetwork.NickName = Data.playerData.username;
        if(Launcher.instance.xpText.text != "XP: " + Data.playerData.xp.ToString())
            Launcher.instance.xpText.text = "XP: " + Data.playerData.xp.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        //updateUserInfoUI();
        if (SceneManager.GetActiveScene().buildIndex == 0 && PhotonNetwork.InRoom)
        {
            Hashtable hash = new Hashtable();
            hash.Add("PlayerReady", true);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }


        if (AllPlayersReady())
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                foreach (var photonPlayer in PhotonNetwork.PlayerList)
                {
                    Debug.Log((bool)photonPlayer.CustomProperties["PlayerReady"]);
                    
                        
                    


                }
               
                
                Debug.Log("All Players Ready");
                Debug.Log("Sync scene? "+PhotonNetwork.AutomaticallySyncScene);
            }
        }
        if (!PhotonNetwork.IsMasterClient && !PhotonNetwork.AutomaticallySyncScene)
        {
            PhotonNetwork.AutomaticallySyncScene=true;

        }
        if(AllPlayersReady()&& !PhotonNetwork.AutomaticallySyncScene)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            if(PhotonNetwork.IsMasterClient)
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}
