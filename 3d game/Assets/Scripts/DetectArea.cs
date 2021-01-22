using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectArea : MonoBehaviour
{

    bool detected;
    public GameObject target;
    public Transform enemy;
    public ParticleSystem muzzleFlash;
    public Transform shootPoint;
    public float range = 100f;
    public float bodyDamage = 15f;
    public GameObject impactEffect;
    public float nextTimeToFire = 0f;
    public float impactForce = 30f;
    public float fireRate = 1f;
    public float attackRange = 15f;
    public bool playerInAttackRange;
    public float shootDelayOnSight=.3f;
    Vector3 v;
    public float spreadFactor = 0;


    // Start is called before the first frame update
    void Start()
    {
        if(target==null)
        target=GameObject.Find("First Person Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (detected == true)
        {

            Vector3 targetPosition = new Vector3(target.transform.position.x, enemy.transform.position.y, target.transform.position.z);
            if (enemy != null)
            {
                enemy.LookAt(target.transform.position);
            }
        }

        Attacking();
                
            
        
    }

    public void Attacking()
    {
        //Vector3 v = (target.transform.position - shootPoint.transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.transform.position, shootPoint.transform.forward, out hit, range))
        {

            if (hit.collider.gameObject.tag == "Player")
            {

                if (Time.time >= nextTimeToFire)
                {
                    nextTimeToFire = Time.time + 1f / fireRate;
                    Invoke("EnemyShooting", shootDelayOnSight);
                    //EnemyShooting();
                    //muzzleFlash.Play();

                }
            }
            if (hit.collider.gameObject.tag != "Player")
            {
                muzzleFlash.Stop();
            }


        }
       
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.tag=="Player")
        {
            detected = true;
            target = other.gameObject;
           // muzzleFlash.Play();

        }
    }
    private void OnTriggerExit(Collider other)
    {
        /*
        if (other.tag == "Player")
        {
            detected = false;
            target = other.gameObject;
            muzzleFlash.Stop();

        }
        */
    }

    private void EnemyShooting()
    {
        muzzleFlash.Play();
        SoundManager.PlaySound("fire");

        Vector3 shootDir = shootPoint.forward;
        shootDir.x += Random.Range(-spreadFactor, spreadFactor);
        shootDir.y += Random.Range(-spreadFactor, spreadFactor);
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.transform.position, shootDir, out hit, range))
        {


            TargetMe target = hit.collider.GetComponent<TargetMe>();
            Debug.Log(hit.collider.name);
            //if (photonView.IsMine)
            // {
            //shooting another player
            if (hit.collider.gameObject.tag == "Player")
            {
                //RPC to damage player
                if (target != null)
               // {
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
               // }
            }

            //}
        }
        if (hit.rigidbody != null)
        {
            hit.rigidbody.AddForce(-hit.normal * impactForce);
            Debug.Log(hit.rigidbody.gameObject.name + " rigidbody");
        }

        GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impactGO,2f);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(shootPoint.position, (shootPoint.forward*100f)+shootPoint.position);
        Gizmos.DrawRay(shootPoint.position, shootPoint.forward);

    }
}

