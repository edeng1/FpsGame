using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    private RagdollDuplicate rag;
   
    private Spawner spawn;
    
    
   

   
    
    public float hp = 50f;
   
    private void Awake()
    {
        
        rag = GetComponent<RagdollDuplicate>();
        
        
        
    }

    public void Die()
    {
        rag.ToggleDead();

        Destroy(gameObject,5f);
    }
   
    
}
