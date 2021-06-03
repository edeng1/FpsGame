using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    private Vector3 fromPos = Vector3.zero;
    private Vector3 toPos = Vector3.zero;
    private float lastTime;
    public float health;
    public float maxHealth;
    public SkinnedMeshRenderer model;
    public Animator animator;
    string anim="f";
    bool animState;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
    }

    public void SetHealth(float _health)
    {
        health = _health;

        if(health<=0f)
        {
            Die();
        }
    }

    public void Die()
    {
        model.enabled = false;
    }

    public void Respawn()
    {
        model.enabled = true;
        SetHealth(maxHealth);
        
    }


    public void SetPosition(Vector3 position, string _anim, bool _animState)
    {
        fromPos = toPos;
        toPos = position;
        lastTime = Time.time;
        anim = _anim;
        animator.SetBool(_anim, _animState);
    }
    
    public void SetAnimation(string _anim, bool _animState)
    {
        animator.SetBool(_anim, _animState);
        anim = _anim;
        animState = _animState;

    }
    

    private void Update()
    {
       
        Debug.Log((Time.time - lastTime)/ (1f / 30f) + " LerpFactor");
        this.transform.position = Vector3.Lerp(fromPos, toPos, (Time.time - lastTime) /(1f/30f));//(1.0f/TICKS_PER_SEC);
        Debug.Log(anim + " " + animState);

    }
}

