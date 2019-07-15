using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashTosser : MonoBehaviour
{
    public ThrowBroToss toss;

    public TrashSpawner trashSpawner;

    public void ReleaseTrash()
    {
        trashSpawner.currentTrash.transform.SetParent(null);

        trashSpawner.currentTrash.GetComponent<Rigidbody>().isKinematic = false;

        Invoke("ThrowBroToss", 1f);
    }

    public bool IsFree()
    {
        return toss.rbodies.Count == 0;
    }

    public void ThrowBroToss()
    {
        toss.Toss();
    }
}
