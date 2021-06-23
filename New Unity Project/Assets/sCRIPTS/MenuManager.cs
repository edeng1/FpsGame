using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField]Menu[] menus;

    public void OpenMenu(string menuName)
    {
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


}
