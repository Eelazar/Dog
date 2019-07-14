using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    public AudioSource audio;

    private void OnCollisionEnter(Collision collision)
    {
        ImpactSound impactSound = collision.transform.GetComponent<ImpactSound>();

        if (impactSound)
        {
            Debug.DrawLine(transform.position, Vector3.up * 100f, Color.blue, 100f);
            audio.Play();
        }
    }
}
