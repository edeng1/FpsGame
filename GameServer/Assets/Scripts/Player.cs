using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public CharacterController controller;
    public Animator animator;
    public string anim;
    public bool animState;
    //public Dictionary<string, bool> animStates = new Dictionary<string, bool>();
    public Transform shootOrigin;
    private float yVelocity = 0;
    public float gravity = -9.81f;
    public float health;
    public float maxhealth = 100f;

    public float normalSpeed = 8f;
    public float sprintSpeed = 12f;
    public float moveSpeed = 8f;
    public int ammo = 30;
    private bool[] inputs;

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;

    }
    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxhealth;
        anim = "Sprinting";
        animState = false;

        inputs = new bool[5];
        
    }
  
    public void FixedUpdate()
    {
        if (health <= 0f)
        {
            return;
        }
        Vector2 _inputDirection = Vector2.zero;
        if (inputs[0])
        {
            _inputDirection.y += 1;
        }
        if (inputs[1])
        {
            _inputDirection.y -= 1;
        }
        if (inputs[2])
        {
            _inputDirection.x -= 1;
        }
        if (inputs[3])
        {
            _inputDirection.x += 1;
        }
        
        Move(_inputDirection);
    }
  
    private void Move(Vector2 _inputDirection)
    {
        if (inputs[4])
        {
            moveSpeed = sprintSpeed;
            anim= "Sprinting";
            animState = true;
        }
        if (!inputs[4])
        {
            moveSpeed = normalSpeed;
            anim = "Sprinting";
            animState = false;
            //anim.SetBool("Sprinting", false);
        }
        Vector3 startPos = transform.position;

        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirection *= moveSpeed;
        Vector3 endPos = _moveDirection;
        
        if (controller.isGrounded)
        {
            yVelocity = 0f;
        }
        yVelocity += gravity;

        _moveDirection.y = yVelocity;
        controller.Move(_moveDirection);
        
        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
        ServerSend.PlayerAnimation(this);
    }

    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }

    public void Shoot(Vector3 _viewDirection)
    {
        if(Physics.Raycast(shootOrigin.position,_viewDirection, out RaycastHit _hit,25f))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                _hit.collider.GetComponent<Player>().TakeDamage(50f); 
            }
        }
    }

    public void TakeDamage(float _damage)
    {
        if (health <= 0f)
        {
            return;
        }

        health -= _damage;
        if (health <= 0f)
        {
            health = 0f;
            controller.enabled = false;
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }
        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        health = maxhealth;
        controller.enabled = true;
        ServerSend.PlayerRespawned(this);
    }

    
}
