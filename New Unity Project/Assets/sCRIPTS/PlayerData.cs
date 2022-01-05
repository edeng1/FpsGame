using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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
    public PlayerData(int _level, int _xp)
    {
        //this.username = _username;
        this.xp = _xp;
        this.level = _level;
        //localId = _localId;
        //idToken = _idToken;
    }
    public PlayerData(bool firstTimeUser)
    {
        xp = 0;
        level = 0;
    }
    public PlayerData(PlayerData playerData)
    {
        xp = playerData.xp;
        level = playerData.level;
    }

}
