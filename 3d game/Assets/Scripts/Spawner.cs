using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject AI;
    private List<GameObject> AIs;
    public GameObject gameThingy;
    public int aiOnScreen=10;

    int i;
    private float spawnDelay = 10;
    public float max_X;
    public float max_Z;
    
    private void Start()
    {
        AIs = new List<GameObject>();
        for (i = 0; i < 10; i++)
        {
            AIs.Add(AI);
            
        }
       
            InvokeRepeating("Spawn", 1f,2f);
        
        
    }
    private void Update()
    {

        
           
        
       
            
    }
    private bool shouldSpawn()
    {
        
        return true;
    }
    public void Spawn()
    {
        float randomX = Random.Range(-max_X, max_X);
        float randomZ = Random.Range(-max_Z, max_Z);

        Vector3 randomSpawnPos = new Vector3(randomX, 2f, randomZ);


        if (transform.childCount < aiOnScreen)
        {
            gameThingy = Instantiate(AI, randomSpawnPos, Quaternion.identity, transform) as GameObject;
        }
        
            

    }
}
