using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MultiplayerMenu : MonoBehaviour
{
    public static MultiplayerMenu instance;

    public GameObject startMenu;
    public TMP_InputField usernameField;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
        }

    }
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        usernameField.interactable = false;
        Client1.instance.ConnectToServer();
    }

}
