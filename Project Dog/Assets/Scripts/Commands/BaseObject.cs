using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    private string xmlPath = "XML\\ThrowBroExplorerFile.xml";

    public string definedName;

    private void Awake()
    {
        ObjectManager.AddObject(this);
    }

    public void Scan()
    {
        Explorer.current.SwitchXML(xmlPath);
    }

    public void Unscan()
    {

    }
}
