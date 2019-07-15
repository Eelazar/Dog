using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ScannedObject
{
    public ScannableObject scannedableObject;

    public float initalDistance;
}

public class ScanManager : MonoBehaviour
{
    public string excludeTag;

    public ParticleSystem particleSystem;

    public float scanTime;

    public float scanRadius;

    public float scanPower;

    public float activeScanPower;

    public ScannableObject currentActive;

    bool activeChanged = false;

    bool scan;

    float scanTimer;

    List<ScannedObject> currentScannedObjects = new List<ScannedObject>();

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

            foreach (ScannedObject currentObject in currentScannedObjects)
            {
                Material mat = currentObject.scannedableObject.GetComponent<Renderer>().material;

                mat.SetFloat("_ScanPower", Mathf.PingPong(Time.time, 1f) * scanPower);
            }

            if (currentActive != null && activeChanged)
            {
                currentActive.baseObject.Unscan();
                activeChanged = false;
                currentActive.baseObject.ScanObject();
            }

            if (currentActive != null)
            {
                Material mat = currentActive.GetComponent<Renderer>().material;

                mat.SetFloat("_ScanPower", Mathf.PingPong(Time.time, 1f) * activeScanPower);
            }

            if (scanTimer <= 0f)
            {
                scan = false;
                foreach (ScannedObject currentObject in currentScannedObjects)
                {
                    Material mat = currentObject.scannedableObject.GetComponent<Renderer>().material;

                    mat.SetFloat("_ScanPower", 0f);
                }

                if (currentActive != null)
                    currentActive.baseObject.Unscan();

                currentActive = null;

                scanTimer = 0f;

                currentScannedObjects.Clear();

                particleSystem.Stop();
            }
        }
    }

    public void Scan()
    {
        particleSystem.Play();
        Invoke("ActuallyScan", 2f);
    }

    public void ExploreOther()
    {
        if (!scan)
            return;

        SwitchContext();
    }

    void ActuallyScan()
    {
        scan = true;
        scanTimer = scanTime;
        ScanForObjects();
        ScanClosestObject();
    }

    void SwitchContext()
    {
        currentIndex++;
        ScanClosestObject();
    }

    int currentIndex = 0;

    void ScanClosestObject()
    {
        currentIndex = (currentIndex < currentScannedObjects.Count) ? currentIndex : 0;

        Debug.Log(currentIndex);

        currentActive = currentScannedObjects[currentIndex].scannedableObject;
        activeChanged = true;
    }

    void ScanForObjects()
    {
        currentScannedObjects.Clear();

        Collider[] objects = Physics.OverlapSphere(transform.position, scanRadius);

        foreach (Collider currentObject in objects)
        {
            ScannableObject relay = currentObject.transform.GetComponent<ScannableObject>();
            if (relay != null)
            {
                float distance = Vector3.Distance(currentObject.transform.position, transform.position);

                currentScannedObjects.Add(new ScannedObject() { scannedableObject = relay, initalDistance = distance });
            }
        }

        currentScannedObjects.Sort((a, b) => a.initalDistance > b.initalDistance ? 1 : -1);
    }
}
