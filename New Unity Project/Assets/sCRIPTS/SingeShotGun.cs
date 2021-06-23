using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingeShotGun : Gun
{

    PhotonView PV;
    PlayerController player;
    Animator anim;
    GunInfo gi;
    ParticleSystem muzzleFlash;
    private int clip;
    private int stash;
    private int ammoUsed;
    public bool canShoot;
    private bool isReloading;
    float time;

    [SerializeField] Camera cam;

    private void Awake()
    {
        
        anim= transform.root.GetComponent<Animator>();
        player = transform.root.GetComponent<PlayerController>();
        PV = GetComponent<PhotonView>();
        gi = (GunInfo)itemInfo;
        clip = gi.clipSize;
        stash = gi.totalAmmo;
        if (PV.IsMine)
        {
            PV.RPC("instantiateGunModel", RpcTarget.All);
        }
       
    }
    void Update()
    {
        if (time > 0)
        {
            player.verticalLookRotation += (gi.verticalRecoil * Time.deltaTime) / gi.duration;
            time -= Time.deltaTime;
        }
    }
    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        if (clip > 0)
        {
            clip--;
        }
        else
        {
            StartCoroutine(Reload());
            return;
        }
        GenerateRecoil();
        PV.RPC("RPC_muzzleFlash", RpcTarget.All);
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;

        if(Physics.Raycast(ray,out RaycastHit hit))
        {
            //hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            
            switch (hit.collider.gameObject.name)
            {
                case "upperArm.L":
                    hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageArm);
                    break;
                case "upperArm.R":
                    hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageArm);
                    break;
                case "lowerArm.L":
                    hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageArm);
                    break;
                case "lowerArm.R":
                    hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageArm);
                    break;
                case "spine":
                    hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageBody);
                    break;
                case "upperLeg.L":
                    hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageArm);
                    break;
                case "upperLeg.R":
                    hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageArm);
                    break;
                case "lowerLeg.L":
                    hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageArm);
                    break;
                case "lowerLeg.R":
                    hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageArm);
                    break;
                case "head":
                    hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageHead);
                    break;

            }
            
            //hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point,hit.normal);
        }    
    }
    void GenerateRecoil()
    {
        time = gi.duration;
    }


    [PunRPC]
    void RPC_muzzleFlash()
    {
        
            muzzleFlash?.Play();
    }
    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Instantiate(bulletImpactPrefab, hitPosition, Quaternion.LookRotation(hitNormal,Vector3.up));
        

    }

    [PunRPC]
    private void instantiateGunModel()
    {
       
            GameObject gunM=Instantiate(itemInfo.itemModel, new Vector3(.2f,.35f, .15f), transform.localRotation*Quaternion.Euler(0, 90, 0), transform.GetChild(0));
        //new Vector3(transform.position.x + 2f, transform.position.y+ 4f, transform.position.z + 2f)
       
        gunM.transform.localPosition = new Vector3(.2f, .35f, .15f);
        gunM.transform.localRotation = transform.localRotation * Quaternion.Euler(0, 90, 0);
        
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
    }

    public IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(gi.reloadTime - .25f);
        stash += clip;
        clip = Mathf.Min(gi.clipSize, stash);
        stash -= clip;
        

        yield return new WaitForSeconds(.25f);
        isReloading = false;
    }


    void RPC_Reload()
    {

    }

    public int GetStash() { return stash; }
    public int GetClip() { return clip; }
}
