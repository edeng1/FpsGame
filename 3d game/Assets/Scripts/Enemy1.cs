using System.Collections;
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
        variablePrefab.name = "M4";
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
        if (rag != null)
        {
           rag.ToggleDead();
            Destroy(gameObject, 5f);
            //Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        Invoke("instantiateWeapon", 3f);
        
    }
    public void instantiateWeapon()
    {
        Transform v=null;
        if (gameObject.transform.GetChild(1).childCount > 3)
        {
            v = gameObject.transform.GetChild(1).GetChild(3).transform;


            GameObject m4 = (GameObject)Instantiate(variablePrefab, v.position, v.rotation);
            m4.name = "M4";
        }
    }
   
    
}
