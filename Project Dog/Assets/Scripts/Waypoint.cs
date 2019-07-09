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

    public UnityEvent startFunction;

    public WaypointEvent waitFunction;

    public UnityEvent endFunction;
}
