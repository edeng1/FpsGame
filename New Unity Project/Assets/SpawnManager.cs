using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    SpawnPoint[] spawnPoints;
    List<SpawnPoint> homeSpawn;
    List<SpawnPoint> awaySpawn;
    
    private void Awake()
    {
        Instance = this;
        spawnPoints = GetComponentsInChildren<SpawnPoint>();

        SetupTDMSpawns();

    }
    private void SetupTDMSpawns()
    {
        if (GameSettings.GameMode == GameMode.TDM)
        {
          
            homeSpawn = new List<SpawnPoint>();
            awaySpawn = new List<SpawnPoint>();
            foreach (SpawnPoint sp in spawnPoints)
            {
                
                if (sp.CompareTag("Home"))
                {
                    homeSpawn.Add(sp);
                   
                }
                if (sp.CompareTag("Away"))
                {
                    awaySpawn.Add(sp);
                    
                }

            }

        }
    }
    public Transform GetSpawnPoint()
    {
        if(GameSettings.GameMode== GameMode.TDM)
        {
            
            if (GameSettings.IsAwayTeam)
            {
                return awaySpawn[Random.Range(0,awaySpawn.Count)].transform;
            }
            else
                return homeSpawn[Random.Range(0, homeSpawn.Count)].transform;
        }

        return spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
    }

}
