using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ComputerController : MonoBehaviour
{
    public List<ComputerTriggerArea> computers;

    List<ComputerTriggerArea> computersRemaining;

    [SerializeField] UnityEvent OnCompletedEvent;

    [SerializeField] Text t;

    // Start is called before the first frame update
    void Start()
    {
        computersRemaining = new List<ComputerTriggerArea>(computers);
        //foreach(var computer in computersRemaining)
        //{
            GameEvents.current.onComputerEnable += HandleComputerEnabled;
        //}
       
        
    }

    void HandleComputerEnabled(ComputerTriggerArea id)
    {
            computersRemaining.Remove(id);
        if (computersRemaining.Count == computersRemaining.Count)
        {
            t.text=($"{computersRemaining.Count} computers left");
        }
            if (computersRemaining.Count == 0)
        {
            OnCompletedEvent.Invoke();
        }

        
    }
}
