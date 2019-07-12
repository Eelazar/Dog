using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    public string baseName;

    public string definedName;

    private void Awake()
    {
        ObjectManager.AddObject(this);
    }
}
