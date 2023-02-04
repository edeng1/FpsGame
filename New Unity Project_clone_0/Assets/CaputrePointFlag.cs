using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaputrePointFlag : MonoBehaviour
{
    public bool awayFlag;
    PhotonView PV;
    bool isColliding=false;
    // Start is called before the first frame update
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    void Start()
    {
        if (gameObject.CompareTag("Away"))
        {
            awayFlag = true;
        }
        else { awayFlag = false; }
        
    }

    private void OnTriggerEnter(Collider other)
    {

        
            if (other.GetComponentInChildren<Flag>())
            {
                if (isColliding) return;
                isColliding = true;
            Debug.Log(other);
                if (!other.GetComponentInChildren<Flag>().CompareTag(gameObject.tag))
                {
                    
                        other.GetComponentInChildren<Flag>().ScoreFlag();
                    
                    
                }

            }
        
    }
    private void Update()
    {
        isColliding = false;
    }


}
