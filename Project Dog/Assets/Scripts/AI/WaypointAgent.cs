using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class WaypointAgent : MonoBehaviour
{
    public Waypoint start;

    public Transform raycastPoint;

    public float raycastLength;

    [HideInInspector]
    public Waypoint currentTarget;

    private GameObject player_Object;
    private Player player_Script;
    private NavMeshAgent navAgent;

    private bool targetA;

    public UnityEvent OnNewTarget;

    public UnityEvent OnStop;

    Quaternion targetRotation;

    void Start()
    {
        //player_Object = GameObject.FindGameObjectWithTag("Player");
        //player_Script = player_Object.GetComponent<Player>();
        navAgent = gameObject.GetComponent<NavMeshAgent>();

        currentTarget = start;

        navAgent.updateRotation = false;

        if(currentTarget != null)
        {
            navAgent.SetDestination(currentTarget.transform.position);
        }
    }

    void Update()
    {
        if (navAgent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 velocity = navAgent.velocity.normalized;

            velocity.y = 0f;

            targetRotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity, Vector3.up), navAgent.angularSpeed * 0.1f * Time.deltaTime);
        }
        else
        {
            if (currentTarget != null)
            {
                if (currentTarget.rotateForward)
                    targetRotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.transform.forward.normalized, Vector3.up), navAgent.angularSpeed * 0.1f * Time.deltaTime);
            }                
        }

        transform.rotation = targetRotation;

        if (halting)
        {
            RaycastHit hit;

            if (Physics.Raycast(raycastPoint.position, transform.forward, out hit, raycastLength))
            {
                if (!hit.transform.tag.Equals("Player"))
                {
                    navAgent.isStopped = halting = false;
                }
            }
            else
            {
                navAgent.isStopped = halting = false;
            }
        }
        else if (navAgent.isStopped)
        {
            Wait();
        }
        else
        {
            if (raycastPoint == null)
                return;

            RaycastHit hit;

            if (Physics.Raycast(raycastPoint.position, transform.forward, out hit, raycastLength))
            {
                if (hit.transform.tag.Equals("Player"))
                {
                    Halt();
                }
            }
        }
    }

    bool halting = false;

    public void Halt()
    {
        navAgent.isStopped = halting = true;

        if (currentTarget != null)
        {
            if (currentTarget.waitTime >= 0.5f)
                if (OnStop != null)
                    OnStop.Invoke();
        }            
    }

    public void Stop()
    {
        navAgent.isStopped = true;

        if (currentTarget != null)
        {
            if (currentTarget.waitTime >= 0.5f)
                if (OnStop != null)
                    OnStop.Invoke();
        }            
    }

    void SetNewTarget()
    {
        currentTarget = currentTarget.next;

        if (currentTarget != null)
            navAgent.SetDestination(currentTarget.transform.position);

        if (OnNewTarget != null)
            OnNewTarget.Invoke();
    }

    void Wait()
    {
        if (currentTarget != null)
        {
            if (currentTarget.Continue)
            {
                SetNewTarget();
                navAgent.isStopped = false;
            }
        }
    }
}
