using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class PlayerInfo
{

    public int actor;
    public string name;
    public short kills;
    public short deaths;
    public short flagCaps;
    public bool awayTeam;



    public PlayerInfo( int a, short k, short d, short fc, string n,bool t)
    {
        
        this.actor = a;
        this.kills = k;
        this.deaths = d;
        this.flagCaps = fc;
        this.name = n;
        this.awayTeam = t;
    }
}

public enum GameState 
{
    Waiting=0,
    Starting=1,
    Playing=2,
    Ending=3
}

public class Manager : MonoBehaviourPunCallbacks, IOnEventCallback
{

    public static Manager Instance;
    public List<PlayerInfo> playerInfo = new List<PlayerInfo>();
    public List<GameObject> playerList = new List<GameObject>();
    
    public int myind;
    public Transform flag;
    public Transform ScoreBoardUI;
    public TMP_Text TimerUI;
    public Transform EndGameUI;
    public TMP_Text HomeScore;
    public TMP_Text AwayScore;
    public TMP_Text FFAScore;
    public TMP_Text KillUI;//unnecessary, used in PlayerController
    public int matchLength = 180;
    private Coroutine timerCoroutine;
    private int currentMatchTime;
    public int mainMenuScene = 0;
    public int killCount = 3;
    public int teamKillCount=4;
    public int teamFlagCount = 4;
    public int awayScore=0;
    public int homeScore=0;
    int actNum;
    private bool playerAdded;

    private GameState state = GameState.Waiting;
    //public Dictionary<int, PlayerManager> playerDictionary = new Dictionary<int, PlayerManager>();
    public enum EventCodes : byte
    {
        NewPlayer,
        UpdatePlayers,
        ChangeStat,
        NewMatch,
        RefreshTimer,
        FlagPickup,
        PlayerLeft
    }



    private void OnEnable()
    {
        
        PhotonNetwork.AddCallbackTarget(this);
        //StartCoroutine(AddEvent());
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
       // UIEventSystem.current.onPlayerKilled -= EventKillUI;
    }
    IEnumerator AddEvent()//unnecessary, used in PlayerController
    {
        yield return new WaitForSeconds(3f);
        UIEventSystem.current.onPlayerKilled += EventKillUI;

    }
   
    void EventKillUI(string eventText)//unnecessary, used in PlayerController
    {
        //StartCoroutine(DisplayKillEvent(eventText));
    }
    IEnumerator DisplayKillEvent(string eventText)//unnecessary, used in PlayerController
    {
        //string[] s = eventText.Split(' ');
        // eventKillUI.text = s[0] + " " + ((SingeShotGun)items[itemIndex]).itemInfo.itemName+" "+ s[1];
        KillUI.text = eventText;
        yield return new WaitForSeconds(1f);
        KillUI.text = "";
    }

    private void Awake()
    {
           
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        actNum = PhotonNetwork.LocalPlayer.ActorNumber;
        EndGameUI = ScoreBoardUI;
        if (PhotonNetwork.IsMasterClient)
        {
            playerAdded = true;
            GameSettings.IsAwayTeam = CalculateTeam();
            RoomManager.Instance.Spawn();
        }
        
        NewPlayer_S();
        InitializeTimer();
        RefreshStats();
    }

    // Update is called once per frame
    void Update()
    {

        
        if (state==GameState.Ending)
        {
            return;
        }
        if (playerAdded == true&&state==GameState.Waiting)
        {
            state = GameState.Playing;
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(playerInfo==null)
            {
                Debug.Log("player info is null");
            }
            else if(ScoreBoardUI.gameObject.activeSelf)
            {
                ScoreBoardUI.gameObject.SetActive(false);
            }
            else
            {
                ScoreBoard(ScoreBoardUI);
            }
            foreach(PlayerInfo p in playerInfo)
            {
                Debug.Log("Actor: "+p.actor + " Kills: "+p.kills+" Deaths:"+p.deaths+" FlagCaps"+p.flagCaps +"p.AwayTeam? "+p.awayTeam);
                Debug.Log("My Actor "+PhotonNetwork.LocalPlayer.ActorNumber+" My GameSettings.AwayTeam "+GameSettings.IsAwayTeam);
                
            }
            
        }
    }

  


