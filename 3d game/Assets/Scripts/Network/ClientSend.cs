using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client1.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client1.instance.udp.SendData(_packet);
    }
    public static void WelcomeReceived()
    {
        using (Packet _packet= new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client1.instance.myId);
            _packet.Write(MultiplayerMenu.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }

    public static void PlayerMovement(bool[] _inputs)
    {
        using (Packet _packet= new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }
            _packet.Write(MultiplayerManager.players[Client1.instance.myId].transform.rotation);

            SendUDPData(_packet);
        }
    }
}
