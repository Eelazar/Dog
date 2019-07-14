using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBroToss : MonoBehaviour
{
    public Animator animator;

    public List<Rigidbody> rbodies;

    public Vector3 tossDirection;

    public float force;

    private void OnTriggerEnter(Collider other)
    {
        rbodies.Add(other.attachedRigidbody);
    }

    private void OnTriggerExit(Collider other)
    {
        rbodies.Remove(other.attachedRigidbody);
    }

    public void Toss()
    {
        animator.SetBool("Toss", true);

        foreach (Rigidbody rbody in rbodies)
        {
            rbody.AddForce(transform.localToWorldMatrix * tossDirection.normalized * force, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + (transform.rotation * tossDirection), Color.red);
    }
}
