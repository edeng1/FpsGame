using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Manager : MonoBehaviour
{
    public GameObject LevelWonScreen;
    public GameObject LevelLostScreen;
    public GameObject player;
    public GameObject cam;
    public Text killsUI;
    public Text HealthUI;
    
    public PlayerMovement p;
    //public GameObject playerSpawn;
    //public GameObject playerPrefab;
    public Rigidbody camRig;
    public GameObject dmgScreen;
    
    public int kills=35;
    // Start is called before the first frame update
    void Awake()
    {
        if(GameObject.Find("First Person Player"))
        {
            player = GameObject.Find("First Person Player");
        }
       

        p = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.Find("First Person Player(Clone)");
            p = FindObjectOfType<PlayerMovement>();
        }
        killsUI.text ="Enemies Remaining: "+kills;
        if (kills == 0)
        {
            GameWon();
            

        }
        HealthUI.text = (int)p.currentHP + "HP";
       
        if (p.currentHP < 40)
        {
            HealthUI.color = Color.red;
        }
        if (p.currentHP >= 40)
        {
            HealthUI.color = Color.green;
        }
    }
    
    public void addKill()
    {
        kills--;

        Debug.Log(kills);
    }
    public void GameWon()
    {

            
            LevelWonScreen.SetActive(true);
            Debug.Log("You Win");


            
            Invoke("GoBackToMenu", 3f);

    }
    public void GameLost()
    {
        

        LevelLostScreen.SetActive(true);
        Debug.Log("You lose");
        player.SetActive(false);
        cam.transform.position = player.transform.position;
        cam.transform.rotation = player.transform.rotation;
        cam.SetActive(true);
        camRig.AddForce(30,0,30);
        Invoke("GoBackToMenu", 3f);



    }
    public void TakingDmg()
    {
        
        dmgScreen.SetActive(true);
        Invoke("Healing", 0.1f);
    }
    public void Healing()
    {
        dmgScreen.SetActive(false);
    }
    public void GoBackToMenu()
    {
        SceneManager.LoadScene(0);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
  
    
}
