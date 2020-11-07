using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GunScript : MonoBehaviourPunCallbacks
{
    public float bodyDamage = 33f;
    public float headDamage = 100f;
    public float extremityDamage = 20f;
    public float range = 100f;
    public float fireRate = 10f;
    public float impactForce = 30f;
    public bool isReloading = false;
    public bool isSprinting = false;
    public bool isPistol = false;
    public LayerMask collisionLayer;
    
   


    public Animator anim;
    PlayerMovement movementScript;
    WeaponSwitching weaponSwitch;
    string currentState;

  

    public int maxAmmo = 10;
    private int currentAmmo;
    private int ammoUsed;
    public int totalAmmo = 200;
    public float reloadTime = 1f;

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    public float nextTimeToFire = 0f;
    public GameObject cameraParent;

    
    
    private void Start()
    {
        
            
            cameraParent.SetActive(photonView.IsMine);
            
            
        currentAmmo = maxAmmo;
        // anim = GetComponent<Animator>();
        movementScript = GetComponent<PlayerMovement>();
        weaponSwitch = GetComponent<WeaponSwitching>();
    }

  
    private void OnEnable()
    {
        isReloading = false;
        anim.SetBool("Reloading", false);
    }
   
    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (isReloading)
            return;



        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        //if not sprinting
        if (isSprinting == false)
        {
            if (Input.GetKey(KeyCode.R) && currentAmmo < maxAmmo && !Input.GetKey(KeyCode.LeftShift))
            {
                StartCoroutine(Reload());
                return;
            }
        }
    
        //if using AK
        if (isPistol == false)
        {
            anim.SetBool("Pistol", false);
           

            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && Input.GetKey(KeyCode.LeftShift) == false)
            {

                nextTimeToFire = Time.time + 1f / fireRate;
                //photonView.RPC("Shoot", RpcTarget.All);
                Shoot();
            }
        }
        //if using pistol
        if (isPistol)
        {
            
            anim.SetBool("Pistol", true);
            
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire && Input.GetKey(KeyCode.LeftShift) == false)
            {

                nextTimeToFire = Time.time + 1f / fireRate;
                //photonView.RPC("Shoot", RpcTarget.All);
                Shoot();
            }
        }



        Sprint();




    }
    
    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        anim.SetBool("Reloading", true);

        yield return new WaitForSeconds(reloadTime -.25f);

        anim.SetBool("Reloading", false);
        yield return new WaitForSeconds(.25f);

        ammoUsed =maxAmmo - currentAmmo;
        totalAmmo -= ammoUsed;
       
        currentAmmo = maxAmmo;
        
       
        
        isReloading = false;
    }

    void Sprint()
    {


        if (Input.GetKey(KeyCode.LeftShift))
        {

            anim.SetBool("Sprinting", true);
            isSprinting = true;

        }
        else
            anim.SetBool("Sprinting", false);
            if(anim.GetBool("Sprinting")==false)
            {
                isSprinting = false;
            }
            
    }
    [PunRPC]
    void Shoot()
    {
            
            muzzleFlash.Play();
        
            currentAmmo--;

            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {

                
                Target target = hit.collider.GetComponent<Target>();
                Debug.Log(hit.collider.name);
                if (photonView.IsMine)
                {
                    //shooting another player
                    if (hit.collider.gameObject.layer == 11)
                    {
                        //RPC to damage player
                        if (target != null)
                        {
                            /*
                            switch (target.damageType)
                            {
                                case Target.collisionType.head:
                                    target.TakeDamage(headDamage);
                                    Debug.Log("Hit Head");
                                    break;
                                case Target.collisionType.body:
                                    target.TakeDamage(bodyDamage);
                                    Debug.Log("Hit body");
                                    break;
                                case Target.collisionType.extremity:
                                    target.TakeDamage(extremityDamage);
                                    break;
                            }
                            */


                            target.TakeDamage(bodyDamage);
                        }
                    }
                
            }
        }
                if (hit.rigidbody != null)
                {
                    //hit.rigidbody.AddForce(-hit.normal * impactForce);
                    Debug.Log(hit.rigidbody.gameObject.name + "rigidbody");
                }

                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
            }
        }
    

