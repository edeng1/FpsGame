﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    private RagdollDuplicate rag;
    GameObject variablePrefab;

    //private Spawner spawn;


    Manager man;




    public float HP = 50f;
   
    private void Awake()
    {
        variablePrefab = (GameObject)Resources.Load("M4_Carbine 2", typeof(GameObject));
        rag = GetComponent<RagdollDuplicate>();
        if (Manager.FindObjectOfType<Manager>())
        {
           man = Manager.FindObjectOfType<Manager>();
        }
        
        
    }
    public void FindGun()
    {

    }

    public void Die()
    {
        if (man!=null)
        {
            man.addKill();
        }
        
        rag.ToggleDead();
        
        
        Destroy(gameObject,5f);
        Invoke("instantiateWeapon", 3f);
        
    }
    public void instantiateWeapon()
    {
        Transform v = gameObject.transform.GetChild(1).GetChild(3).transform;
        Instantiate(variablePrefab, v.position, v.rotation);
    }
   
    
}
