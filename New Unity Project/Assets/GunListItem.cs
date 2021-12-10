using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GunListItem : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    GunInfo _info;
    public void SetUp(GunInfo info)
    {
        _info = info;
        text.text = info.itemName;
        if (!PlayerPrefs.HasKey("Guns"))
        {
            PlayerPrefs.SetString("Guns","Ak47");
        }



    }

    public void OnClick()
    {

        if (PlayerPrefs.HasKey("Guns"))
        {
            PlayerPrefs.SetString("Guns", _info.itemName);
            Debug.Log(PlayerPrefs.GetString("Guns"));
        }

    }
}
