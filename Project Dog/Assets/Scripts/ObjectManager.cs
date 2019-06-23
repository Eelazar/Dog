using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectManager
{
    private static Dictionary<string, BaseObject> objectDictionary = new Dictionary<string, BaseObject>();

    public static BaseObject GetObject(string name)
    {
        BaseObject baseObject = null;

        objectDictionary.TryGetValue(name, out baseObject);

        return baseObject;
    }

    public static void AddObject(string name, BaseObject baseObject)
    {
        objectDictionary.Add(name, baseObject);
    }
}