    private void ScoreBoard(Transform scoreBoard)
    {

        //specify gamemode
        if (GameSettings.GameMode == GameMode.FFA) { scoreBoard = scoreBoard.GetChild(3); }//FFA
        if (GameSettings.GameMode == GameMode.TDM) { scoreBoard = scoreBoard.GetChild(4); }//TDM
        if (GameSettings.GameMode == GameMode.CTF) { scoreBoard = scoreBoard.GetChild(5); }//CTF
        for (int i=2; i<scoreBoard.childCount;i++)
        {
            Destroy(scoreBoard.GetChild(i).gameObject);
        }

        //set leaderboard map and mode
        scoreBoard.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = System.Enum.GetName(typeof(GameMode), GameSettings.GameMode); 
        scoreBoard.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = System.Enum.GetName(typeof(GameMap), SceneManager.GetActiveScene().buildIndex - 1);
        
        GameObject playerCard = scoreBoard.GetChild(1).gameObject;
        playerCard.SetActive(false);

        List<PlayerInfo> sorted = SortPlayers(playerInfo);

        foreach(PlayerInfo p in sorted)
        {

            GameObject newCard = Instantiate(playerCard, scoreBoard) as GameObject;

            if(GameSettings.GameMode!=GameMode.FFA)
            {
                
                if (p.awayTeam)
                {
                    
                    newCard.transform.GetComponent<Image>().color=Color.red;
                    
                }
                if (!p.awayTeam)
                {
                    Color blueA = new Color32(87, 95, 197, 205);
                    newCard.transform.GetComponent<Image>().color = blueA;
                   
                }

            }

            var user=newCard.transform.Find("User").GetComponent<TMP_Text>();
            var kills = newCard.transform.Find("Kills").GetComponent<TMP_Text>();
            var deaths= newCard.transform.Find("Deaths").GetComponent<TMP_Text>();
            user.text = p.name.ToString();
            kills.text = p.kills.ToString();
            deaths.text = p.deaths.ToString();
            if (GameSettings.GameMode == GameMode.CTF)
            {
                var caps= newCard.transform.Find("Caps").GetComponent<TMP_Text>();
                caps.text = p.flagCaps.ToString();
                if (p.actor.Equals(PhotonNetwork.LocalPlayer.ActorNumber)) { caps.color = Color.yellow; }

            }

            if (p.actor.Equals(PhotonNetwork.LocalPlayer.ActorNumber))
            {
                
                user.color = Color.yellow;
                kills.color = Color.yellow;
                deaths.color = Color.yellow;
            }

            newCard.SetActive(true);
        }

        scoreBoard.gameObject.SetActive(true);
        scoreBoard.parent.gameObject.SetActive(true);


    }        
    private void RefreshTimerUI()
    {
        string minutes = (currentMatchTime / 60).ToString("00");
        string seconds= (currentMatchTime % 60).ToString("00");
        TimerUI.text = $"{minutes}:{seconds}";

    }

