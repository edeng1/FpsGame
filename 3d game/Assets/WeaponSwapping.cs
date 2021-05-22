using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwapping : MonoBehaviour
{
    public Transform currentWeapon;
    public GameObject swappedWeapon;
    public bool holdingAK=true;
    public static Dictionary<string, string> weapons=new Dictionary<string, string>();
    // Start is called before the first frame update
    void Start()
    {
        weapons["M4"] = "M4";
        weapons["AK"] = "AK 1";
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
        currentWeapon = transform.GetChild(0);
    }



    public void swapWeapon()
    {
        
        //GameObject temp = currentWeapon.gameObject;
        Destroy(currentWeapon.gameObject);
        GameObject a=Instantiate(swappedWeapon, currentWeapon.position, currentWeapon.rotation,transform);
        a.transform.SetAsFirstSibling();
        //swappedWeapon = temp;

    }
    public Transform getCurrentWeapon()
    {
        return currentWeapon;
    }
    
}
