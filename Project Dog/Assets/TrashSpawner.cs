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

    public void Spawn()
    {
        currentTrash = Instantiate(Trash, spawnPoint);

        Invoke("Parent", 3f);
    }

    public void Parent()
    {
        currentTrash.transform.SetParent(dropPoint.agent.transform);
        currentTrash.GetComponent<Animator>().enabled = false;
    }
}
