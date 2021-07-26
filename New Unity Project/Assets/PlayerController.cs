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

    int itemIndex;
    int previousItemIndex=-1;


    public bool awayTeam;
    public float verticalLookRotation;
    bool isGrounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    private Vector3 realPosition = Vector3.zero;
    public Animator anim;
    bool isSprinting;
    public bool die = false;
    private float lastTime;
    private float nextTimeToFire = 0f;
    [SerializeField] TMP_Text healthUI;
    [SerializeField] TMP_Text ammoUI;
    public bool isDead;
    public float pointIncreasePerSecond = 5f;

    public Transform playerBody;

    Rigidbody rb;

    PhotonView PV;

    [SerializeField] private GameObject ragdollModel;
    [SerializeField] private GameObject normalModel;

    PlayerManager playerManager;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    private void Awake()
    {
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

            
            anim = GetComponent<Animator>();
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
            
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
        if (die)
        {
            Die();
            die = false;
        }
        Look();
        Move();
        Jump();
        Reload();
        Sprint();
        HealthUI(currentHealth);
        AmmoUI();
        for (int i=0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }
        

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
            if (Input.GetMouseButton(0) &&Time.time>=nextTimeToFire && !isSprinting)
            {
                nextTimeToFire = Time.time + 1f / gunInfo.fireRate;
                items[itemIndex].Use();
            }
        }



        if (transform.position.y < -20f)
        {
            Die();
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
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(!PV.IsMine && targetPlayer==PV.Owner)
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

    private void FixedUpdate()
    {

        if (!PV.IsMine)
        {
            return;
        }
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
        
        
           //rb.MovePosition(Vector3.Lerp(transform.position, realPosition, Time.fixedDeltaTime / (1 / 30)));
        
    }

    public void TakeDamage(float damage, int actorNumber)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage, actorNumber);
    }


    [PunRPC]
    void RPC_TakeDamage(float damage,int actorNumber)
    {
        if (!PV.IsMine)
        {
            return;
        }

        currentHealth -= damage;
        
        if(currentHealth<=0)
        {
            
            Die(actorNumber);
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

            PV.RPC("ToggleDead", RpcTarget.All);

            playerManager.StartCoroutine(playerManager.Die());
        }
    }
    void Die(int actorNumber)
    {
        if (!isDead)
        {

            isDead = true;

            PV.RPC("ToggleDead", RpcTarget.All);

            playerManager.StartCoroutine(playerManager.Die(actorNumber));
        }
    }

    [PunRPC]
    public void ToggleDead()
    {
        CopyTransformData(sourceTransform: normalModel.transform, destinationTransform: ragdollModel.transform);
        ragdollModel.gameObject.SetActive(true);
        normalModel.gameObject.SetActive(false);
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


}
