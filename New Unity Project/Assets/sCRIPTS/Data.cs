using Photon.Pun;
using Proyecto26;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Data 
{
    public static PlayerData playerData;
    
    public static string idToken;
    public static string localId;
    public static string username;

    

    public static void SaveToDatabase(PlayerData _playerData)
    {
        
        playerData = _playerData;
        RestClient.Put("https://multiplayer-game-258e6-default-rtdb.firebaseio.com/users/"+ playerData.localId + ".json?auth="+ playerData.idToken, playerData);
        Debug.Log("Saved" + playerData.username);
        
    }

    public static void LoadFromDataBase()
    {
        RestClient.Get<PlayerData>("https://multiplayer-game-258e6-default-rtdb.firebaseio.com/users/" + localId + ".json?auth=" + idToken).Then(response =>
        {
            playerData = response;
            //playerData.username = response.username;
            PhotonNetwork.NickName =  response.username.ToString();
        });
        
    }
    


}
