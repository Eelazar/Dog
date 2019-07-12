using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanManager : MonoBehaviour
{
    public ParticleSystem particleSystem;

    public float scanTime;

    public float scanRadius;

    public float scanInterval;

    bool scan;

    float scanTimer;

    float scanIntervalTimer;

    List<GameObject> currentScannedObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        particleSystem.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (scan)
        {
            scanTimer -= Time.deltaTime;

            scanIntervalTimer += Time.deltaTime;

            if (scanIntervalTimer >= scanInterval)
            {
                ScanForObjects();
                scanIntervalTimer = 0f;
            }

            foreach (GameObject currentObject in currentScannedObjects)
            {
                Material mat = currentObject.GetComponent<Renderer>().material;

                mat.SetFloat("_ScanPower", Mathf.PingPong(Time.time, 0.5f));
            }

            if (scanTimer <= 0f)
            {
                scan = false;
                particleSystem.Stop();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !scan)
        {
            Scan();
        }
    }

    public void Scan()
    {
        particleSystem.Play();
        scan = true;
        scanTimer = scanTime;
    }

    List<GameObject> remove = new List<GameObject>();

    void ScanForObjects()
    {
        Collider[] objects = Physics.OverlapSphere(transform.position, scanRadius);

        remove.Clear();
        //Remove
        foreach (GameObject currentObject in currentScannedObjects)
        {
            bool found = false;

            for (int i = 0; i < objects.Length; i++)
            {
                if (currentObject.Equals(objects[i].GetComponent<Collider>()))
                {
                    found = true;
                }
            }

            if (!found)
            {
                remove.Add(currentObject);
            }
        }

        foreach (GameObject currentObject in remove)
        {
            // Reset Effect


            currentScannedObjects.Remove(currentObject);
        }

        foreach (Collider currentObject in objects)
        {
            if (currentObject.transform.root.GetComponent<BaseObject>() != null)
            {
                if (!currentScannedObjects.Contains(currentObject.gameObject))
                {
                    currentScannedObjects.Add(currentObject.gameObject);
                }
            }
        }
    }
}
