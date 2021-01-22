using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabPackage : MonoBehaviour
{
    public GameObject package;
    public GameObject activeSpawns;
    public GameObject activeBots;
    public GameObject inactiveBots;
    public bool haveSpawnsandBots;

    public GameObject extractionPoint;
    ExtractionManager man;
    public void Awake()
    {
        man = FindObjectOfType<ExtractionManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (haveSpawnsandBots == true)
            {
                package.SetActive(false);
                activeSpawns.SetActive(false);
                activeBots.SetActive(false);
                inactiveBots.SetActive(true);
            }
            

            extractionPoint.SetActive(true);
            man.GotThePackage();
        }
       

    }
}
