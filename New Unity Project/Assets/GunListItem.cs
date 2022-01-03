using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GunListItem : MonoBehaviour
{


    [SerializeField] TMP_Text text;
    GunInfo _info;
    [SerializeField] GameObject customize;
    [SerializeField] TMP_Text customizeText;
    RectTransform myRectTransform;
    RectTransform customizeRectTransform;
  
    public void SetUp(GunInfo info)
    {
       
        customizeRectTransform = customize.GetComponent<RectTransform>();
        _info = info;
        //text.text = info.itemName;
        Texture2D texture = info.gunImage;
        if (texture != null)
        {
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            gameObject.GetComponent<Image>().sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
           
        }
        
        if (!PlayerPrefs.HasKey("Guns"))
        {
            PlayerPrefs.SetString("Guns","Ak47");
        }
        //Sets the customize text when choosing loadout to be same size as picture and right below it. Makes text as the name of the skin of the gun.
        myRectTransform = GetComponent<RectTransform>();
        customizeText.text = _info.itemModel.transform.Find("Skin").GetChild(0).gameObject.name;//gets the skin of the gun.
        if (SceneManager.GetActiveScene().buildIndex == 0) {//if in menu or game scene
            customizeRectTransform.sizeDelta = new Vector2(myRectTransform.sizeDelta.x, myRectTransform.sizeDelta.y / 4);
            customize.transform.localPosition = new Vector3(0, -50);
        }
            
        else
        {
            customizeRectTransform.sizeDelta = new Vector2(myRectTransform.sizeDelta.x / 2, myRectTransform.sizeDelta.y / 8);
            customizeText.fontSize = 10;
            customize.transform.localPosition = new Vector3(0, -30);
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
    public void OnClickCustomize()
    {
        if(_info.itemModel == _info.itemModels[1])
            _info.itemModel = _info.itemModels[0];
        else
        {
            _info.itemModel = _info.itemModels[1];
        }
        customizeText.text = _info.itemModel.transform.Find("Skin").GetChild(0).gameObject.name;

    }
}
