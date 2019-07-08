using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ThrowBro_Wander : MonoBehaviour
{
    public float walkRadius;

    private GameObject player_Object;
    private Player player_Script;
    private NavMeshAgent navAgent;

    private Vector3 currentTarget;
    //private Vector3 newTarget;

    void Start()
    {
        //player_Object = GameObject.FindGameObjectWithTag("Player");
        //player_Script = player_Object.GetComponent<Player>();
        navAgent = gameObject.GetComponent<NavMeshAgent>();

        currentTarget = FindRandomPointOnNav();
    }

    void Update()
    {
        if (navAgent.remainingDistance < 0.1F)
        {
            currentTarget = FindRandomPointOnNav();
        }

        navAgent.SetDestination(currentTarget);


    }

    Vector3 FindRandomPointOnNav()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;

        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        Vector3 finalPosition = hit.position;

        return finalPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, walkRadius);

        Gizmos.DrawWireSphere(currentTarget, 1);
    }
}
