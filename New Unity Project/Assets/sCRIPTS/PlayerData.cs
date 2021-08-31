using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData 
{
    public string username;
    public int xp;
    public int level;
    public string localId;
    public string idToken;

    public PlayerData()
    {

    }
    public PlayerData(string _username, int _xp, int _level, string _localId, string _idToken)
    {
        this.username = _username;
        this.xp = _xp;
        this.level = _level;
        localId = _localId;
        idToken = _idToken;
    }
    public PlayerData(bool firstTimeUser)
    {
        xp = 0;
        level = 0;
    }

}
