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
    public Toggle PostProcessing;
    public GameObject PostProcessingMainMenu;
    public GameObject crosshairs;
    public AudioSource[] audioSrc;
    float sensitivity;
    float volume;
    float defaultSens = 1f;
    float defaultVol = .1f;
    float maxSens = 10f;
    float maxVol = 2f;
    private void Start()
    {
        
        if (PlayerPrefs.HasKey("sens")) {
            Debug.Log(PlayerPrefs.GetFloat("sens"));
            sensitivity= PlayerPrefs.GetFloat("sens");
            
            
        }
        else
        {
            sensitivity = defaultSens;
            
            sensInputField.text = sensitivity.ToString();
        }
        sensSlider.value = sensitivity;

        if (PlayerPrefs.HasKey("vol"))
        {
            volume = PlayerPrefs.GetFloat("vol");
            
            if (audioSrc != null)
            {
                foreach (AudioSource a in audioSrc)
                {
                    a.volume = PlayerPrefs.GetFloat("vol");
                }
            }

        }
        else
        {
            volume = defaultVol;
            
            volInputField.text = volume.ToString();
            if (audioSrc != null)
            {
                foreach (AudioSource a in audioSrc)
                {
                    a.volume = defaultVol;
                }
            }

        }
        volSlider.value = volume;

        if (PlayerPrefs.HasKey("postp"))
        {
            int pp = PlayerPrefs.GetInt("postp");
            if (pp == 0)
            {
                PostProcessing.isOn = false;
                TogglePostProcessing(false);
            }
            else if(pp==1)
            {
                PostProcessing.isOn = true;
                TogglePostProcessing(true);
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
        if (crosshairs.activeSelf)
        {
            ToggleCrosshairs();
        }

    }
    public void ToggleSettings()
    {
        
        if (!settings.activeSelf&&paused)
        {
            settings.SetActive(true);
            loadout.SetActive(false);
            crosshairs.SetActive(false);
        }
        else { settings.SetActive(false); }
    }
    public void ToggleLoadout()
    {
        if (!loadout.activeSelf && paused)
        {
            loadout.SetActive(true);
            settings.SetActive(false);
            crosshairs.SetActive(false);
        }
        else { loadout.SetActive(false); }
    }
    public void ToggleCrosshairs()
    {

        if (!crosshairs.activeSelf && paused)
        {
            crosshairs.SetActive(true);
            loadout.SetActive(false);
            settings.SetActive(false);
        }
        else { crosshairs.SetActive(false); }
    }

    public void InputChangeSens()
    {
        float result=0;
        float.TryParse(sensInputField.text, out result);
        Debug.Log(result);
        
        if (result < 0) { result = 0; sensInputField.text ="0"; }
        if (result > maxSens) { result = maxSens; sensInputField.text = maxSens.ToString(); }
        ChangeSensitivity(result);

    }
    public void InputChangeVol()
    {
        float result = 0;
        float.TryParse(volInputField.text, out result);
        Debug.Log(result);

        if (result < 0) { result = 0; volInputField.text = "0"; }
        if (result > maxVol) { result = maxVol; volInputField.text = maxVol.ToString(); }
        ChangeVolume(result);

    }
    public void ChangeVolume(float vol)
    {
        PlayerPrefs.SetFloat("vol", vol);
        PlayerPrefs.Save();
        volSlider.value = vol;
        if (audioSrc != null)
            foreach(AudioSource a in audioSrc)
            {
                a.volume = vol;
            }
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
        Manager.Instance.PlayerLeft_S(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.LocalPlayer.NickName);
        SceneManager.LoadScene(0);
        if (PhotonNetwork.IsMasterClient) { PhotonNetwork.DestroyAll(); }
        if (!PhotonNetwork.IsMasterClient) // when master client quits, the other clients will return to the same menu.
        {
            
            PhotonNetwork.LeaveRoom();
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
            
        }

    }

    public void TogglePostProcessing(bool toggle)
    {
        Debug.Log("PP called" + toggle);
        if(Manager.Instance)
            Manager.Instance.TogglePostProcessing(toggle);
        else
        {
            if (PostProcessingMainMenu != null)
            {
                PostProcessingMainMenu.SetActive(toggle);
            }
        }

        int t = 0;

        if (toggle) { t = 1; }
        PlayerPrefs.SetInt("postp", t);
    }

    
}
