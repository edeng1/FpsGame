using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    public bool isLoaded;
    GameObject prev;
    
    [SerializeField]public Menu[] menus;

    private void Awake()
    {
        instance = this;
        
        isLoaded = true;
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
                prev = gO;
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


}
