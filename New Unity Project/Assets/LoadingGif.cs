using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingGif : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(gif());
    }
  
    IEnumerator gif()
    {
        gameObject.GetComponent<TMP_Text>().text = "Loading";
        yield return new WaitForSeconds(.5f);
        gameObject.GetComponent<TMP_Text>().text = "Loading.";
        yield return new WaitForSeconds(.5f);
        gameObject.GetComponent<TMP_Text>().text = "Loading..";
        yield return new WaitForSeconds(.5f);
        gameObject.GetComponent<TMP_Text>().text = "Loading...";
        yield return new WaitForSeconds(.5f);

        StartCoroutine(gif());
    }
  
}
