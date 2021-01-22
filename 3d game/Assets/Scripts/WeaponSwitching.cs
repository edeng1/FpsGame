using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Photon.Pun;
public class WeaponSwitching : MonoBehaviour //PunCallbacks
{
    public int selectedWeapon = 0;
     public Animator anim;
    

    // Start is called before the first frame update
    void Start()
    {
        //if (!photonView.IsMine) return;

        //photonView.RPC("SelectWeapon", RpcTarget.All);
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {

        //if (!photonView.IsMine) return;
        int previousSelectedWeapon = selectedWeapon;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            
            selectedWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)&&transform.childCount>=2)
        {
            
            selectedWeapon = 1;
        }
        if(previousSelectedWeapon!=selectedWeapon)
        {
            //photonView.RPC("SelectWeapon", RpcTarget.All);
            SelectWeapon();
        }
    }
    //[PunRPC]
    void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
   
    }

   

}
