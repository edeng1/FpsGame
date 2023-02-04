using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks,IPunObservable, IDamageable
{
    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    float initialWalkSpeed;
    [SerializeField] GameObject cameraHolder;

    [SerializeField] Item[] items;

    int itemIndex, enemyItemIndex;
    int previousItemIndex=-1;

    public CharacterController controller;
    public bool awayTeam;
    public float verticalLookRotation;
    bool isGrounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    private Vector3 realPosition = Vector3.zero;
    public Animator anim;
    [SerializeField] Pause pauseM;
    bool isSprinting;
    public bool die = false;
    private float lastTime;
    private float nextTimeToFire = 0f;
    [SerializeField] TMP_Text healthUI;
    [SerializeField] TMP_Text ammoUI;
    [SerializeField] TMP_Text eventUI;
    [SerializeField] TMP_Text eventLevelUpUI;
    [SerializeField] TMP_Text eventWeaponUnlockUI;
    [SerializeField] TMP_Text eventKillUI;

    [SerializeField] GameObject eventHeadshotUI;
    [SerializeField] GameObject KillFeedUI;
    public bool isDead;
    public float pointIncreasePerSecond = 5f;
    //Vector3 move;
    public Transform playerBody;
    Vector3 velocity;
    Rigidbody rb;
    const float GRAVITY = -9.81f;
    const float fallSpeed = 7f;
    PhotonView PV;
    private int actorNumber;
    public GameObject[] headColor;
    public GameObject[] skinColor;
    public GameObject[] headColorRag;
    public GameObject[] skinColorRag;
    public Texture2D[] crosshairs;
    public RawImage crosshair;

    //Values that will be synced over network
    Vector3 latestPos;
    Quaternion latestRot;
    //Lag compensation
    float currentTime = 0;
    double currentPacketTime = 0;
    double lastPacketTime = 0;
    Vector3 positionAtLastPacket = Vector3.zero;
    Quaternion rotationAtLastPacket = Quaternion.identity;

    [SerializeField] private GameObject ragdollModel;
    [SerializeField] private GameObject normalModel;

    PlayerManager playerManager;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;
    bool isShooting = false;
    bool isCrouched = false;
    
    private void Awake()
    {
       
        actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        isDead = false;
        isSprinting = false;
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        sprintSpeed *= transform.localScale.x;
        walkSpeed *=transform.localScale.x;
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
        awayTeam = (bool)PV.InstantiationData[1];

    }
    private void Start()
    {
        if (PV.IsMine)
        {
            crosshairs = new Texture2D[35];
            for(int i = 0; i < 35; i++)
            {
                crosshairs[i]=Resources.Load<Texture2D>("CrosshairPack/" + (i+1)) as Texture2D;
            }
            //crosshairs = Resources.LoadAll("CrosshairPack", typeof(Texture2D));
            EquipItem(0);

            if (!PlayerPrefs.HasKey("sens"))
            {
                PlayerPrefs.SetFloat("sens", mouseSensitivity);
            }
            if (PlayerPrefs.HasKey("Crosshair"))
            {
               crosshair.texture= (Texture2D)crosshairs[PlayerPrefs.GetInt("Crosshair") - 1] as Texture2D;
            }

            anim = GetComponent<Animator>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(ragdollModel.transform.GetChild(7).GetChild(0).gameObject);
            Destroy(rb);
            
            //Destroy(healthUI.transform.parent.gameObject);
            //Destroy(ammoUI.transform.parent.gameObject);
            Destroy(transform.GetChild(5).gameObject);
        }
        ChooseSkinColor();
        initialWalkSpeed = walkSpeed;
    }
    private void OnEnable()
    {
        UIEventSystem.current.onFlagPickUp += EventUI;
        UIEventSystem.current.onFlagDrop += EventUI;
        UIEventSystem.current.onFlagReturn += EventUI;
        UIEventSystem.current.onFlagCapture += EventUI;
        UIEventSystem.current.onPlayerKilled += EventKillUI;
        UIEventSystem.current.onLevelUp += EventLevelUpUI;
        UIEventSystem.current.onWeaponUnlocked += EventWeaponUnlockUI;
        base.OnEnable();
    }
    private void OnDisable()
    {
        UIEventSystem.current.onFlagPickUp -= EventUI;
        UIEventSystem.current.onFlagDrop -= EventUI;
        UIEventSystem.current.onFlagReturn -= EventUI;
        UIEventSystem.current.onFlagCapture -= EventUI;
        UIEventSystem.current.onPlayerKilled -= EventKillUI;
        UIEventSystem.current.onLevelUp -= EventLevelUpUI;
        UIEventSystem.current.onWeaponUnlocked -= EventWeaponUnlockUI;
        base.OnDisable();
    }
    private void Update()
    {
        if (!PV.IsMine)
        {
            //LagComp();
               
           
        }
        if (PV.IsMine)
        {


            if (awayTeam != playerManager.awayTeam)
            {
                //PV.RPC("SyncTeam", RpcTarget.All, GameSettings.IsAwayTeam);
            }
            ChooseSkinColor();
            if (die)
            {
                Die();
                die = false;
            }
            bool pause = Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.P);

            if (pause)
            {
                pauseM.TogglePause();
            }
            if (pauseM) //if instance of pause manager exists
            {
                if (!pauseM.paused) //if not paused
                {
                    Look();
                    //Move();
                    //Jump();
                    Reload();
                    Sprint();
                    OnStateChange();
                    Crouch();
                    SwitchWeapon();
                    Shoot();
                    anim.SetBool("isShooting", isShooting);
                }
            }

            if (mouseSensitivity != PlayerPrefs.GetFloat("sens"))
            {
                mouseSensitivity = PlayerPrefs.GetFloat("sens");
            }

            HealthUI(currentHealth);
            AmmoUI();







            if (transform.position.y < -40f)
            {
                Die();
            }
        }

    }
    void ChooseSkinColor()
    {
        PV.RPC("RPC_ChooseSkinColor", RpcTarget.All, GameSettings.IsAwayTeam);
    }
    [PunRPC]
    void RPC_ChooseSkinColor(bool awayTeam)
    {
      
        if (awayTeam)//white
        {
            headColor[0].SetActive(false);
            headColor[1].SetActive(true);
            skinColor[0].SetActive(false);
            skinColor[1].SetActive(true);

            headColorRag[0].SetActive(false);
            headColorRag[1].SetActive(true);
            skinColorRag[0].SetActive(false);
            skinColorRag[1].SetActive(true);
        }
        else//black
        {
            headColor[1].SetActive(false);
            headColor[0].SetActive(true);
            skinColor[1].SetActive(false);
            skinColor[0].SetActive(true);

            headColorRag[1].SetActive(false);
            headColorRag[0].SetActive(true);
            skinColorRag[1].SetActive(false);
            skinColorRag[0].SetActive(true);
        }
    }
    
    void Look()
    {
        if (isDead) { return; }
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        verticalLookRotation += mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
        
    }
    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(x, 0, y).normalized;
        
        anim.SetFloat("PosY", y);
        anim.SetFloat("PosX", x);
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
        

        

        

    }
    float xAnim = 0;
    float yAnim = 0;
    void MoveController()
    {
        
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        //xAnim = Mathf.MoveTowards(xAnim, x, 1f*Time.deltaTime);
        //yAnim = Mathf.MoveTowards(yAnim, y, 1f * Time.deltaTime);

        Vector3 move = (transform.right * x + transform.forward * y);
        if (move.magnitude > 1)
        {
            move /= move.magnitude;
        }
       

        anim.SetFloat("PosY", y);
        anim.SetFloat("PosX", x);
        if(controller!=null)
            controller.Move(move * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed) * Time.deltaTime);
        velocity.y += GRAVITY*fallSpeed * Time.deltaTime;
        if (controller != null)
            controller.Move(velocity * Time.deltaTime);
        int currentY = 0;
        if (x != 0 || y != 0)
        {
            anim.SetBool("Forward",true);
        }
        else
        {
            anim.SetBool("Forward", false);
            //anim.SetBool("Backward", false);
        }
        
    }
    private void FixedUpdate()
    {

        if (!PV.IsMine)
        {
            return;
        }
        MoveController();
        //Move();
        //rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);


        //rb.MovePosition(Vector3.Lerp(transform.position, realPosition, Time.fixedDeltaTime / (1 / 30)));

    }
    void OnStateChange()
    {
        if (isCrouched)
        {
            walkSpeed = initialWalkSpeed / (float)1.5;
        }
        else
        {
            walkSpeed = initialWalkSpeed;
        }
    }
    void Crouch()
    {
        if (Input.GetKey(KeyCode.LeftControl)|| Input.GetKey(KeyCode.C))
        {
            isCrouched = true;
            isSprinting = false;
            
        }
        else
        {
            isCrouched = false;
            
        }
        
        anim.SetBool("isCrouched", isCrouched);
    }

    void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
           
        }
        else
        {
            isSprinting = false;
        }
        anim.SetBool("isSprinting", isSprinting);
    }
    void Reload()
    {
        if(Input.GetKeyDown(KeyCode.R)&&!isDead)
        {
        
            items[itemIndex].StartCoroutine(((SingeShotGun)items[itemIndex]).Reload());
            
        }
    }
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    bool animDone = false;
    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;

       
                itemIndex = _index;
                items[itemIndex].itemGameObject.SetActive(true);

                if (previousItemIndex != -1)
                {
                    items[previousItemIndex].itemGameObject.SetActive(false);
                }

                previousItemIndex = itemIndex;

                if (PV.IsMine)
                {
                    Hashtable hash = new Hashtable();
                    hash.Add("itemIndex", itemIndex);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

                }


                if (items[itemIndex].GetComponent<SingeShotGun>() != null)
                    anim.SetBool("isPistol", items[itemIndex].GetComponent<SingeShotGun>().gi.isPistol);
            
          
       
    }
    IEnumerator EquipAnim(int _index)
    {

        if (_index == previousItemIndex)
            yield break;
        bool hasGun = false;
        itemIndex = _index;
        
        if (items[itemIndex].GetComponent<SingeShotGun>() != null)
            hasGun = true;
       
       

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        }

        if (hasGun) { items[itemIndex].GetComponent<SingeShotGun>().switchingGuns=true; }
        anim.SetBool("isReloading", true);
       
        yield return new WaitForSeconds(.5f);
        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;
        items[itemIndex].itemGameObject.SetActive(true);

        if (items[itemIndex].GetComponent<SingeShotGun>() != null)
            anim.SetBool("isPistol", items[itemIndex].GetComponent<SingeShotGun>().gi.isPistol);

        if (hasGun) { if (hasGun) { items[itemIndex].GetComponent<SingeShotGun>().switchingGuns =false; } }
       
        anim.SetBool("isReloading", false);
        


       
        
    }


    void SwitchWeapon()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                //EquipItem(i);
                StartCoroutine(EquipAnim(i));
                break;
            }
        }
    }

    void Shoot()
    {
        
        var gunInfo = ((GunInfo)items[itemIndex].itemInfo);
        if (gunInfo.fullyAuto == false)
        {
            if (Input.GetMouseButtonDown(0) && !isSprinting && !isDead)
            {
                items[itemIndex].Use();
                isShooting = true;
                
                //anim.Play("PistolShoot");
            }
            else
            {
                isShooting = false;
            }
        }
        else
        {
            if (Input.GetMouseButton(0) &&  !isSprinting && !isDead)
            {
                if (Time.time >= nextTimeToFire)
                {
                    nextTimeToFire = Time.time + 1f / gunInfo.fireRate;
                    items[itemIndex].Use();
                }
                if(items[itemIndex].GetComponent<SingeShotGun>().GetClip()>0)
                    isShooting = true;
                
               
            }
            else
            {
                isShooting = false;
            }
        }
        
       
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
      
        if (!PV.IsMine && targetPlayer==PV.Owner)
        {
           
            EquipItem((int)changedProps["itemIndex"]); 
        }
        
        
    }

    public void SetGroundedState(bool _grounded)
    {
        isGrounded = _grounded;
    }

    
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
     {

        SerializeNormalComp(stream);
        //SerializeLagComp(stream, info);






    }
    private void SerializeNormalComp(PhotonStream stream)//Not used
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else if (stream.IsReading)
        {
            realPosition = (Vector3)stream.ReceiveNext();
        }
    }
    private void SerializeLagComp(PhotonStream stream, PhotonMessageInfo info)//Not used
    {
        if (stream.IsWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            //Network player, receive data
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();

            //Lag compensation
            currentTime = 0.0f;
            lastPacketTime = currentPacketTime;
            currentPacketTime = info.SentServerTime;
            positionAtLastPacket = transform.position;
            rotationAtLastPacket = transform.rotation;
        }
    }

    private void LagComp()//Not used
    {
        //Lag compensation
        double timeToReachGoal = currentPacketTime - lastPacketTime;
        currentTime += Time.deltaTime;

        //Update remote player
        transform.position = Vector3.Lerp(positionAtLastPacket, latestPos, (float)(currentTime / timeToReachGoal));
        transform.rotation = Quaternion.Lerp(rotationAtLastPacket, latestRot, (float)(currentTime / timeToReachGoal));
        Debug.Log((float)(currentTime / timeToReachGoal));
    }

    

    public void TakeDamage(float damage, int actorNumber, string gunName, bool headshot=false)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage, actorNumber,gunName,headshot);
    }


    [PunRPC]
    void RPC_TakeDamage(float damage,int actorNumber, string gunName,bool headshot)
    {
        if (!PV.IsMine)
        {
            return;
        }

        currentHealth -= damage;
        
        if(currentHealth<=0)
        {
            
            Die(actorNumber, gunName,headshot);
        }
    }

    void HealthUI(float health)
    {
        healthUI.text = (int)health+"HP";
        if(health>0)
        {
            currentHealth += pointIncreasePerSecond * Time.deltaTime;
        }
        if (health > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (healthUI!= null)
        {
            if (health <= 40)
            {

                healthUI.color = Color.red;
                healthUI.transform.parent.GetComponentInChildren<RawImage>().color = Color.red;
            }
            else
            {
                healthUI.color = Color.green;
                healthUI.transform.parent.GetComponentInChildren<RawImage>().color = Color.green;
            }
        }
          

    }

    void EventUI(string eventText)
    {
        if (PV.IsMine)
            StartCoroutine(DisplayEvent(eventText));
    }

    IEnumerator DisplayEvent(string eventText)
    {
        if (eventText.Substring(0, 4) == "Away")
        {
            eventUI.color = new Color32(236, 110, 10, 205);
        }
        else
        {
            eventUI.color =new Color32(87, 95, 197, 205);
        }
        eventUI.text = eventText;
        yield return new WaitForSeconds(1f);
        eventUI.text = "";
    }

    void EventKillUI(string eventText,bool headshot)
    {
        if(PV.IsMine)
            StartCoroutine(DisplayKillEvent(eventText,headshot));
    }
    IEnumerator DisplayKillEvent(string eventText,bool headshot)
    {

        TMP_Text newKill=null;
        GameObject newHeadShot = null;
        if (!headshot)
        {
            newKill = Instantiate(eventKillUI, KillFeedUI.transform);
            newKill.text = eventText;
            newKill.transform.SetAsFirstSibling();
        }
        else
        {
            newHeadShot = Instantiate(eventHeadshotUI, KillFeedUI.transform);
            newHeadShot.transform.GetChild(1).GetComponent<TMP_Text>().text = eventText;
            newHeadShot.transform.SetAsFirstSibling();
        }
        
        if (KillFeedUI.transform.childCount > 5)
        {
            if(KillFeedUI.transform.GetChild(5).gameObject)
                Destroy(KillFeedUI.transform.GetChild(5).gameObject);
        }
        yield return new WaitForSeconds(5.5f);
        if(newKill)
            Destroy(newKill);
        if (newHeadShot)
            Destroy(newHeadShot);
    }
    void EventLevelUpUI(string eventText)
    {
        if (PV.IsMine)
            StartCoroutine(DisplayLevelUpEvent(eventText));
    }
    IEnumerator DisplayLevelUpEvent(string eventText)
    {

        eventLevelUpUI.text = eventText;
        yield return new WaitForSeconds(3f);
        eventLevelUpUI.text = "";
    }
    void EventWeaponUnlockUI(string eventText)
    {
        if (PV.IsMine)
            StartCoroutine(DisplayWeaponUnlockEvent(eventText));
    }
    IEnumerator DisplayWeaponUnlockEvent(string eventText)
    {

        eventWeaponUnlockUI.text = eventText;
        yield return new WaitForSeconds(3f);
        eventWeaponUnlockUI.text = "";
    }

    void AmmoUI()
    {
        var gun= ((SingeShotGun)items[itemIndex]);
        ammoUI.text = gun.GetClip()+"/"+gun.GetStash();
       
    }

    void Die() //committed suicide
    {
        if (!isDead)
        {
            
            isDead = true;
            Destroy(transform.GetChild(5).GetChild(0).gameObject);
            Destroy(transform.GetChild(5).GetChild(1).gameObject);
            Destroy(transform.GetChild(5).GetChild(2).gameObject);
            PV.RPC("ToggleDead", RpcTarget.All, GetComponent<PhotonView>().ViewID);
           
            playerManager.StartCoroutine(playerManager.Die());
        }
    }
    void Die(int actorNumber,string gunName,bool headshot) //killed by player
    {
        if (!isDead)
        {

            isDead = true;
            Destroy(transform.GetChild(5).GetChild(0).gameObject);
            Destroy(transform.GetChild(5).GetChild(1).gameObject);
            Destroy(transform.GetChild(5).GetChild(2).gameObject);
            PV.RPC("ToggleDead", RpcTarget.All,GetComponent<PhotonView>().ViewID);
           
            
            playerManager.StartCoroutine(playerManager.Die(actorNumber,gunName,headshot));
        }
    }

    [PunRPC]
    public void ToggleDead(int vID)
    {
        CopyTransformData(sourceTransform: normalModel.transform, destinationTransform: ragdollModel.transform);
        normalModel.gameObject.SetActive(false);
        ragdollModel.gameObject.SetActive(true);
        Destroy(PhotonView.Find(vID).GetComponent<PlayerController>().controller);

        
    }

    private void CopyTransformData(Transform sourceTransform, Transform destinationTransform)
    {
        
        if (sourceTransform.childCount != destinationTransform.childCount)
        {
            Debug.LogWarning("Invalid transform copy");
        }
        for (int i = 0; i < sourceTransform.childCount; i++)
        {
            if (sourceTransform.childCount == destinationTransform.childCount)
            {
                var source = sourceTransform.GetChild(i);
                var destination = destinationTransform.GetChild(i);


                destination.position = source.position;
                destination.rotation = source.rotation;
                var rb = destination.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = new Vector3(-5, -5, -5);
                }

                CopyTransformData(source, destination);
            }
           
        }
        

        //destinationTransform.position = sourceTransform.position;
        //destinationTransform.rotation = sourceTransform.rotation;
    }

    public void TrySync()
    {
        PV.RPC("SyncTeam", RpcTarget.All, GameSettings.IsAwayTeam);
    }

    [PunRPC]
    private void SyncTeam(bool _awayTeam)
    {
        awayTeam = _awayTeam;
        //transform.Find("Primary").GetComponent<SingeShotGun>().instantiateGunModel();
        if (!awayTeam)
        {

        }
    }

    public bool getAwayTeam()
    {
        return awayTeam;
    }

    public int getActorNumber()
    {
        return actorNumber;
    }

    public string GetGunName()
    {
        return ((SingeShotGun)items[itemIndex]).itemInfo.itemName;
    }



}