    private List<PlayerInfo> SortPlayers (List<PlayerInfo> _playerInfo)
    {
        List<PlayerInfo> sorted = new List<PlayerInfo>();

        if (GameSettings.GameMode == GameMode.FFA)
        {

            while (sorted.Count < _playerInfo.Count)
            {
                short highest = -1;
                PlayerInfo selection = _playerInfo[0];

                foreach (PlayerInfo p in _playerInfo)
                {
                    if (sorted.Contains(p)) continue;
                    if (p.kills > highest)
                    {
                        selection = p;
                        highest = p.kills;
                    }
                }

                sorted.Add(selection);

            }
        }

        if (GameSettings.GameMode != GameMode.FFA)
        {
            List<PlayerInfo> homeSorted = new List<PlayerInfo>();
            List<PlayerInfo> awaySorted = new List<PlayerInfo>();

            int homeSize = 0;
            int awaySize = 0;

            foreach(PlayerInfo p in _playerInfo)
            {
                if (p.awayTeam) awaySize++;
                else homeSize++;
            }

            while (homeSorted.Count < homeSize)
            {
                short highest = -1;
                PlayerInfo selection = _playerInfo[0];

                foreach (PlayerInfo p in _playerInfo)
                {
                    if (p.awayTeam) continue;
                    if (homeSorted.Contains(p)) continue;
                    if (p.kills > highest)
                    {
                        selection = p;
                        highest = p.kills;
                    }
                }

                homeSorted.Add(selection);

            }
            while (awaySorted.Count < awaySize)
            {
                short highest = -1;
                PlayerInfo selection = _playerInfo[0];

                foreach (PlayerInfo p in _playerInfo)
                {
                    if (!p.awayTeam) continue;
                    if (awaySorted.Contains(p)) continue;
                    if (p.kills > highest)
                    {
                        selection = p;
                        highest = p.kills;
                    }
                }

                awaySorted.Add(selection);

            }
            sorted.AddRange(homeSorted);
            sorted.AddRange(awaySorted);
        }

        return sorted;

    }
    


    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code >= 200) return;

        EventCodes e = (EventCodes)photonEvent.Code;
        object[] o = (object[])photonEvent.CustomData;

        switch (e)
        {
            case EventCodes.NewPlayer:
                NewPlayer_R(o);
                break;

            case EventCodes.UpdatePlayers:
                UpdatePlayers_R(o);
                break;

            case EventCodes.ChangeStat:
                ChangeStat_R(o);
                break;
            case EventCodes.RefreshTimer:
                RefreshTimer_R(o);
                break;
            case EventCodes.FlagPickup:
                FlagPickUp_R(o);
                break;
            case EventCodes.PlayerLeft:
                PlayerLeft_R(o);
                break;


        }
    }

    private bool CalculateTeam()
    {
        Debug.Log(PhotonNetwork.PlayerList.Length);
        if(PhotonNetwork.LocalPlayer.ActorNumber> PhotonNetwork.PlayerList.Length)//if player leaves then rejoins
        {
            Debug.Log("Calculated with PlayerListLength");
            return PhotonNetwork.PlayerList.Length % 2 == 0;
        }
        Debug.Log("Calculated with ActorNum");
        return PhotonNetwork.LocalPlayer.ActorNumber % 2 == 0;
    }

    public void NewPlayer_S()
    {
        object[] package = new object[7];

        package[0] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[1] = (short)0;
        package[2] = (short)0;
        package[3] = (short)0;
        package[4] = PhotonNetwork.LocalPlayer.NickName;
        package[5] = CalculateTeam();
       

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
        );
    }
    public void NewPlayer_R(object[] data)
    {
        PlayerInfo p = new PlayerInfo(
            
            (int)data[0],
            (short)data[1],
            (short)data[2],
            (short)data[3],

            (string)data[4],
            (bool)data[5]

        );

        playerInfo.Add(p);

        RoomManager.Instance.getPlayerManager().TrySync();
        if (GameSettings.GameMode == GameMode.CTF) {
            if (PhotonNetwork.IsMasterClient)
            {

                if (FindObjectOfType<FlagManager>())
                    FlagManager.Instance.TrySync();
            }
        }
       

            UpdatePlayers_S((int)state,playerInfo);
    }

    public void PlayerLeft_S(int actorNumber)
    {
        object[] package = new object[1];

        package[0] = PhotonNetwork.LocalPlayer.ActorNumber;
        PhotonNetwork.RaiseEvent(
           (byte)EventCodes.PlayerLeft,
           package,
           new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
           new SendOptions { Reliability = true }
       );
    }

    public void PlayerLeft_R(object[] data)
    {

        foreach(PlayerInfo p in playerInfo)
        {
            if (p.actor.Equals((int)data[0]))
            {
                playerInfo.Remove(p);
                break;
            }
        }
        UpdatePlayers_S((int)state,playerInfo);
        Debug.Log("Player quit");
    }

    public void UpdatePlayers_S(int state, List<PlayerInfo> info)
    {
        object[] package = new object[info.Count+1];

        package[0] = state;
        for (int i = 0; i < info.Count; i++)
        {
            object[] piece = new object[6];

           
            piece[0] = info[i].actor;
            piece[1] = info[i].kills;
            piece[2] = info[i].deaths;
            piece[3] = info[i].flagCaps;
            piece[4] = info[i].name;
            piece[5] = info[i].awayTeam;
            package[i+1] = piece;
        }

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.UpdatePlayers,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }

    public void UpdatePlayers_R(object[]data)
    {
        state = (GameState)data[0];
        playerInfo = new List<PlayerInfo>();
        
        for (int i = 1; i < data.Length; i++)
        {
            object[] extract = (object[])data[i];

            PlayerInfo p = new PlayerInfo(


                (int)extract[0],
                (short)extract[1],
                (short)extract[2],
                (short)extract[3],
                (string)extract[4],
                (bool)extract[5]

            );

            playerInfo.Add(p);

            if (PhotonNetwork.LocalPlayer.ActorNumber == p.actor)
            {
                myind = i-1;

                if(!playerAdded)
                {
                    playerAdded = true;
                    GameSettings.IsAwayTeam = p.awayTeam;
                  
                      RoomManager.Instance.Spawn();
                      RoomManager.Instance.getPlayerManager().TrySync();
                }
            }
        }
        StateCheck();
        if (homeScore > awayScore)
            EndGameUI = gameObject.transform.GetChild(0).Find("HomeEndScoreBoard");
        if (awayScore > homeScore)
            EndGameUI = gameObject.transform.GetChild(0).Find("AwayEndScoreBoard");
        else { EndGameUI = gameObject.transform.GetChild(0).Find("ScoreBoard"); }
    }

    public void ChangeStat_S(int actor, byte stat, byte amt)
    {
        object[] package = new object[] { actor, stat, amt };

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.ChangeStat,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }
    public void ChangeStat_R(object[] data)
    {
        int actor = (int)data[0];
        byte stat = (byte)data[1];
        byte amt = (byte)data[2];

        for (int i = 0; i < playerInfo.Count; i++)
        {
            if (playerInfo[i].actor == actor)
            {
                switch (stat)
                {
                    case 0: //kills
                        playerInfo[i].kills += amt;
                        if (GameSettings.GameMode == GameMode.TDM)
                        {
                            if (playerInfo[i].awayTeam)
                            {
                                awayScore += amt;
                            }
                            if (!playerInfo[i].awayTeam)
                            {
                                homeScore += amt;
                            }
                        }
                        Debug.Log($"Player {playerInfo[i].name} : kills = {playerInfo[i].kills} :actor = {actor}");
                        
                        break;

                    case 1: //deaths
                        playerInfo[i].deaths += amt;
                        Debug.Log($"Player {playerInfo[i].name} : deaths = {playerInfo[i].deaths}");
                        break;
                    case 2: //flag caps
                        playerInfo[i].flagCaps += amt;
                        if (GameSettings.GameMode == GameMode.CTF)
                        {
                            
                            if (playerInfo[i].awayTeam)
                            {
                                awayScore += amt;
                            }
                            if (!playerInfo[i].awayTeam)
                            {
                                homeScore += amt;
                            }
                        }
                        Debug.Log($"Player {playerInfo[i].name} : flag captures = {playerInfo[i].flagCaps} :actor = {actor} :LocalActorNum: {PhotonNetwork.LocalPlayer.ActorNumber}");
                        break;
                }

                RefreshStats();
                if (ScoreBoardUI.gameObject.activeSelf) ScoreBoard(ScoreBoardUI);

                break;

                
            }
        }
        ScoreCheck();
       
    }

    public void RefreshTimer_S()
    {
        object[] package = new object[] { currentMatchTime };

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.RefreshTimer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
        );
    }
    public void RefreshTimer_R(object[] data)
    {
        currentMatchTime = (int)data[0];
        RefreshTimerUI();
    }

        private void StateCheck()
    {
        if(state==GameState.Ending)
        {
            EndGame();
        }
    }
    public void FlagPickUp_S(object flag)
    {
        object[] package = new object[] { flag };
        PhotonNetwork.RaiseEvent(
           (byte)EventCodes.FlagPickup,
           package,
           new RaiseEventOptions { Receivers = ReceiverGroup.All },
           new SendOptions { Reliability = true }
       );
    }
    public void FlagPickUp_R(object[] data)
    {
        flag = (Transform)data[0];

    }

    public void RefreshStats()
    {
        if (GameSettings.GameMode != GameMode.FFA)
        {
            HomeScore.transform.parent.parent.gameObject.SetActive(true);
            //AwayScore.transform.parent.gameObject.SetActive(true);
            HomeScore.text = $"Home {homeScore}";
            AwayScore.text = $"Away {awayScore}";
        }
        else
        {
            FFAScore.gameObject.SetActive(true);
            if (playerInfo.Count > myind)
            {
                
                FFAScore.text = $"Kills {playerInfo[myind].kills}";
            }
            else
            {
                
                FFAScore.text = "Kills 0";
            }
           
        }


    }

    private void ScoreCheck()
    {
        bool detectWin = false;

        if (GameSettings.GameMode == GameMode.FFA)
        {
            detectWin = FFAWin();
        }
        if (GameSettings.GameMode == GameMode.TDM)
        {
            detectWin = TDMWin();
        }
        if (GameSettings.GameMode == GameMode.CTF)
        {
            detectWin = CTFWin();
        }


        if (detectWin)
        {
            if(PhotonNetwork.IsMasterClient && state!=GameState.Ending)
            {
                UpdatePlayers_S((int)GameState.Ending, playerInfo);
            }
        }

    }
    private bool FFAWin()
    {
        bool detectWin = false;
        foreach (PlayerInfo p in playerInfo)
        {
            if (p.deaths >= killCount)
            {
                detectWin = true;
                break;
            }
        }
        return detectWin;
    }
    private bool TDMWin()
    {
        bool detectWin = false;
        if(homeScore>=teamKillCount)
        {
            EndGameUI = gameObject.transform.GetChild(0).Find("HomeEndScoreBoard");
            detectWin = true;
        }
        if(awayScore >= teamKillCount)
        {
            EndGameUI = gameObject.transform.GetChild(0).Find("AwayEndScoreBoard");
            detectWin = true;
        }
        else
        {
            EndGameUI = gameObject.transform.GetChild(0).GetChild(0);
        }
        
        return detectWin;
    }
    private bool CTFWin()
    {
        bool detectWin = false;
        if (homeScore >= teamFlagCount)
        {
            EndGameUI = gameObject.transform.GetChild(0).Find("HomeEndScoreBoard");
            detectWin = true;
        }
        if (awayScore >= teamFlagCount)
        {
            EndGameUI = gameObject.transform.GetChild(0).Find("AwayEndScoreBoard");
            detectWin = true;
        }
        else
        {
            EndGameUI = gameObject.transform.GetChild(0).GetChild(0);
        }

        return detectWin;
    }


    private void InitializeTimer()
    {
        currentMatchTime = matchLength;
        RefreshTimerUI();
        if (PhotonNetwork.IsMasterClient)
        {
            timerCoroutine = StartCoroutine(Timer());
        }
    }
    public GameState getState()
    {
        return state;
    }
    private void EndGame()
    {
        state = GameState.Ending;
        //Destroy(RoomManager.Instance.gameObject);
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        currentMatchTime = 0;
        RefreshTimerUI();



        if (PhotonNetwork.IsMasterClient)
        {
            
            PhotonNetwork.DestroyAll();
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            
        }
      
        StartCoroutine(End(5f));

        if (GameSettings.GameMode == GameMode.FFA)
        {
            ScoreBoardUI.gameObject.SetActive(true);
            ScoreBoard(ScoreBoardUI);
        }
        if (GameSettings.GameMode != GameMode.FFA)
        {
            ScoreBoardUI.gameObject.SetActive(true);
            ScoreBoard(EndGameUI);
            //EndGameUI.gameObject.SetActive(true);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


    }

    private IEnumerator End(float wait)
    {
        
        yield return new WaitForSeconds(wait);


        //PhotonNetwork.LeaveRoom();

        PhotonNetwork.AutomaticallySyncScene = false;
        
        //if(PhotonNetwork.IsMasterClient) //I changed this. please no break
        PhotonNetwork.LoadLevel(0);
        
      
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(1f);
        currentMatchTime -= 1;
        if(currentMatchTime<=0)
        {
            
            timerCoroutine = null;
            UpdatePlayers_S((int)GameState.Ending, playerInfo);
        }
        else
        {
            RefreshTimer_S();
            timerCoroutine = StartCoroutine(Timer());
        }

    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.LoadLevel(0);
    }



    private void OnApplicationQuit()
    {
        PlayerLeft_S(PhotonNetwork.LocalPlayer.ActorNumber);
        Debug.Log("I quit");
    }


    public Tuple<string,string> GetPlayerNames(int myActor,int yourActor)
    {
        string myName = "";
        string yourName = "";

        for (int i = 0; i < playerInfo.Count; i++)
        {
            if (playerInfo[i].actor ==myActor)
            {
                myName = playerInfo[i].name;
            }
            if (playerInfo[i].actor == yourActor)
            {
                yourName = playerInfo[i].name;
            }
            if (myName != "" && yourName != "")
            {
                break;
            }
        }

        return Tuple.Create(myName, yourName);

    }

    public string GetPlayerName(int actor)
    {
        for (int i = 0; i < playerInfo.Count; i++)
        {
            if (playerInfo[i].actor == actor)
            {
                return playerInfo[i].name;
            }
        }
        return "";
    }

}
