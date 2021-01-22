using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleDetectArea : DetectArea
{
    // Start is called before the first frame update
    public void Start()
    {
        if(GameObject.Find("First Person Player"))
        {
            target = GameObject.Find("First Person Player");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target==null)
        {
            target = GameObject.Find("First Person Player(Clone)");
        }
        if (enemy != null)
        {
            enemy.LookAt(target.transform.position);
        }
        Attacking();
    }
}
