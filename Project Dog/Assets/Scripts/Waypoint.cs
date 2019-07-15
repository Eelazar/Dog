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

    public bool rotateForward;

    public WaypointAgent agent;

    public UnityEvent startFunction;

    public WaypointEvent waitFunction;

    public UnityEvent endFunction;

    public bool continueWaypoint;

    public bool waitForSignal;

    public bool Continue
    {
        get
        {
            return continueWaypoint;
        }
    }

    public void SetContinueWaypoint()
    {
        if (endFunction != null)
            endFunction.Invoke();

        continueWaypoint = true;

        wait = false;

        waitTimer = 0f;
    }

    public void ResetContinue()
    {
        continueWaypoint = false;
    }

    float waitTimer;

    bool wait;

    public bool IsAtWaypoint()
    {
        Vector3 pos = transform.position;

        pos.y = agent.transform.position.y;

        float distance = Vector3.Distance(pos, agent.transform.position);
        return distance < 0.25f;
    }

    private void Update()
    {
        if (!agent.currentTarget.Equals(this))
            return;

        if (IsAtWaypoint() && !wait && waitTimer <= -1f)
        {

            if (startFunction != null)
                startFunction.Invoke();

            agent.Stop();

            waitTimer = waitTime;

            if (!waitForSignal)
                continueWaypoint = false;

            wait = true;
        }

        waitTimer -= Time.deltaTime;

        if (wait)
        {
            if (waitFunction != null)
                waitFunction.Invoke(waitTimer / (float)waitTime);

            if (!waitForSignal && waitTimer <= 0f)
            {
                if (endFunction != null)
                    endFunction.Invoke();

                continueWaypoint = true;

                wait = false;
            }
        }
    }

}
