using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    Transform _destination;
    public NavMeshAgent _navMeshAgent;
    Vector3 curPos;
    public Transform player;
    public Vector3 walkPoint;
    bool isWalkPointSet;
    public float walkPointRange=5f;
    public float sightRange=10f, attackRange=10f;
    public bool playerInSightRange, playerInAttackRange;
    public LayerMask whatIsPlayer;
    private Rigidbody r;
    public Transform wallDetect;
    public float timeUntilMove = 5f;
    public float HP = 100f;
    bool chaseFlag = false;

    Vector3 lastPos;
    public Animator anim;
    // Start is called before the first frame update

    private void Awake()
    {
        chaseFlag = false;

        //player = GameObject.Find("First Person Player").transform;
        anim = GetComponent<Animator>();

        r = GetComponent<Rigidbody>();

    }
    void Start()
    {
        if(GameObject.Find("First Person Player"))
        {
            player = GameObject.Find("First Person Player").transform;
        }
        

        _navMeshAgent = this.GetComponent<NavMeshAgent>();
        /*
        if (_destination == null)
        {
            _destination=player.transform;
        }
       */
        

    }
    private void FixedUpdate()
    {
        if (player==null)
        {
            player = GameObject.Find("First Person Player(Clone)").transform;
        }
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

       
        if (_navMeshAgent == null)
        {   
            Debug.LogError("Nav mesh agent not attached to" + gameObject.name);
        }
        if (chaseFlag)
        {
            ChaseFlag();
        }
        else
        {
            if(!playerInSightRange)
                     Patrolling();
            if (playerInSightRange)
            {
                //transform.LookAt(player.position);
                ChasePlayer();
            }
               
            //SetDestination();
            anim.SetBool("Walking", true);
        }
        //Debug.Log(timeUntilMove+ " timeUntilMove");
        //mn Debug.Log(Time.time + " Time.time");
    }


    private void Patrolling()
    {
        
        if (!isWalkPointSet) SearchWalkPoint();
        if (isWalkPointSet)
        {
            _navMeshAgent.SetDestination(walkPoint);
            timeUntilMove -= Time.deltaTime;
        }
           
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f||timeUntilMove<=0)
        {
            isWalkPointSet = false;
            timeUntilMove = 5f;
        }
        
       
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        isWalkPointSet = true;
    }

    private void ChasePlayer()
    {
        _navMeshAgent.SetDestination(player.position);
    }
    private void ChaseFlag()
    {
        _navMeshAgent.SetDestination(FindObjectOfType<Flag>().transform.position);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        
    }

    /*
        private void SetDestination()
        {
            if(_destination!=null)
            {
                Vector3 targetVector = _destination.transform.position;

                _navMeshAgent.SetDestination(targetVector);
            }
        }
        */

  



}
