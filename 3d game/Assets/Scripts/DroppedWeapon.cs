using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DroppedWeapon : MonoBehaviour
{
    WeaponSwapping swap;
  
    Dictionary<string, GameObject> weaponsToDropOnGround = new Dictionary<string, GameObject>();
    Dictionary<string, string> weaponNames = new Dictionary<string, string>();
    TextMeshProUGUI text;
    
    string name;
    bool nearWeapon=false;
    

    void Start()
    {
    
        
        
    }
    private void OnEnable()
    {
        weaponsToDropOnGround.Add("M4", (GameObject)Resources.Load("M4_Carbine 2", typeof(GameObject)));
        weaponsToDropOnGround.Add("AK", (GameObject)Resources.Load("AK-48", typeof(GameObject)));
        
        
        swap = FindObjectOfType<WeaponSwapping>();
        text = GameObject.Find("Pickup").GetComponent<TextMeshProUGUI>();
        
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Player")
        {
            

            nearWeapon = true;
            
            if (!gameObject.CompareTag(swap.transform.GetChild(0).tag)){
                swap.swappedWeapon = (GameObject)Resources.Load(WeaponSwapping.weapons[gameObject.tag], typeof(GameObject));
                text.text = $"Press F to pickup {gameObject.tag}";
            }
            
            
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            nearWeapon = false;
        }
        text.text = "";
    }

  
    void changeDroppedWeapon()
    {
        Transform t = transform;
        
        if (swap.getCurrentWeapon()!=swap.swappedWeapon)
        {
            GameObject gun= (GameObject)Instantiate(weaponsToDropOnGround[swap.getCurrentWeapon().tag],t.position,t.rotation);
            swap.swapWeapon();
            gameObject.name = gameObject.tag;
            Destroy(gameObject);
            
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
