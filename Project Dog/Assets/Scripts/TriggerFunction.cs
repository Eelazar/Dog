using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TransformEvent : UnityEvent<Transform> { }

public class TriggerFunction : MonoBehaviour
{
    public string triggerTag;

    public TransformEvent functionEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == triggerTag)
        {
            functionEvent.Invoke(other.transform);
        }
    }

}
