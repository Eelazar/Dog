using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBroToss : MonoBehaviour
{
    public AudioSource audio;

    public Animator animator;

    public Collider platformCollider;

    public List<Rigidbody> rbodies;

    public Vector3 tossDirection;

    public float force;

    bool canToss = true;

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
        if (!canToss)
            return;

        if (platformCollider != null)
            platformCollider.isTrigger = true;

        animator.SetBool("Toss", true);

        foreach (Rigidbody rbody in rbodies)
        {
            rbody.AddForce(transform.localToWorldMatrix * tossDirection.normalized * force, ForceMode.Impulse);
        }

        audio.Play();

        Invoke("ResetPlatformCollider", 1.5f);
    }

    void ResetPlatformCollider()
    {
        if (platformCollider != null)
            platformCollider.isTrigger = false;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + (transform.rotation * tossDirection), Color.red);
    }

    public void enabletoss()
    {
        canToss = true;
    }

    public void disabletoss()
    {
        canToss = false;
    }

    public void throwbrotoss()
    {
        canToss = true;
        Toss();
    }
}
