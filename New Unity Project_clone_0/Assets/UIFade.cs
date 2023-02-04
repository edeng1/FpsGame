using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Image>().CrossFadeAlpha(0.1f, 2.0f, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
