using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public GameObject Trash;

    public Transform spawnPoint;

    public Waypoint dropPoint;

    public TrashTosser tosser;

    public GameObject currentTrash;

    public float parentTimer;

    public void Spawn()
    {
        Debug.Log("SPAWN");

        if (tosser.IsFree())
        {
            currentTrash = Instantiate(Trash, spawnPoint);

            Invoke("Parent", parentTimer);
        }
    }

    public void Parent()
    {
        currentTrash.transform.SetParent(dropPoint.agent.transform);
        dropPoint.SetContinueWaypoint();
    }
}
