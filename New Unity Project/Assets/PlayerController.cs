using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;


public class PlayerController : MonoBehaviourPunCallbacks,IPunObservable, IDamageable
{
    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
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
    [SerializeField] TMP_Text eventKillUI;
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

    [SerializeField] private GameObject ragdollModel;
    [SerializeField] private GameObject normalModel;

    PlayerManager playerManager;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;

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
        awayTeam = playerManager.awayTeam;

    }
    private void Start()
    {
        if (PV.IsMine)
        {
            EquipItem(0);

            if (!PlayerPrefs.HasKey("sens"))
            {
                PlayerPrefs.SetFloat("sens", mouseSensitivity);
            }
            
            anim = GetComponent<Animator>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(healthUI.transform.parent.gameObject);
            Destroy(ammoUI.transform.parent.gameObject);
        }
        ChooseSkinColor();

    }
    private void OnEnable()
    {
        UIEventSystem.current.onFlagPickUp += EventUI;
        UIEventSystem.current.onFlagDrop += EventUI;
        UIEventSystem.current.onFlagReturn += EventUI;
        UIEventSystem.current.onFlagCapture += EventUI;
        UIEventSystem.current.onPlayerKilled += EventKillUI;
        base.OnEnable();
    }
    private void OnDisable()
    {
        UIEventSystem.current.onFlagPickUp -= EventUI;
        UIEventSystem.current.onFlagDrop -= EventUI;
        UIEventSystem.current.onFlagReturn -= EventUI;
        UIEventSystem.current.onFlagCapture -= EventUI;
        UIEventSystem.current.onPlayerKilled -= EventKillUI;
        base.OnDisable();
    }
    private void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }
        if (awayTeam != playerManager.awayTeam)
        {
            PV.RPC("SyncTeam", RpcTarget.All, GameSettings.IsAwayTeam);
        }
        ChooseSkinColor();
        if (die)
        {
            Die();
            die = false;
        }
        bool pause = Input.GetKeyDown(KeyCode.Escape);
        
        if(pause)
        {
           pauseM.TogglePause();
        }
        if(pauseM){
            if (!pauseM.paused)
            {
                Look();
                //Move();
                Jump();
                Reload();
                Sprint();
                SwitchWeapon();
                Shoot();
            }
        }

        if(mouseSensitivity!=PlayerPrefs.GetFloat("sens"))
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
    void MoveController()
    {
        
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 move = (transform.right * x + transform.forward * y);
        if (move.magnitude > 1)
        {
            move /= move.magnitude;
        }
        anim.SetFloat("PosY", y);
        anim.SetFloat("PosX", x);
        controller.Move(move * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed) * Time.deltaTime);
        velocity.y += GRAVITY*fallSpeed * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        
    }
    private void FixedUpdate()
    {

        if (!PV.IsMine)
        {
            return;
        }
        MoveController();
        //rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);


        //rb.MovePosition(Vector3.Lerp(transform.position, realPosition, Time.fixedDeltaTime / (1 / 30)));

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
        if(Input.GetKey(KeyCode.R))
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
            if (Input.GetMouseButtonDown(0) && !isSprinting)
            {
                items[itemIndex].Use();
            }
        }
        else
        {
            if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire && !isSprinting)
            {
                nextTimeToFire = Time.time + 1f / gunInfo.fireRate;
                items[itemIndex].Use();
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
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else if (stream.IsReading)
        {
            realPosition = (Vector3)stream.ReceiveNext();
        }
     }

    

    public void TakeDamage(float damage, int actorNumber, string gunName)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage, actorNumber,gunName);
    }


    [PunRPC]
    void RPC_TakeDamage(float damage,int actorNumber, string gunName)
    {
        if (!PV.IsMine)
        {
            return;
        }

        currentHealth -= damage;
        
        if(currentHealth<=0)
        {
            
            Die(actorNumber, gunName);
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
        if (health<=40)
        {
            healthUI.color = Color.red;
        }
        else
            healthUI.color = Color.green;

    }

    void EventUI(string eventText)
    {
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

    void EventKillUI(string eventText)
    {
        StartCoroutine(DisplayKillEvent(eventText));
    }
    IEnumerator DisplayKillEvent(string eventText)
    {
        
        //eventKillUI.text = eventText;
        TMP_Text newKill=Instantiate(eventKillUI,KillFeedUI.transform);
        newKill.text = eventText;
        newKill.transform.SetAsFirstSibling();
        if (KillFeedUI.transform.childCount > 5)
        {
            if(KillFeedUI.transform.GetChild(5).gameObject)
                Destroy(KillFeedUI.transform.GetChild(5).gameObject);
        }
        yield return new WaitForSeconds(5.5f);
        if(newKill)
            Destroy(newKill);
    }

    void AmmoUI()
    {
        var gun= ((SingeShotGun)items[itemIndex]);
        ammoUI.text = gun.GetClip()+"/"+gun.GetStash();
       
    }

    void Die()
    {
        if (!isDead)
        {
            
            isDead = true;
            //GetComponentInChildren<Camera>().gameObject.SetActive(false);
            PV.RPC("ToggleDead", RpcTarget.All);

            playerManager.StartCoroutine(playerManager.Die());
        }
    }
    void Die(int actorNumber,string gunName)
    {
        if (!isDead)
        {

            isDead = true;

            PV.RPC("ToggleDead", RpcTarget.All);

            playerManager.StartCoroutine(playerManager.Die(actorNumber,gunName));
        }
    }

    [PunRPC]
    public void ToggleDead()
    {
        CopyTransformData(sourceTransform: normalModel.transform, destinationTransform: ragdollModel.transform);
        normalModel.gameObject.SetActive(false);
        ragdollModel.gameObject.SetActive(true);
        
    }

    private void CopyTransformData(Transform sourceTransform, Transform destinationTransform)
    {
        /*
        if (sourceTransform.childCount != destinationTransform.childCount)
        {
            Debug.LogWarning("Invalid transform copy");
        }
        for (int i = 0; i < sourceTransform.childCount; i++)
        {
            var source = sourceTransform.GetChild(i);
            var destination = destinationTransform.GetChild(i);
            destination.position = source.position;
            destination.rotation = source.rotation;
            var rb = destination.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity =new Vector3(0,0,0);
            }

            CopyTransformData(source, destination);
        }
        */

        destinationTransform.position = sourceTransform.position;
        destinationTransform.rotation = sourceTransform.rotation;
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
