using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Photon.Pun;
public class MouseLook : MonoBehaviour //PunCallbacks
{
    public float mouseSensitivity = 4f;
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
        if (Cursor.visible == false)
        {


            float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            //mRecoil();
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); Quaternion.Euler(xRotation, transform.rotation.eulerAngles.y, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }

    }

   
   


}

