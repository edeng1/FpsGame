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

                if(activeSpawns!=null)
                    activeSpawns.SetActive(false);


                if (activeBots != null)
                    activeBots.SetActive(false);

                if (inactiveBots != null)
                    inactiveBots.SetActive(true);

            }

            if(package!=null)
                package.SetActive(false);

            if(extractionPoint!=null)
                extractionPoint.SetActive(true);

            if(man!=null)
                man.GotThePackage();
        }
        }
       

}

