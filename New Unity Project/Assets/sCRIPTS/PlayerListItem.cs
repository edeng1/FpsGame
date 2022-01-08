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
    PhotonView PV;
    public void SetUp(Player _player)
    {
        PV = GetComponent<PhotonView>();
        player = _player;
        nameText.text = _player.NickName;
        if(_player.CustomProperties.ContainsKey("level"))
            levelText.text = _player.CustomProperties["level"].ToString();
        
    }
    private void Update()
    {
        if (player != null)
        {
            if (player.CustomProperties.ContainsKey("level"))
                levelText.text = player.CustomProperties["level"].ToString();
        }
       
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
