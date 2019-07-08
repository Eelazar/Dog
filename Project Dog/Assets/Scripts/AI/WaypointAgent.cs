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
        else
        {
            if (navAgent.remainingDistance < 0.1f)
            {
                Debug.Log(currentTarget.startFunction != null);

                if (currentTarget.startFunction != null)
                    currentTarget.startFunction.Invoke();

                waitTimer = currentTarget.waitTime;

                navAgent.isStopped = true;
            }
        }
    }

    void SetNewTarget()
    {
        Debug.Log(currentTarget.endFunction != null);

        if (currentTarget.endFunction != null)
            currentTarget.endFunction.Invoke();

        currentTarget = currentTarget.next;

        if (currentTarget != null)
            navAgent.SetDestination(currentTarget.transform.position);
    }

    float waitTimer;

    void Wait()
    {
        waitTimer -= Time.deltaTime;

        if (currentTarget.waitFunction != null)
            currentTarget.waitFunction.Invoke((currentTarget.waitTime - waitTimer) / currentTarget.waitTime);

        if (waitTimer <= 0f)
        {
            SetNewTarget();
            navAgent.isStopped = false;
        }
    }
}
