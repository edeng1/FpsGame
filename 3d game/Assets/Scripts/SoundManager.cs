using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static AudioClip fireSound;
    public static AudioClip fireSoundEnemy;
    static AudioSource audioSrc;

    private void Awake()
    {
        
    }
    // Start is called beforfiree the first frame update
    void Start()
    {
        fireSound = Resources.Load<AudioClip>("aksoundLow");
        fireSoundEnemy = Resources.Load<AudioClip>("FireSound");
        audioSrc = GetComponent<AudioSource>();
        
        ChangeVolume();
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "fire":
                
                audioSrc.PlayOneShot(fireSound);
                
                
                break;
            case "enemyFire":
                audioSrc.PlayOneShot(fireSoundEnemy);
                break;
        }
        
    }
    public static void MuteVolume()
    {
        audioSrc.volume = 0;
    }
    public static void StopSound()
    {

    }
    public void ChangeVolume()
    {


        //audioSrc.volume = PlayerPrefs.GetFloat("Volume");
        //PlayerPrefs.Save();
        
        Debug.Log(PlayerPrefs.GetFloat("Volume") / 100 + " /100 Volume");
        
        audioSrc.volume = PlayerPrefs.GetFloat("Volume") / 100;
        //audioSrc.volume = 1f;



    }
    

}
