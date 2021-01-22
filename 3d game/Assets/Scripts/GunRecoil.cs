using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRecoil : MonoBehaviour
{

    public MouseLook playerCamera;
    public float verticalRecoil;
    public float duration;
    float time;
    // Start is called before the first frame update
    private void Awake()
    {
        playerCamera = FindObjectOfType<MouseLook>();
    }
    public void GenerateRecoil()
    {
         time = duration;
        
    }
    // Update is called once per frame
    void Update()
    {
        if(time>0)
        {
            playerCamera.xRotation -= (verticalRecoil*Time.deltaTime)/duration;
            time -= Time.deltaTime;
        }
    }
}
