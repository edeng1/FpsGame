using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    Transform _destination;
    NavMeshAgent _navMeshAgent;
    Vector3 curPos;
    
    Vector3 lastPos;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();
        if (_destination == null)
        {
            _destination=GameObject.Find("First Person Player").transform;
        }
       
    }
    private void FixedUpdate()
    {
        if (_navMeshAgent == null)
        {
            Debug.LogError("Nav mesh agent not attached to" + gameObject.name);
        }
        else
        {
            SetDestination();
            //anim.SetBool("Walking", true);
        }
       
    }


    private void SetDestination()
    {
        if(_destination!=null)
        {
            Vector3 targetVector = _destination.transform.position;
            
            _navMeshAgent.SetDestination(targetVector);
        }
    }

    

}
