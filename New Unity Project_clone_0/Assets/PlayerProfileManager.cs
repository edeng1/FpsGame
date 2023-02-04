using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerProfileManager : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInput;

    public void OnUsernameInputValueChanged()
    {
        PhotonNetwork.NickName = usernameInput.ToString();
    }
}
