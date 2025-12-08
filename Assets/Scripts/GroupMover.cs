using UnityEngine;
using UnityEngine.AI;

public class GroupMover : MonoBehaviour
{
    [SerializeField] public Transform laneOne;
    [SerializeField] public Transform laneTwo;
    [SerializeField] public Transform laneThree;
    [SerializeField] public Transform goal;

    private NavMeshAgent agent;
    private Vector3 target;
    private bool hasReachedLane;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        laneOne = GameObject.FindWithTag("laneOne").GetComponent<Transform>();
        laneTwo = GameObject.FindWithTag("laneTwo").GetComponent<Transform>();
        laneThree = GameObject.FindWithTag("laneThree").GetComponent<Transform>();
        goal = GameObject.FindWithTag("laneOne").GetComponent<Transform>();
        int rand = Random.Range(1, 3);
        if(rand == 1)
        {
            agent.SetDestination(laneOne.position);
            target = laneOne.position;
        }
        else if (rand == 2)
        {
            agent.SetDestination(laneTwo.position);
            target = laneTwo.position;
        }
        else if (rand == 3) 
        {
            agent.SetDestination(laneThree.position);
            target = laneThree.position;
        }
        hasReachedLane = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool reached = !agent.pathPending && agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, 0.1f);
        if (reached && hasReachedLane)
        {
            target = goal.transform.position;
            agent.SetDestination(target);
        } else if (reached && !hasReachedLane)
        {
            hasReachedLane = true;
        }
    }
}
