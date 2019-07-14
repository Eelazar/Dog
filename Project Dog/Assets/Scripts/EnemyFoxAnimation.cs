using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFoxAnimation : MonoBehaviour
{
    public WaypointAgent agent;

    public Animator animator;

    public void Move()
    {
        animator.SetBool("Idle", false);
    }

    public void Stop()
    {

        animator.SetBool("Idle", true);
    }
}
