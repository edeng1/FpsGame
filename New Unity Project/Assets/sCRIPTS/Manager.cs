using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerInfo
{
    
    public int actor;
    public string name;
    public short kills;
    public short deaths;
    public bool awayTeam;
    

    public PlayerInfo( int a, short k, short d, string n)
    {
        
        this.actor = a;
        this.kills = k;
        this.deaths = d;
        this.name = n;
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
    public int myind;
    public List<PlayerManager> playerList=new List<PlayerManager>();
    public Transform ScoreBoardUI;
    public Transform EndGameUI;

    public int mainMenuScene = 0;
    public int killCount = 3;

    private GameState state = GameState.Waiting;
    //public Dictionary<int, PlayerManager> playerDictionary = new Dictionary<int, PlayerManager>();
    public enum EventCodes : byte
    {
        NewPlayer,
        UpdatePlayers,
        ChangeStat,
        NewMatch,
        RefreshTimer
    }



    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Awake()
    {
        
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        NewPlayer_S();
    }

    // Update is called once per frame
    void Update()
    {
        if(state==GameState.Ending)
        {
            return;
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
                Debug.Log("Actor: "+p.actor + " Kills: "+p.kills+" Deaths:"+p.deaths+Random.Range(0,110));
            }
            
        }
    }

  


    private void ScoreBoard(Transform scoreBoard)
    {
        for(int i=2; i<scoreBoard.childCount;i++)
        {
            Destroy(scoreBoard.GetChild(i).gameObject);
        }
        GameObject playerCard = scoreBoard.GetChild(1).gameObject;

        playerCard.SetActive(false);

        List<PlayerInfo> sorted = SortPlayers(playerInfo);

        foreach(PlayerInfo p in sorted)
        {
            GameObject newCard = Instantiate(playerCard, scoreBoard) as GameObject;

            newCard.transform.Find("User").GetComponent<TMP_Text>().text = p.name.ToString();
            newCard.transform.Find("Kills").GetComponent<TMP_Text>().text = p.kills.ToString();
            newCard.transform.Find("Deaths").GetComponent<TMP_Text>().text = p.deaths.ToString();

            newCard.SetActive(true);
        }

        scoreBoard.gameObject.SetActive(true);



    }                                               

    private List<PlayerInfo> SortPlayers (List<PlayerInfo> _playerInfo)
    {
        List<PlayerInfo> sorted = new List<PlayerInfo>();

        while(sorted.Count<_playerInfo.Count)
        {
            short highest = -1;
            PlayerInfo selection = _playerInfo[0];

            foreach(PlayerInfo p in _playerInfo)
            {
                if (sorted.Contains(p)) continue;
                if(p.kills>highest)
                {
                    selection = p;
                    highest=p.kills;
                }
            }

            sorted.Add(selection);

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

       
        }
    }

    public void NewPlayer_S()
    {
        object[] package = new object[7];

        package[0] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[1] = (short)0;
        package[2] = (short)0;
        package[3] = PhotonNetwork.LocalPlayer.NickName;
        

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
            (string)data[3]
            
        );

        playerInfo.Add(p);

        

        UpdatePlayers_S((int)state,playerInfo);
    }

    public void UpdatePlayers_S(int state, List<PlayerInfo> info)
    {
        object[] package = new object[info.Count+1];

        package[0] = state;
        for (int i = 0; i < info.Count; i++)
        {
            object[] piece = new object[4];

           
            piece[0] = info[i].actor;
            piece[1] = info[i].kills;
            piece[2] = info[i].deaths;
            piece[3] = info[i].name;

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
                (string)extract[3]

            );

            playerInfo.Add(p);

            if (PhotonNetwork.LocalPlayer.ActorNumber == p.actor)
            {
                myind = i-1;
            }
        }
        StateCheck();
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
                        Debug.Log($"Player {playerInfo[i].name} : kills = {playerInfo[i].kills}");
                        break;

                    case 1: //deaths
                        playerInfo[i].deaths += amt;
                        Debug.Log($"Player {playerInfo[i].name} : deaths = {playerInfo[i].deaths}");
                        break;
                }
                
                if (ScoreBoardUI.gameObject.activeSelf) ScoreBoard(ScoreBoardUI);

                break;

                
            }
        }
        ScoreCheck();
       
    }
    private void StateCheck()
    {
        if(state==GameState.Ending)
        {
            EndGame();
        }
    }

    private void ScoreCheck()
    {
        bool detectWin = false;

        foreach(PlayerInfo p in playerInfo)
        {
            if(p.deaths>=killCount)
            {
                detectWin = true;
                break;
            }
        }

        if(detectWin)
        {
            if(PhotonNetwork.IsMasterClient && state!=GameState.Ending)
            {
                UpdatePlayers_S((int)GameState.Ending, playerInfo);
            }
        }

    }

    private void EndGame()
    {
        state = GameState.Ending;
        //Destroy(RoomManager.Instance.gameObject);

        
        if (PhotonNetwork.IsMasterClient)
        {
            
            PhotonNetwork.DestroyAll();
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            
        }
      
        StartCoroutine(End(5f));
        

        ScoreBoardUI.gameObject.SetActive(true);
        ScoreBoard(ScoreBoardUI);

       

    }

    private IEnumerator End(float wait)
    {
        
        yield return new WaitForSeconds(wait);


        //PhotonNetwork.LeaveRoom();
        PhotonNetwork.AutomaticallySyncScene = false;
        
        PhotonNetwork.LoadLevel(0);
        
      
    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.LoadLevel(0);
    }



}
