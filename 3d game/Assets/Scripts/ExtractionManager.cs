using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ExtractionManager : Manager
{
    //public Text topLeftText;
    // Start is called before the first frame update
    //PlayerMovement p;
    public string displayMessage="Return to the start";
    private void Awake()
    {
       
        player = GameObject.Find("First Person Player");
        p = FindObjectOfType<PlayerMovement>();
    }
    void Start()
    {

        if (GameObject.Find("First Person Player"))
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
    public void GotThePackage()
    {
        killsUI.text = displayMessage;
    }
   
    public void GoBackToMenu()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 3);
        SceneManager.LoadScene(0);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
