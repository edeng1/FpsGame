using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoorController : MonoBehaviour
{
    public int id;
    public float openDoorPos = 2.5f;
    public float closeDoorPos = 0f;
    public GameObject door;
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.current.onDoorWayTriggerOpen += OnDoorWayOpen;
        GameEvents.current.onDoorWayTriggerClose += OnDoorWayClose;
        
}

    private void OnDoorWayOpen(int id)
    {
        if (id == this.id)
        {
            LeanTween.moveLocalX(door, openDoorPos, 1f).setEaseInQuad();
        }
        
    }
    private void OnDoorWayClose(int id)
    {
        if (id == this.id)
        {
            LeanTween.moveLocalX(door, closeDoorPos, 1f).setEaseInQuad();
        }
    }

}
