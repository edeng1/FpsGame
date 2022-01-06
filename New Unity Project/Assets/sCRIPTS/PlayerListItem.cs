using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text levelText;
    public Player player;

    public void SetUp(Player _player)
    {
        player = _player;
        nameText.text = _player.NickName;
        levelText.text = RoomManager.playerData.level.ToString();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
       if(player==otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
