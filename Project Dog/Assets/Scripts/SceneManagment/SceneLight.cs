using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLight : MonoBehaviour
{
    public string sceneName;

    public GameObject LightingObject;

    private void Start()
    {
        SceneLightManager.Add(this);
    }

    public void Activate()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        LightingObject.SetActive(true);
    }

    public void Deactivate()
    {
        LightingObject.SetActive(false);
    }
}
