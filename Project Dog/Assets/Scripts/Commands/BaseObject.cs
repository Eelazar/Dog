using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    public string xmlPath;

    public string definedName;

    private void Awake()
    {
        ObjectManager.AddObject(this);
    }

    public void ScanObject()
    {
        Explorer.current.SwitchXML(xmlPath, definedName);
    }

    public void Unscan()
    {

    }
}
