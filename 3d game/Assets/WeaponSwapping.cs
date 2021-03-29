using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwapping : MonoBehaviour
{
    public Transform currentWeapon;
    public GameObject swappedWeapon;
    public bool holdingAK=true;
    // Start is called before the first frame update
    void Start()
    {
        swappedWeapon = (GameObject)Resources.Load("M4", typeof(GameObject));
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            swapWeapon();
        }
        if (transform.GetChild(0).tag == "M4")
        {
            holdingAK = false;
        }
        else if (transform.GetChild(0).tag == "AK")
        {
            holdingAK = true;
        }
    }



    public void swapWeapon()
    {
        currentWeapon = transform.GetChild(0);
        Destroy(currentWeapon.gameObject);
        GameObject a=Instantiate(swappedWeapon, currentWeapon.position, currentWeapon.rotation,transform);
        a.transform.SetAsFirstSibling();
        

    }
    
}
