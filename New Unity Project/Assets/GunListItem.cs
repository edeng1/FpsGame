using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class GunListItem : MonoBehaviour
{


    [SerializeField] TMP_Text text;
    GunInfo _info;
    public void SetUp(GunInfo info)
    {
        _info = info;
        //text.text = info.itemName;
        Texture2D texture = info.gunImage;
        if (texture != null)
        {
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            gameObject.GetComponent<Image>().sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f)); ;
        }
       
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
