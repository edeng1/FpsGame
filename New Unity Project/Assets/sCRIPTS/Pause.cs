using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Pause : MonoBehaviour
{
    public bool paused = false;
    private bool disconnecting = false;
    public Slider volSlider;
    public Slider sensSlider;
    public GameObject settings;

    private void Start()
    {
        if (PlayerPrefs.HasKey("sens")) { sensSlider.value = PlayerPrefs.GetFloat("sens"); }
        Debug.Log(PlayerPrefs.GetFloat("sens"));
        
    }
    public void TogglePause()
    {
        if (disconnecting) return;

        paused = !paused;

        transform.GetChild(0).gameObject.SetActive(paused);

        Cursor.lockState=(paused)?CursorLockMode.None:CursorLockMode.Locked;
        Cursor.visible = paused;
        if (settings.activeSelf)
        {
            ToggleSettings();
        }

    }
    public void ToggleSettings()
    {
        
        if (!settings.activeSelf&&paused)
        {
            settings.SetActive(true);
        }
        else { settings.SetActive(false); }
    }

    public void ChangeSensitivity(float sens)
    {
        PlayerPrefs.SetFloat("sens", sens);
    }

    public void Quit()
    {
        disconnecting = true;
        //RoomManager.Instance.destroyPM();
        Manager.Instance.PlayerLeft_S(PhotonNetwork.LocalPlayer.ActorNumber);
        SceneManager.LoadScene(0);
        if (!PhotonNetwork.IsMasterClient) // when master client quits, the other clients will return to the same menu.
        {
            PhotonNetwork.LeaveRoom();
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
            
        }

    }

    
}
