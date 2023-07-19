using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CrosshairListItem : MonoBehaviour
{
    [SerializeField] Transform parentCanvas;
    [SerializeField] Transform crosshairOnPlayer;
    RectTransform myRectTransform;
    
    Texture crosshair;
    Sprite crosshair2;
    bool setup = false;
    public void SetUp(Texture2D texture)
    {
        if (texture != null)
        {
            crosshair = texture;
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            crosshair2= Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            gameObject.GetComponent<Image>().sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        }
        myRectTransform = GetComponent<RectTransform>();
        parentCanvas = transform.parent.parent.parent.parent.parent;//Canvas 
        crosshairOnPlayer = parentCanvas.GetChild(2).GetChild(3);//Crosshair
        
        setup = true;

    }
    public void OnClick()
    {
        if (setup&&crosshair!=null&&crosshairOnPlayer!=null)
        {
            crosshairOnPlayer.gameObject.GetComponent<RawImage>().texture = crosshair;
            //crosshairOnPlayer.gameObject.GetComponent<Image>().sprite = crosshair2;
            if (System.Int32.TryParse(crosshair.name, out int j))
            {
                PlayerPrefs.SetInt("Crosshair",j);
            }

           
        }
        
    }
}
