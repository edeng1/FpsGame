using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingGif : MonoBehaviour
{
    TMP_Text loading;
    void OnEnable()
    {
        loading = gameObject.GetComponent<TMP_Text>();
        StartCoroutine(gif());
    }
  
    IEnumerator gif()
    {
       loading.text = "Loading";
        yield return new WaitForSeconds(.5f);
        for(int i = 0; i < 3; i++)
        {
            gameObject.GetComponent<TMP_Text>().text += ".";
            yield return new WaitForSeconds(.5f);
        }

        StartCoroutine(gif());
    }
  
}
