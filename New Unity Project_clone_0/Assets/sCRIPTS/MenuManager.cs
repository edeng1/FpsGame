using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    public bool isLoaded;
    GameObject prev;
    public Camera mainCam;

    [SerializeField]public Menu[] menus;

    private void Awake()
    {
        instance = this;
        
        
    }
    public void OpenMenu(string menuName)
    {/*
        foreach(Menu m in menus)
        {
            if (m.menuName == menuName)
            {
                m.Open();
            }
            else if (m.open)
            {
                CloseMenu(m);
            }
        }*/

        for(int i=0;i<transform.childCount;i++)
        {
            var gO = transform.GetChild(i).gameObject;
            if (gO.name == menuName + "Menu")
            {
                if (gO.name != "LoadingMenu")
                {
                    //isLoaded=ZoomIn(gO);
                }
               
               
                if (gO.name == "RoomMenu" || gO.name == "FindRoomMenu")
                {
                    //ZoomIn(gO);

                }
                if (gO.name == "TitleMenu")
                {
                    //LeanTween.moveZ(mainCam.gameObject, 16f, .4f);
                }
                gO.SetActive(true);
            }
            if(gO.name!=menuName+"Menu")
            {
                
                
                gO.SetActive(false);
            }
        }
       
       
    }

    public void OpenMenu(Menu menu)
    {
        foreach (Menu m in menus)
        {
            if (m.open)
            {
                CloseMenu(m);
            }
        }
        menu.Open();
    }
    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
    public void CloseMenu(string menuName)
    {

    }
    public bool ZoomIn(GameObject menu)
    {
        Vector3 temp = menu.transform.localScale;
        LeanTween.moveZ(mainCam.gameObject, 19.5f, .4f);
        temp = menu.transform.localScale;
        menu.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(menu, new Vector3(1, 1, 1), .4f);
        return true;
    }
    public void ZoomOut()
    {

        if (mainCam != null)
        {
            //LeanTween.moveZ(Launcher.instance.mainCamera.gameObject, 16f, .5f);
            LeanTween.moveZ(mainCam.gameObject, 16f, .5f);
        }


    }


}
