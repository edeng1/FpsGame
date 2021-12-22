using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text mapText;
    [SerializeField] TMP_Text modeText;
    [SerializeField] TMP_Text playerCountText;

    public RoomInfo info;

   public void SetUp(RoomInfo _info)
    {
        info = _info;
        text.text = info.Name;
        if (info.CustomProperties.ContainsKey("map"))
        {
            mapText.text = "" + Launcher.instance.maps[(int)info.CustomProperties["map"]].name;
        }
        if(info.CustomProperties.ContainsKey("mode"))
        {
            modeText.text= "" + System.Enum.GetName(typeof(GameMode), info.CustomProperties["mode"]);
        }
        else
        {
            Debug.Log("no custom property");
        }
        playerCountText.text = _info.PlayerCount.ToString()+"/" +_info.MaxPlayers.ToString();
    }

    public void OnClick()
    {
        
        Launcher.instance.JoinRoom(info);
    }
}
