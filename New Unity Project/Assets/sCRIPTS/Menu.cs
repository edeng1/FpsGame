using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    public string menuName;
    public bool open;
    public Camera mainCam;
   


    public void Open()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {


            if (gameObject.name == "RoomMenu" || gameObject.name == "FindRoomMenu")
            {
                ZoomIn();

            }
            if (gameObject.name == "TitleMenu")
            {
                ZoomOut();

            }

            open = true;

            gameObject.SetActive(true);
        }
    }
    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
    }

    public void ZoomIn()
    {
        Vector3 temp = transform.localScale;
        LeanTween.moveZ(mainCam.gameObject, 19.5f, .5f);
        temp = transform.localScale;
        transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), .5f);
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
