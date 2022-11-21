using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{

    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }
    public event Action<int> onDoorWayTriggerOpen;
    public void DoorwayTriggerOpen(int id)
    {
        if (onDoorWayTriggerOpen!=null)
        {
            onDoorWayTriggerOpen(id);//invokes it
        }
    }

    public event Action<int> onDoorWayTriggerClose;
    public void DoorwayTriggerClose(int id)
    {
        if (onDoorWayTriggerClose != null)
        {
            onDoorWayTriggerClose(id);//invokes it
        }
    }

    public event Action<ComputerTriggerArea> onComputerEnable;
    public void ComputerEnabled(ComputerTriggerArea id)
    {
        if(onComputerEnable!=null)
        {
            onComputerEnable(id);//invokes it 
        }
        Debug.Log(onComputerEnable.GetInvocationList().Length+" Computerenabled action events");
    }
}
