using UnityEngine;

public class SpawnOnTrigger : MonoBehaviour
{
    public int spawnAmount = 3;
    public static int maxSpawnedBots = 20;
    public GameObject[] spawns;
   
    public GameObject AI;
    public GameObject botHolder;
    
    private bool triggered;
    public float secondsToNextUse = 30f;

    private void Start()
    {
        botHolder = GameObject.Find("SpawnedBots");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && AI!=null)
        {
          
            if (!triggered&& botHolder.transform.childCount < maxSpawnedBots)
            {
                for (int i = 0; i < spawnAmount; i++)
                {
                    
                        Instantiate(AI, spawns[i].transform.position, Quaternion.identity,botHolder.transform);
                    
                       
                }
                triggered = true;
                Invoke("SetTriggeredFalse", secondsToNextUse);
            }
           
        }
    }
    private void SetTriggeredFalse()
    {
        triggered = false;
    }

}
