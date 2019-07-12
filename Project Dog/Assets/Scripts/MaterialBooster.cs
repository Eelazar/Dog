using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialBooster : MonoBehaviour
{
    Material mat;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetFloat("_ScanPower", Mathf.PingPong(Time.time, 0.5f));
    }
}
