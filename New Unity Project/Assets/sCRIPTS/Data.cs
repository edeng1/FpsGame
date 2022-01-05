using Photon.Pun;
using Proyecto26;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public static class Data 
{
    public static PlayerData playerData;
    
    public static string idToken;
    public static string localId;
    public static string username;
    private static int xpToNextLevel=100;

    
    public static bool Save(PlayerData _playerData)
    {
         
        PlayerPrefs.SetInt("xp", _playerData.xp);
        if (_playerData.xp >= GetExperienceToNextLevel(_playerData.level, _playerData.xp))
        {
            PlayerPrefs.SetInt("level", ++_playerData.level);
            return true;
            //++_playerData.level;
        }
        return false;
     

    }


    public static PlayerData Load()
    {

        if (!PlayerPrefs.HasKey("level") || !PlayerPrefs.HasKey("xp"))
        {
            return new PlayerData(0, 0);
        }

        return new PlayerData(PlayerPrefs.GetInt("level"),PlayerPrefs.GetInt("xp"));


      

    }

    private static int GetExperienceToNextLevel(int level,int xp)
    {
        return level *(int)Math.Sqrt(1000+xp);
    }


    public static bool Save2(PlayerData _playerData)
    {
        bool levelUp = false;
        if (_playerData.xp >= GetExperienceToNextLevel(_playerData.level, _playerData.xp))
        {

            levelUp =true;
            ++_playerData.level;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.data";
        FileStream stream = new FileStream(path, FileMode.Create);
        PlayerData data = new PlayerData(_playerData);
        formatter.Serialize(stream, data);
        stream.Close();
        return levelUp;
    }

    public static PlayerData Load2()
    {

       
        string path = Application.persistentDataPath + "/player.data";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            stream.Position = 0;
            PlayerData data;
            try
            {
                 data = (PlayerData)formatter.Deserialize(stream);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return new PlayerData(0, 0);
            }
            
            stream.Close();

            

            return data;
            
        }
        else
        {
            Debug.Log("Save file not found. Creating new user.");
            return new PlayerData(0, 0);
        }

    }


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
