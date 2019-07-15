using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanManager : MonoBehaviour
{
    public ParticleSystem particleSystem;

    public float scanTime;

    public float scanRadius;

    public float scanInterval;

    public float scanPower;

    public float activeScanPower;

    public ScannableObject currentActive;

    bool activeChanged = false;

    bool scan;

    float scanTimer;

    float scanIntervalTimer;

    List<ScannableObject> currentScannedObjects = new List<ScannableObject>();

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

            foreach (ScannableObject currentObject in currentScannedObjects)
            {
                Material mat = currentObject.GetComponent<Renderer>().material;

                mat.SetFloat("_ScanPower", Mathf.PingPong(Time.time, scanPower));
            }

            if (currentActive != null && activeChanged)
            {
                currentActive.baseObject.Scan();
                activeChanged = false;
            }

            if (currentActive != null)
            {
                Material mat = currentActive.GetComponent<Renderer>().material;

                mat.SetFloat("_ScanPower", Mathf.PingPong(Time.time, activeScanPower));
            }

            if (scanTimer <= 0f)
            {
                scan = false;
                foreach (ScannableObject currentObject in currentScannedObjects)
                {
                    Material mat = currentObject.GetComponent<Renderer>().material;

                    mat.SetFloat("_ScanPower", 0f);
                }

                currentActive.baseObject.Unscan();

                currentActive = null;

                currentScannedObjects.Clear();

                particleSystem.Stop();
            }
        }

        if (Input.GetKeyDown(KeyCode.F1) && !scan)
        {
            particleSystem.Play();
            Invoke("Scan", 2f);
        }
    }

    public void Scan()
    {
        scan = true;
        scanTimer = scanTime;
    }

    List<ScannableObject> remove = new List<ScannableObject>();

    void ScanForObjects()
    {
        Collider[] objects = Physics.OverlapSphere(transform.position, scanRadius);

        remove.Clear();
        //Remove
        foreach (ScannableObject currentObject in currentScannedObjects)
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

        foreach (ScannableObject currentObject in remove)
        {
            // Reset Effect

            Material mat = currentObject.GetComponent<Renderer>().material;

            mat.SetFloat("_ScanPower", 0f);

            currentScannedObjects.Remove(currentObject);
        }

        float currentDistance = float.MaxValue;

        ScannableObject current = null;

        foreach (Collider currentObject in objects)
        {
            ScannableObject relay = currentObject.transform.GetComponent<ScannableObject>();
            if (relay != null)
            {
                float distance = Vector3.Distance(currentObject.transform.position, transform.position);

                if (distance < currentDistance)
                {
                    currentDistance = distance;
                    current = relay;
                }

                if (!currentScannedObjects.Contains(relay))
                {
                    currentScannedObjects.Add(relay);
                }
            }
        }

        if (currentActive == null)
        {
            activeChanged = true;
        }
        else
        {
            if (!currentActive.Equals(current))
            {
                activeChanged = true;
            }
        }

        currentActive = current;
    }
}
