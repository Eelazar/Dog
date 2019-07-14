using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ConveyorBeltObject
{
    public Rigidbody rbody;
    public float force;
}
public class ConveyorBelt : MonoBehaviour
{
    public List<ConveyorBeltObject> rbodies = new List<ConveyorBeltObject>();

    public Vector3 forceDirection;

    public float force;

    private void OnTriggerEnter(Collider other)
    {
        ConveyorBeltObject conveyorBeltObject = new ConveyorBeltObject();
        conveyorBeltObject.rbody = other.attachedRigidbody;
        conveyorBeltObject.force = 0f;

        rbodies.Add(conveyorBeltObject);
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < rbodies.Count; i++)
        {
            ConveyorBeltObject conveyorBeltObject = rbodies[i];
            if (conveyorBeltObject.rbody.Equals(other.attachedRigidbody))
            {
                rbodies.Remove(conveyorBeltObject);
                break;
            }
        }
    }

    public void Push()
    {
        for (int i = 0; i < rbodies.Count; i++)
        {
            ConveyorBeltObject conveyorBeltObject = rbodies[i];

            Vector3 worldVelocity = conveyorBeltObject.rbody.velocity;

            Vector3 worldForceDirection = (transform.rotation * forceDirection) * force;

            Vector3 velocityAlongDirection = Vector3.Project(worldVelocity, worldForceDirection);

            Vector3 nearestPointOnBelt = transform.position + Vector3.Project((conveyorBeltObject.rbody.transform.position - transform.position), transform.rotation * forceDirection);

            conveyorBeltObject.rbody.AddForce((nearestPointOnBelt - conveyorBeltObject.rbody.transform.position) * conveyorBeltObject.force, ForceMode.Force);

            Debug.DrawLine(nearestPointOnBelt, conveyorBeltObject.rbody.transform.position, Color.green);

            if (velocityAlongDirection.sqrMagnitude < force * force)
                conveyorBeltObject.rbody.AddForce(transform.localToWorldMatrix * forceDirection.normalized * force * conveyorBeltObject.force, ForceMode.Force);

            conveyorBeltObject.force = Mathf.Clamp(conveyorBeltObject.force + Time.fixedDeltaTime, 0f, 1f);

            rbodies[i] = conveyorBeltObject;
        }
    }

    private void FixedUpdate()
    {
        Push();
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + (transform.rotation * forceDirection), Color.red);
    }
}
