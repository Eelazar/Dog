using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class WaypointEvent : UnityEvent<float> { };

public class Waypoint : MonoBehaviour
{
    public Waypoint next;

    public int waitTime;

    public WaypointAgent agent;

    public UnityEvent startFunction;

    public WaypointEvent waitFunction;

    public UnityEvent endFunction;

    private bool continueWaypoint;

    public bool Continue
    {
        get
        {
            return continueWaypoint;
        }
    }

    public void SetContinueWaypoint()
    {
        continueWaypoint = true;
    }

    float waitTimer;

    bool wait;

    public bool IsAtWaypoint()
    {
        float distance = Vector3.Distance(transform.position, agent.transform.position);

        return distance < 0.05f;
    }

    private void Update()
    {
        if (IsAtWaypoint() && !wait && waitTimer <= -1f)
        {
            if (startFunction != null)
                startFunction.Invoke();


            agent.Stop();

            waitTimer = waitTime;

            continueWaypoint = false;

            wait = true;
        }

        waitTimer -= Time.deltaTime;

        Debug.Log(waitTimer);

        if (wait)
        {
            if (waitFunction != null)
                waitFunction.Invoke(waitTimer / (float)waitTime);

            if (waitTimer <= 0f)
            {
                if (endFunction != null)
                    endFunction.Invoke();

                continueWaypoint = true;

                wait = false;
            }
        }
    }

}
