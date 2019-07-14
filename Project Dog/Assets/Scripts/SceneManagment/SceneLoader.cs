using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string[] scenesToLoad;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < scenesToLoad.Length; i++)
        {
            SceneManager.LoadSceneAsync(scenesToLoad[i], LoadSceneMode.Additive);
        }
    }
}
