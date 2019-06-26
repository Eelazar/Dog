using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ThrowBro_Patrol : MonoBehaviour
{
    public Transform transformA;
    public Transform transformB;

    public float waitTime;

    private GameObject player_Object;
    private Player player_Script;
    private NavMeshAgent navAgent;

    private bool targetA;

    void Start()
    {
        //player_Object = GameObject.FindGameObjectWithTag("Player");
        //player_Script = player_Object.GetComponent<Player>();
        navAgent = gameObject.GetComponent<NavMeshAgent>();

        navAgent.SetDestination(transformA.position);
    }

    void Update()
    {
        if(navAgent.remainingDistance < 0.1F && targetA)
        {
            targetA = !targetA;
            navAgent.SetDestination(transformB.position);
            navAgent.isStopped = true;

            StartCoroutine(Pause());
        }
        else if (navAgent.remainingDistance < 0.1F && !targetA)
        {
            targetA = !targetA;
            navAgent.SetDestination(transformA.position);
            navAgent.isStopped = true;

            StartCoroutine(Pause());
        }
    }

    IEnumerator Pause()
    {
        yield return new WaitForSeconds(waitTime);

        navAgent.isStopped = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transformA.position, 1);

        Gizmos.DrawWireSphere(transformB.position, 1);
    }
}
