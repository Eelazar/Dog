using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneLightManager
{
    static List<SceneLight> sceneLights = new List<SceneLight>();

    public static void Add(SceneLight newLight)
    {
        sceneLights.Add(newLight);
    }

    public static void Activate(string sceneName)
    {
        for (int i = 0; i < sceneLights.Count; i++)
        {
            if (sceneLights[i].sceneName == sceneName)
            {
                sceneLights[i].Activate();
            }
            else
            {
                sceneLights[i].Deactivate();
            }
        }
    }
}
