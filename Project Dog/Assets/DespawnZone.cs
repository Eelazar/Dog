using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnZone : MonoBehaviour
{
    public string tag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(tag))
        {
            Destroy(other.gameObject);
        }
    }

}
