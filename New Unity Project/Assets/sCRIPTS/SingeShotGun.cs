using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;
public class SingeShotGun : Gun
{

    PhotonView PV;
    PlayerController player;
    Animator anim;
    public GunInfo gi { get; private set; }
    ParticleSystem muzzleFlash;
    public Dictionary<string, GunInfo> gunsDictionairy;
    UnityEngine.Object[] gunsArray;
    private int clip;
    private int stash;
    private int ammoUsed;
    public bool canShoot;
    public bool isReloading;
    public bool switchingGuns;
    public AudioClip reloadSound;
    public Shaker myShaker;
    public ShakePreset myShakePreset;
    float time;


    [SerializeField] Camera cam;

    private void Awake()
    {
        if (itemGameObject.name == "Primary")
        {
            if (PlayerPrefs.HasKey("Guns")) //Check current gun that's equipped by player.
                itemInfo = Resources.Load<GunInfo>("Items/Guns/" + PlayerPrefs.GetString("Guns"));
            else
                itemInfo = Resources.Load<GunInfo>("Items/Guns/AK47");
        }

        gunsArray = Resources.LoadAll("Items/Guns", typeof(GunInfo));
        foreach(GunInfo g in gunsArray)
        {
            Debug.Log(g.itemName+ "guns in Singeshotgun");
        }
        gunsDictionairy = new Dictionary<string, GunInfo>();
        SetGunDictionairy(gunsArray);

        anim = transform.root.GetComponent<Animator>();
        player = transform.root.GetComponent<PlayerController>();
        PV = GetComponent<PhotonView>();
        gi = (GunInfo)itemInfo;
        clip = gi.clipSize;
        stash = gi.totalAmmo;
        reloadSound = (AudioClip)Resources.Load("Items/Guns/reload1", typeof(AudioClip));
        //RoomManager.Instance.guns.Add(this.GetComponent<SingeShotGun>());
        if (PV.IsMine)
        {
            instantiateGunModel();
        }

    }
    private void OnEnable()
    {
        if (isReloading) { isReloading = false; }
    }
    void Update()
    {
        if (time > 0)
        {
            player.verticalLookRotation += (gi.verticalRecoil * Time.deltaTime) / gi.duration;
            time -= Time.deltaTime;
        }
    }
    private void SetGunDictionairy(UnityEngine.Object[] gunsArr){
        //Dictionary<string, GunInfo> gunsDict=new Dictionary<string, GunInfo>();
        foreach (GunInfo g in gunsArr)
        {
            gunsDictionairy.Add(g.itemName, g);
        }
        foreach (string g in gunsDictionairy.Keys)
        {
            Debug.Log(g + " dict");
        }
    }
    public override void Use()
    {
        Shoot();
        
    }

