using System;
using System.Collections.Generic;
using UnityEngine;


public class LockRotation : MonoBehaviour
{

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "trash")
        {
            other.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
}
