using UnityEngine;

public class SpawnOnTrigger : MonoBehaviour
{
    public int spawnAmount = 3;

    public GameObject[] spawns;
   
    public GameObject AI;
    
    private bool triggered;
    public float secondsToNextUse = 30f;
   
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && AI!=null)
        {
          
            if (!triggered)
            {
                for (int i = 0; i < spawnAmount; i++)
                {
                    Instantiate(AI, spawns[i].transform.position, Quaternion.identity);
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