    void Shoot()
    {
        if (switchingGuns) { return; }
        if (isReloading) { return; }
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
        myShaker.Shake(myShakePreset);
        PV.RPC("RPC_playSound",RpcTarget.All,itemInfo.name);
        PV.RPC("RPC_muzzleFlash", RpcTarget.All);
        
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;

        if(Physics.Raycast(ray,out RaycastHit hit))
        {
            //hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            bool applyDamage = false;
            if(GameSettings.GameMode==GameMode.FFA)
            {
                applyDamage = true;
            }
            if (GameSettings.GameMode != GameMode.FFA)
            {
                if (hit.collider.transform.root.GetComponent<IDamageable>()?.getAwayTeam() != GameSettings.IsAwayTeam)
                {

                    applyDamage = true;
                }
            }

                if (applyDamage)
                {


                    switch (hit.collider.gameObject.name)
                    {
                        case "upperArm.L":
                            hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageArm, PhotonNetwork.LocalPlayer.ActorNumber, itemInfo.itemName);
                            break;
                        case "upperArm.R":
                            hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageArm, PhotonNetwork.LocalPlayer.ActorNumber, itemInfo.itemName);
                            break;
                        case "lowerArm.L":
                            hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageArm, PhotonNetwork.LocalPlayer.ActorNumber, itemInfo.itemName);
                            break;
                        case "lowerArm.R":
                            hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageArm, PhotonNetwork.LocalPlayer.ActorNumber, itemInfo.itemName);
                            break;
                        case "spine":
                            hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageBody, PhotonNetwork.LocalPlayer.ActorNumber, itemInfo.itemName);
                            break;
                        case "upperLeg.L":
                            hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageArm, PhotonNetwork.LocalPlayer.ActorNumber, itemInfo.itemName);
                            break;
                        case "upperLeg.R":
                            hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageArm, PhotonNetwork.LocalPlayer.ActorNumber, itemInfo.itemName);
                            break;
                        case "lowerLeg.L":
                            hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageArm, PhotonNetwork.LocalPlayer.ActorNumber, itemInfo.itemName);
                            break;
                        case "lowerLeg.R":
                            hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageArm, PhotonNetwork.LocalPlayer.ActorNumber, itemInfo.itemName);
                            break;
                        case "head":
                            hit.collider.transform.root.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damageHead, PhotonNetwork.LocalPlayer.ActorNumber, itemInfo.itemName,true);
                            break;

                    }
                }

            //hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            if(hit.collider.transform.root.GetComponent<IDamageable>()!=null)
                 PV.RPC("RPC_Shoot", RpcTarget.All, hit.point,hit.normal,true);
            else
                PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal,false);

            
        }    
    }
    void GenerateRecoil()
    {
        time = gi.duration;
    }

    [PunRPC]
    void RPC_playSound(string gunName)
    {
        // UnityEngine.Object gun = Resources.Load("Items/Guns/" + gunName, typeof(GunInfo));// This will get the gun of the player whos shooting and play their gun sound
        UnityEngine.Object gun = gunsDictionairy[gunName]; // delete this and uncomment ^ if stops working
        if (((GunInfo)gun).itemSound!=null)
            sfx.PlayOneShot(((GunInfo)gun).itemSound);
    }


    [PunRPC]
    void RPC_muzzleFlash()
    {
        
            muzzleFlash?.Play();
    }
    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal, bool applyDamage)
    {
        if(!applyDamage)
            Instantiate(bulletImpactPrefab, hitPosition, Quaternion.LookRotation(hitNormal,Vector3.up));
        else
            Instantiate(bloodImpactPrefab, hitPosition, Quaternion.LookRotation(hitNormal, Vector3.up));

    }

    public void instantiateGunModel()
    {
        PV.RPC("RPC_instantiateGunModel", RpcTarget.All, itemInfo.name);
    }

    [PunRPC]
    private void RPC_instantiateGunModel(string gunName)
    {
        //UnityEngine.Object guns = Resources.Load("Items/Guns/"+gunName, typeof(GunInfo));// will get the other player
        UnityEngine.Object guns = gunsDictionairy[gunName]; // delete this and uncomment ^ if stops working

        GameObject gunM=Instantiate(((GunInfo)guns).itemModel, new Vector3(.2f,.35f, .15f), transform.localRotation*Quaternion.Euler(0, 90, 0), transform.GetChild(0));
        
       
        gunM.transform.localPosition = ((GunInfo)guns).itemPosition; //.2f .35f .15f
        gunM.transform.localRotation = transform.localRotation * Quaternion.Euler(((GunInfo)guns).itemRotation);// 0 90 0
        //gunM.transform.localRotation = transform.localRotation * Quaternion.Euler(0, 90, 0);
        gunM.transform.localScale = itemInfo.itemScale;

        muzzleFlash = GetComponentInChildren<ParticleSystem>();
    }

   
    public IEnumerator Reload()
    {
        if (stash > 0&&clip<gi.clipSize&&isReloading==false)
        {
            isReloading = true;
            sfx.PlayOneShot(reloadSound);
            player.anim.SetBool("isReloading", isReloading);
            yield return new WaitForSeconds(gi.reloadTime - .25f);
            stash += clip;
            clip = Mathf.Min(gi.clipSize, stash);
            stash -= clip;
           

            yield return new WaitForSeconds(.25f);
            isReloading = false;
            player.anim.SetBool("isReloading", isReloading);
        }

    }

    public void PlayReloadAnim()
    {
       isReloading = !isReloading; 
       player.anim.SetBool("isReloading", isReloading);
     
    }

    void RPC_Reload()
    {

    }

    public bool IsReloading()
    {
        return isReloading;
    }
    public int GetStash() { return stash; }
    public int GetClip() { return clip; }


}
