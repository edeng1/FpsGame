using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ButtonLean : MonoBehaviour
{
    public Vector3 position;
    public Vector3 curposition;
    bool done=true;
    RectTransform myRectTransform;
    private void Awake()
    {
        myRectTransform = GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (MenuManager.instance.isLoaded&&done==true)
            position = myRectTransform.localPosition;
        done = false;
        curposition = transform.position;
    }
    public void OnHighlight()
    {
        LeanTween.moveLocalX(gameObject, 40f, .2f);
        //LeanTween.move(gameObject.GetComponent<RectTransform>(), new Vector3(myRectTransform.localPosition.x-5,myRectTransform.localPosition.y), .2f);
    }
    public void OnHighlight2()
    {
        LeanTween.moveLocalX(gameObject, 500f, .2f);
    }
    public void OffHighlight()
    {
        
        LeanTween.move(gameObject.GetComponent<RectTransform>(), new Vector3(177.5f, myRectTransform.localPosition.y), .2f);
        
    }
    public void OffHighlight2()
    {

        LeanTween.move(gameObject.GetComponent<RectTransform>(),position, .2f);

    }
}
