﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Photon.Pun;
public class MouseLook : MonoBehaviour //PunCallbacks
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public static bool cursorIsLocked = true;
    
    public float recoil=1f;
    public float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible=false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (!photonView.IsMine) return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        //mRecoil();
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation=Quaternion.Euler(xRotation, 0f, 0f); Quaternion.Euler(xRotation, transform.rotation.eulerAngles.y, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

    }

   
   


}

