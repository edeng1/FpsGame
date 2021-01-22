using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    private RagdollDuplicate rag;

    //private Spawner spawn;


    Manager man;




    public float HP = 50f;
   
    private void Awake()
    {
        
        rag = GetComponent<RagdollDuplicate>();
        if (Manager.FindObjectOfType<Manager>())
        {
           man = Manager.FindObjectOfType<Manager>();
        }
        
        
    }

    public void Die()
    {
        if (man!=null)
        {
            man.addKill();
        }
        
        rag.ToggleDead();

        Destroy(gameObject,5f);
    }
   
    
}
