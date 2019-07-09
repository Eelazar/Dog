using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WaypointAgent : MonoBehaviour
{
    public Waypoint start;

    Waypoint currentTarget;

    private GameObject player_Object;
    private Player player_Script;
    private NavMeshAgent navAgent;

    private bool targetA;

    void Start()
    {
        //player_Object = GameObject.FindGameObjectWithTag("Player");
        //player_Script = player_Object.GetComponent<Player>();
        navAgent = gameObject.GetComponent<NavMeshAgent>();

        currentTarget = start;

        navAgent.SetDestination(currentTarget.transform.position);
    }

    void Update()
    {
        if (navAgent.isStopped)
        {
            Wait();
        }
    }

    public void Stop()
    {
        navAgent.isStopped = true;
    }

    void SetNewTarget()
    {
        currentTarget = currentTarget.next;

        if (currentTarget != null)
            navAgent.SetDestination(currentTarget.transform.position);
    }

    float waitTimer;

    void Wait()
    {
        if (currentTarget.Continue)
        {
            SetNewTarget();
            navAgent.isStopped = false;
        }
    }
}
