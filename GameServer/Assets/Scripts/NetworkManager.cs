using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject playerPrefab;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }      
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object");
            Destroy(this);
        }
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
       
        Server.Start(10, 17069);
        
    }
    private void OnApplicationQuit()
    {
        Server.Stop();
    }
    public Player instantiatePlayer()
    {
        return Instantiate(playerPrefab, new Vector3(0f,0.5f,0f), Quaternion.identity).GetComponent<Player>();
    }
}
