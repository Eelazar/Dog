using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashTosser : MonoBehaviour
{
    public TrashSpawner trashSpawner;

    public void ReleaseTrash()
    {
        trashSpawner.currentTrash.transform.SetParent(null);

        trashSpawner.currentTrash.GetComponent<Rigidbody>().isKinematic = false;
    }
}
