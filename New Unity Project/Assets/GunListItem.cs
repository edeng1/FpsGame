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
    Steamworks.InventoryDef[] itemDefs;
    [SerializeField] int index = -1;//default skin

    public void SetUp(GunInfo info)
    {
        index = -1;
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
        if (_info.itemModel == null) { _info.itemModel = _info.itemModelsUnlocked[0]; }
        try
        {
            if (_info.itemModel != _info.itemModelsUnlocked[0])//if current skin is one of the bought skins, index will start on that skin.
            {
                for (int i = 0; i < itemDefs.Length; i++)
                {
                    if (itemDefs[i].Description + itemDefs[i].Id == _info.itemModel.gameObject.name)
                    {
                        index = i;
                    }
                }
            }
        }
        catch { }
        
        if(_info.itemModel.transform.Find("Skin")!=null)
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


        try
        {
            itemDefs = Steamworks.SteamInventory.Definitions;
        }
        catch
        {

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
        Debug.Log(itemDefs.Length + " Length");
        if (itemDefs.Length==0){ return; }
        
       
        while (index < itemDefs.Length) {
            index++;
            
            if (index >= itemDefs.Length)
            {
                index = -1;
                _info.itemModel = _info.itemModelsUnlocked[0];//default skin
                break;
            }
            if (itemDefs[index].Description.ToString().Contains(_info.name.ToString()))
            {
                Debug.Log(itemDefs[1].Description.ToString());
                _info.itemModel = Resources.Load<GameObject>("Skins/" + itemDefs[index].Description + itemDefs[index].Id);
               
                break;
            }
            
            

        }



        Debug.Log(index);
        customizeText.text = _info.itemModel.transform.Find("Skin").GetChild(0).gameObject.name;
        
    }
}
