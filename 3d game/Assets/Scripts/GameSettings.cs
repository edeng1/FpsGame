using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameSettings : MonoBehaviour
{

    public Slider sensitivitySlider;
    public Slider audioSlider;

    private void Awake()
    {
        //PlayerPrefs.SetFloat("Volume", 1f);
    }
    void Start()
    {
        if(PlayerPrefs.GetFloat("FirstPlay")==0)
        {
            PlayerPrefs.SetFloat("Sensitivity",50f);
            PlayerPrefs.SetFloat("Volume", 50f);
            PlayerPrefs.SetFloat("FirstPlay",-1);
        }
        
        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        audioSlider.value= PlayerPrefs.GetFloat("Volume");
        PlayerPrefs.Save();
    }
   


    public void UpdateVolume()
    {
        //float vol = volume;
        PlayerPrefs.SetFloat("Volume", audioSlider.value);
        PlayerPrefs.Save();
    }

}
