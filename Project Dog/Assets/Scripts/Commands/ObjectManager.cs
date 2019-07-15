using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectManager
{
    private static Dictionary<string, BaseObject> objectDictionary = new Dictionary<string, BaseObject>();

    public static bool GetObject(string name, out BaseObject baseObject)
    {
        return objectDictionary.TryGetValue(name, out baseObject);
    }

    public static void AddObject(BaseObject baseObject)
    {
        objectDictionary.Add(baseObject.definedName, baseObject);
    }

    public static void Clear()
    {
        objectDictionary.Clear();
    }
}
