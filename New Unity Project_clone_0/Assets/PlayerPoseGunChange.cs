using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPoseGunChange : MonoBehaviour
{
    public static PlayerPoseGunChange Instance;
    [SerializeField] Transform playerPose;
    [SerializeField] GameObject[] playerPoseGuns;
    GunInfo itemInfo;
    Object[] guns;
    private void Awake()
    {
        if (Instance)
        {
            return;
        }
        Instance = this;
    }
    void Start()
    {
        guns=Resources.LoadAll("Items/Guns", typeof(GunInfo));
        InstantiateGun();
    }
    /*
    private void Update()
    {
        if (PlayerPrefs.HasKey("Guns"))
        {
            foreach (GameObject g in playerPoseGuns)
            {
                if (PlayerPrefs.GetString("Guns") == g.name)
                {
                    g.SetActive(true);
                }
                else
                {
                    g.SetActive(false);
                }
            }
            PlayerPrefs.GetString("Guns");
        }

    }
    */


    public void InstantiateGun()
    {
        foreach (Transform t in playerPose)
        {
            Destroy(t.gameObject);
        }
        if (PlayerPrefs.HasKey("Guns"))
            itemInfo = Resources.Load<GunInfo>("Items/Guns/" + PlayerPrefs.GetString("Guns"));
        else
            itemInfo = Resources.Load<GunInfo>("Items/Guns/AK47");
        GameObject gunM = Instantiate(itemInfo.itemModel,playerPose);
        gunM.transform.localPosition = itemInfo.playerPoseitemPosition; //.2f .35f .15f
        gunM.transform.localRotation = transform.localRotation * Quaternion.Euler(itemInfo.playerPoseitemRotation);// 0 90 0
       
        gunM.transform.localScale = itemInfo.playerPoseitemScale;
    }
}
