using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedWeapon : MonoBehaviour
{
    WeaponSwapping swap;
    GameObject akPrefab;
    GameObject m4Prefab;
    bool nearWeapon=false;
    

    void Start()
    {
    
        
        
    }
    private void OnEnable()
    {
        akPrefab = (GameObject)Resources.Load("AK-48", typeof(GameObject));
        m4Prefab= (GameObject)Resources.Load("M4_Carbine 2", typeof(GameObject));
        swap = FindObjectOfType<WeaponSwapping>();
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Player")
        {
            /*Transform t = other.transform.parent;
            for (int i=0; i < t.childCount; i++)
            {
                if (t.GetChild(i).GetComponent<WeaponSwapping>())
                {
                    swap = t.GetChild(i).GetComponent<WeaponSwapping>();
                }
            }*/

            nearWeapon = true;
            
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            nearWeapon = false;
        }
    }

  
    void changeDroppedWeapon()
    {
        Transform t = transform;
        if (swap.holdingAK && gameObject.tag == "M4")
        {
            swap.swapWeapon();
            Instantiate(akPrefab, t.position, t.rotation);
            Destroy(gameObject);
            
            swap.swappedWeapon= (GameObject)Resources.Load("AK 1", typeof(GameObject));
        }
        else if (!swap.holdingAK && gameObject.tag == "AK")
        {
            swap.swapWeapon();    
            Instantiate(m4Prefab, t.position, t.rotation);
            Destroy(gameObject);
            
            swap.swappedWeapon = (GameObject)Resources.Load("M4", typeof(GameObject));
        }

        
    }
   
    void Update()
    {
        if (nearWeapon == true)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                
                changeDroppedWeapon();
            }
        }
    }
}
