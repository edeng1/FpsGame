using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaputrePointFlag : MonoBehaviour
{
    public bool awayFlag;
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.CompareTag("Away"))
        {
            awayFlag = true;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Flag>())
        {
            if (!other.CompareTag(gameObject.tag))
            {
                other.GetComponent<Flag>().ScoreFlag();
            }
            
        }
    }
    

}
