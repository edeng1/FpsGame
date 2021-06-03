using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
   public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server:{_msg}");
        Client1.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        Client1.instance.udp.Connect(((IPEndPoint)Client1.instance.tcp.socket.Client.LocalEndPoint).Port);

    }

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        MultiplayerManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
    }

    public static void PlayerPosition(Packet _packet)
    {
        
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        string _anim = _packet.ReadString();
        bool _animState = _packet.ReadBool();
        //MultiplayerManager.players[_id].transform.position=_position;
        MultiplayerManager.players[_id].SetPosition(_position,_anim,_animState);  
       
    }
    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();
        MultiplayerManager.players[_id].transform.rotation = _rotation;
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Destroy(MultiplayerManager.players[_id].gameObject);
        MultiplayerManager.players.Remove(_id);
    }
    public static void PlayerHealth(Packet _packet)
    {
        int _id = _packet.ReadInt();

        float _health = _packet.ReadFloat();

        MultiplayerManager.players[_id].SetHealth(_health);
        

    }
    public static void PlayerRespawned(Packet _packet)
    {
        int _id = _packet.ReadInt();
        MultiplayerManager.players[_id].Respawn();


    }

    public static void PlayerAnimation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _anim = _packet.ReadString();
        bool _animState = _packet.ReadBool();
        MultiplayerManager.players[_id].SetAnimation(_anim, _animState);
        
    }
}
