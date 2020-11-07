using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerMovement : MonoBehaviourPunCallbacks
{
    public CharacterController controller;

    
    
 

    public float speed=8f;
    public float normalSpeed = 8f;
    public float sprintSpeed = 10f;
    public float gravity = -9.81f;
    public bool isSprinting = false;
    public Animator anim;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public GameObject playerModel;

    //protected Rigidbody rig;
    Vector3 velocity;
    bool isGrounded;
    
    void Start()
    {
        if (!photonView.IsMine)
        {
            gameObject.layer = 11;
            playerModel.SetActive(true);
        }
        else if(photonView.IsMine)
        {
            gameObject.layer = 9;
            playerModel.SetActive(false);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
        if (!photonView.IsMine)
        {
            return;
        }
        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded&&velocity.y<0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * x + transform.forward * z);
        if(move.magnitude>1)
        {
            move /= move.magnitude;
        }

        controller.Move(move*speed*Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

       
        anim.SetFloat("VelX", x);
        anim.SetFloat("VelY", z);
        
        

        Sprint();
    }
    void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = sprintSpeed;
        }
        else
        {
            speed = normalSpeed;
        }
       
    }
    
}
