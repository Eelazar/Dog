using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public class DogScript : MonoBehaviour
{
    public GameObject master;

    private NavMeshAgent navAgent;

    private GameObject permanentTarget;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if(permanentTarget != null)
        {
            navAgent.SetDestination(permanentTarget.transform.position);
        }
    }

    public void CMDWalk(Vector3 target)
    {
        navAgent.isStopped = false;
        navAgent.SetDestination(target);
    }

    public void CMDFollow(GameObject target)
    {
        navAgent.isStopped = false;
        permanentTarget = target;
    }

    public void CMDStopFollow()
    {
        navAgent.isStopped = true;
        permanentTarget = null;
    }

    public void CMDStop()
    {
        navAgent.isStopped = true;
    }

    public void CMDGo()
    {
        navAgent.isStopped = false;
    }
}
