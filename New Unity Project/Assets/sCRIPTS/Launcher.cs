using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using UnityEngine.UI;

[System.Serializable]
public class MapData
{
    public string name;
    public int scene;
}

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;


    public static RoomInfo currentRoomInfo;

    [SerializeField] Image mapImage;
    [SerializeField] TMP_Text modeValueText_InRoom;
    [SerializeField] TMP_InputField usernameInputField;
    [SerializeField] Button usernameSaveButton;
    [SerializeField] TMP_InputField roomNameIF;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text mapValueText;
    [SerializeField] TMP_Text mapValueText_OptionsMenu;
    [SerializeField] TMP_Text modeValueText;
    [SerializeField] TMP_Text modeValueText_OptionsMenu;
    [SerializeField] Slider maxPlayersSlider;
    [SerializeField] TMP_Text maxPlayersValueText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;
    [SerializeField] GameObject gameOptionsButton;
    [SerializeField] RoomManager roomManagerPrefab;
    [SerializeField] public Camera mainCamera;
    [SerializeField] public TMP_Text usernameText;
    [SerializeField] public TMP_Text xpText;
    [SerializeField] public TMP_Text levelText;
    private RoomOptions roomOptions;
    public static List<RoomInfo> roomItems;
    public MapData[] maps;
    private int currentMap = 0;

    public static bool inRoom = false;
    
    // Start is called before the first frame update



    void Awake()
    {
        if (instance)///////////////////
        {
            
            return;
        }
      
        instance = this;
    }
    void Start()
    {
        Debug.Log(roomItems);
        //Debug.Log(Data.idToken);
        if (PlayerPrefs.HasKey("NickName")) { PhotonNetwork.NickName = PlayerPrefs.GetString("NickName"); }
        maxPlayersSlider.value = 8;

           // if (Data.idToken == null)
           // {
           //MenuManager.instance.OpenMenu("Login");
           // }
           // else
           // {

        PhotonNetwork.ConnectUsingSettings();
            CreateRoomManager();
            mapValueText.text = "Map: " + maps[(int)GameSettings.GameMap].name;
            SetMapImageAndModeInRoom();
            modeValueText.text = "Mode: " + System.Enum.GetName(typeof(GameMode), GameSettings.GameMode);
            mapValueText_OptionsMenu.text = "Map: " + maps[(int)GameSettings.GameMap].name;
            modeValueText_OptionsMenu.text = "Mode: " + System.Enum.GetName(typeof(GameMode), GameSettings.GameMode);
        if (RoomManager.playerData != null) {
            levelText.text = "Level: " + RoomManager.playerData.level.ToString();
            xpText.text = "XP: " + RoomManager.playerData.xp.ToString();
            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
            properties.Add("level", RoomManager.playerData.level.ToString());
            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        }
        else
        {
            levelText.text = "Level: 0";
            xpText.text = "XP: 0";
        }

        // }


    }
    public void SetMapImageAndModeInRoom(RoomInfo info=null)
    {
        Texture2D MapImage;
        if (info!=null)
        {
            MapImage = Resources.Load<Texture2D>("Maps/Map" + (info.CustomProperties["maps"]).ToString());
            modeValueText_InRoom.text = System.Enum.GetName(typeof(GameMode), info.CustomProperties["modes"]); 
        }
        else
        {
            MapImage = Resources.Load<Texture2D>("Maps/Map" + ((int)GameSettings.GameMap).ToString());
            modeValueText_InRoom.text = System.Enum.GetName(typeof(GameMode), GameSettings.GameMode);
        }
        
        
       
        if (MapImage == null) { Debug.Log("Map null"); }
        if (MapImage != null)
        {

            Rect rect = new Rect(0, 0, MapImage.width, MapImage.height);
            mapImage.GetComponent<Image>().sprite = Sprite.Create(MapImage, rect, new Vector2(0.5f, 0.5f));
        }
        
    }
    public void StartAfterLogin()
    {
        PhotonNetwork.ConnectUsingSettings();
        CreateRoomManager();
        mapValueText.text = "Map: " + maps[currentMap].name;
        modeValueText.text = "Mode: " + System.Enum.GetName(typeof(GameMode), GameSettings.GameMode);
        mapValueText_OptionsMenu.text = "Map: " + maps[currentMap].name;
        modeValueText_OptionsMenu.text = "Mode: " + System.Enum.GetName(typeof(GameMode), GameSettings.GameMode);
        
            levelText.text = RoomManager.playerData.level.ToString();
        
            xpText.text = RoomManager.playerData.xp.ToString();
    }
    public void SaveUserName()
    {
        //Data.playerData.username = usernameInputField.text;
        PlayerPrefs.SetString("NickName", usernameInputField.text);
        PhotonNetwork.NickName = PlayerPrefs.GetString("NickName");
        //Data.SaveToDatabase(new PlayerData(usernameInputField.text,Data.playerData.xp,Data.playerData.level,Data.localId,Data.idToken));
    }

    private void Update()
    {
        
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.instance.OpenMenu("Title");/////////////////////
        //StartCoroutine(Wait(MenuManager.instance));
       
        if (roomItems != null) { createRoomList(roomItems); }
        
        Debug.Log("Joined Lobby");
        if (PhotonNetwork.NickName =="")
        {
            PhotonNetwork.NickName = "Player" + Random.Range(0, 1000).ToString("0000");
            
        }
        Debug.Log(PhotonNetwork.NickName);
        
    }
    IEnumerator Wait(MenuManager menus)
    {
        
        yield return new WaitUntil(() => menus.isLoaded);
        
        MenuManager.instance.OpenMenu("Title");


    }

   public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameIF.text))
        {
            return;
        }
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)maxPlayersSlider.value;
        options.CustomRoomPropertiesForLobby = new string[] { "map", "mode" };
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties.Add("map", (int)GameSettings.GameMap);
        properties.Add("mode", (int)GameSettings.GameMode);
        options.CustomRoomProperties = properties;

        

        PhotonNetwork.CreateRoom(roomNameIF.text,options);
        //SetMapImageAndModeInRoom();
        MenuManager.instance.OpenMenu("Loading");/////////////////////////////////
    }

    public void ConfirmGameOptions()//Confirms changes to game options while in room. GameOptions button.
    {
        RoomOptions options = new RoomOptions();

        
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties.Add("map", (int)GameSettings.GameMap);
        properties.Add("mode", (int)GameSettings.GameMode);
       


        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)//When Gamemode or Map is changed in game options, this sends it to all the clients.
    {
        foreach(var p in propertiesThatChanged)
        {
            Debug.Log(p.Key);
        }
        if (propertiesThatChanged.ContainsKey("map")){
            currentMap = (int)propertiesThatChanged["map"];
            GameSettings.GameMap = (GameMap)currentMap;
        }
        if (propertiesThatChanged.ContainsKey("mode"))
        {
           int modeNum= (int)propertiesThatChanged["mode"];
            GameSettings.GameMode = (GameMode)modeNum;
        }

        Debug.Log(propertiesThatChanged);
        SetMapImageAndModeInRoom();
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
    }



    public void ChangeMap()
    {
        currentMap= (int)GameSettings.GameMap + 1; ;
        if (currentMap >= maps.Length)
        { currentMap = 0; }
        GameSettings.GameMap = (GameMap)currentMap;
        mapValueText.text = "Map: " + maps[currentMap].name;
        mapValueText_OptionsMenu.text = "Map: " + maps[currentMap].name;
       
    }
    public void ChangeMode()
    {
        int newMode = (int)GameSettings.GameMode + 1;
        if (newMode >= System.Enum.GetValues(typeof(GameMode)).Length)
        { newMode = 0; }
        GameSettings.GameMode = (GameMode)newMode;
        modeValueText.text = "Mode: " + System.Enum.GetName(typeof(GameMode),newMode);
        modeValueText_OptionsMenu.text= "Mode: " + System.Enum.GetName(typeof(GameMode), newMode);
    }

    public void ChangeMaxPlayersSlider(float _value)
    {
        maxPlayersValueText.text = Mathf.RoundToInt(_value).ToString();
    }

    public override void OnJoinedRoom()
    {

        inRoom = true;
        MenuManager.instance.OpenMenu("Room");///////////
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        Debug.Log("Joined Room "+ PhotonNetwork.CurrentRoom.Name);
        
        Player[] players = PhotonNetwork.PlayerList;
        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < players.Count(); i++)
        {
            /*
            if (players[i] == PhotonNetwork.LocalPlayer)
            {
                ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
                properties.Add("level", RoomManager.playerData.level.ToString());
                players[i].SetCustomProperties(properties);
            }*/
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
            
        }
        Debug.Log(roomItems);
        gameOptionsButton.SetActive(PhotonNetwork.IsMasterClient);
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        gameOptionsButton.SetActive(PhotonNetwork.IsMasterClient);
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.instance.OpenMenu("Error");//////////////////////////////
    }

    bool gameStarting = false;
    public void StartGame()////////////////////////////////////////////
    {
       

        if (gameStarting == false)
        {
            gameStarting = true;
            if (RoomManager.Instance.AllPlayersReady())
            {
                Debug.Log("All players ready");

                PhotonNetwork.LoadLevel(maps[(int)GameSettings.GameMap].scene);
            }
            
        }



    }
   
    
   

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("Loading");////
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("Loading");////////////////////
        GameSettings.GameMode = (GameMode)info.CustomProperties["mode"];
        GameSettings.GameMap = (GameMap)info.CustomProperties["map"];
        SetMapImageAndModeInRoom();
        Debug.Log("Mode: " + System.Enum.GetName(typeof(GameMode), GameSettings.GameMode));
        currentRoomInfo = info;
        
    }

    private static Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        if(PhotonNetwork.IsConnectedAndReady)
            PhotonNetwork.JoinLobby();
        //OnJoinedLobby();
        inRoom = false;
        GameSettings.GameMode = (GameMode)0;
        currentMap = 0;
        GameSettings.GameMap = (GameMap)0;
        mapValueText.text = "Map: " + maps[(int)GameSettings.GameMap].name;
        modeValueText.text = "Mode: " + System.Enum.GetName(typeof(GameMode), (int)GameSettings.GameMode);
        roomNameIF.text = "";
        cachedRoomList.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        roomItems = roomList;
        //Debug.Log(roomItems[0].Name);
        createRoomList(roomList);


    }
    
    //public List<RoomInfo> _roomList;
    public void createRoomList(List<RoomInfo> roomList)

    {
        foreach (Transform t in roomListContent)
        {
            Destroy(t.gameObject);
        }
        /*foreach (RoomInfo r in roomList)
        {
           if (r.RemovedFromList)
               continue;


           // _roomList.Add(r);
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(r);

        }
        */
        

    

    for (int i = 0; i<roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
        foreach (KeyValuePair<string, RoomInfo> entry in cachedRoomList)
        {
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(cachedRoomList[entry.Key]);
        }
    }

   

    

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        /*
        if (newPlayer == PhotonNetwork.LocalPlayer)
        {
            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
            properties.Add("level", RoomManager.playerData.level.ToString());
            newPlayer.SetCustomProperties(properties);
        }*/
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void CreateRoomManager()
    {
        if(!FindObjectOfType<RoomManager>())
            Instantiate(roomManagerPrefab).name="RoomManager";
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
    }
    


}
