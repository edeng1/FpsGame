using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject AI;
    private GameObject[] AIs;
    public Enemy1 enemy;
    int i;
    private float spawnDelay = 10;
    public float max_X;
    public float max_Z;
    
    private void Start()
    {
        InvokeRepeating("Spawn", 3f,5f);
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
        Instantiate(AI, randomSpawnPos, Quaternion.identity);

    }
}
