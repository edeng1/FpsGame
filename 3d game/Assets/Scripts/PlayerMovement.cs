using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Photon.Pun;
public class PlayerMovement : MonoBehaviour//PunCallbacks
{
    public CharacterController controller;


    public float maxHP=100;
    public float currentHP;
    public float pointIncreasePerSecond = 5f;
    //public Text HealthUI;
    public MouseLook mouse;


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
    private void Awake()
    {
        mouse = GetComponent<MouseLook>();
        
    }
    void Start()
    {
        currentHP = maxHP;
        
        /*See your arms but not your model, see enemy model but not their arms
         * 
         * if (!photonView.IsMine)
        {
            gameObject.layer = 11;
            playerModel.SetActive(true);
        }
        else if(photonView.IsMine)
        {
            gameObject.layer = 9;
            playerModel.SetActive(false);
        }
        */

    }

    // Update is called once per frame
    void Update()
    {
        if (currentHP > 0)
        {
            currentHP += pointIncreasePerSecond * Time.deltaTime;
        }
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        if (currentHP < 0)
        {
            currentHP = 0;
        }
        /*
        HealthUI.text = (int)currentHP + "HP";
        if (currentHP < 40)
        {
            HealthUI.color = Color.red;
        }
        if (currentHP >= 40)
        {
            HealthUI.color = Color.green;
        }
        */
        //if (!photonView.IsMine) return;
    }
    void FixedUpdate()
    {
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

        if (anim != null)
        {
            anim.SetFloat("VelX", x);
            anim.SetFloat("VelY", z);
        }
        
        
        

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
    public void Die()
    {
        print("I Died");
    }
    public void AdjustSensitivity(float newSens)
    {
        mouse.mouseSensitivity = newSens;
        PlayerPrefs.SetFloat("Sensitivity", newSens);
        PlayerPrefs.Save();
    }
}


public interface ITargetInterface
{
    void TakeDamage(float amount);
}