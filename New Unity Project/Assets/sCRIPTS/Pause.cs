using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
public class Pause : MonoBehaviour
{
    public bool paused = false;
    private bool disconnecting = false;
    public Slider volSlider;
    public Slider sensSlider;
    public TMP_InputField sensInputField;
    public TMP_InputField volInputField;
    public GameObject settings;
    public GameObject loadout;
    public AudioSource audioSrc;
    float sensitivity;
    private void Start()
    {
        if (PlayerPrefs.HasKey("sens")) {
            Debug.Log(PlayerPrefs.GetFloat("sens"));
            sensitivity= PlayerPrefs.GetFloat("sens");
            sensSlider.value = sensitivity;
            
        }
        if (PlayerPrefs.HasKey("vol"))
        {
            
            volSlider.value = PlayerPrefs.GetFloat("vol");
            if (audioSrc != null)
            {
                audioSrc.volume = PlayerPrefs.GetFloat("vol");
            }

        }

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
        if (loadout.activeSelf)
        {
            ToggleLoadout();
        }

    }
    public void ToggleSettings()
    {
        
        if (!settings.activeSelf&&paused)
        {
            settings.SetActive(true);
            loadout.SetActive(false);
        }
        else { settings.SetActive(false); }
    }
    public void ToggleLoadout()
    {
        if (!loadout.activeSelf && paused)
        {
            loadout.SetActive(true);
            settings.SetActive(false);
        }
        else { loadout.SetActive(false); }
    }

    public void InputChangeSens()
    {
        float result=0;
        float.TryParse(sensInputField.text, out result);
        Debug.Log(result);
        
        if (result < 0) { result = 0; sensInputField.text ="0"; }
        if (result > 1) { result = 1; sensInputField.text = "1"; }
        ChangeSensitivity(result);

    }
    public void InputChangeVol()
    {
        float result = 0;
        float.TryParse(volInputField.text, out result);
        Debug.Log(result);

        if (result < 0) { result = 0; volInputField.text = "0"; }
        if (result > 1) { result = 1; volInputField.text = "1"; }
        ChangeVolume(result);

    }
    public void ChangeVolume(float vol)
    {
        PlayerPrefs.SetFloat("vol", vol);
        PlayerPrefs.Save();
        volSlider.value = vol;
        if (audioSrc != null)
            audioSrc.volume = vol;
        if (volInputField != null)
            volInputField.text = vol.ToString();

    }

    public void ChangeSensitivity(float sens)
    {
        
        PlayerPrefs.SetFloat("sens", sens);
        PlayerPrefs.Save();
        sensSlider.value= sens;
        if(sensInputField!=null)
            sensInputField.text = sens.ToString();
        Debug.Log(PlayerPrefs.GetFloat("sens"));
       
    }

    public void Quit()
    {
        disconnecting = true;
        //RoomManager.Instance.destroyPM();
        Manager.Instance.PlayerLeft_S(PhotonNetwork.LocalPlayer.ActorNumber);
        SceneManager.LoadScene(0);
        if (PhotonNetwork.IsMasterClient) { PhotonNetwork.DestroyAll(); }
        if (!PhotonNetwork.IsMasterClient) // when master client quits, the other clients will return to the same menu.
        {
            
            PhotonNetwork.LeaveRoom();
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
            
        }

    }

    
}
