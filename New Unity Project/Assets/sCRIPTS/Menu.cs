using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    public bool open;
    
    public void Open()
    {
        
        if (menuName == "Room"|| menuName=="Find")
        {
            ZoomIn();
        }
       if(menuName=="Title")
        {
            ZoomOut();
        }
        
        open = true;
        
        gameObject.SetActive(true);
    }
    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
    }

    public void ZoomIn()
    {
        Vector3 temp = transform.localScale;
        LeanTween.moveZ(Launcher.instance.mainCamera.gameObject, 19.5f, .5f);
        temp = transform.localScale;
        transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), .5f);
    }
    public void ZoomOut()
    {
        
        LeanTween.moveZ(Launcher.instance.mainCamera.gameObject, 16f, .5f);
       
    }
}
