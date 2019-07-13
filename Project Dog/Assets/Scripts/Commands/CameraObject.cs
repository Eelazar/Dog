using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObject : BaseObject
{
    public string sceneName;

    GameObject camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = transform.GetChild(0).gameObject;

        CameraManager.Add(this);
    }

    public void OnCommandActivate(CommandContext context)
    {
        CameraManager.activeCamera = this;

        CameraManager.UpdateCameras();

        SceneLightManager.Activate(sceneName);
    }

    public void SetState(bool state)
    {
        Debug.Log(state);

        camera.SetActive(state);
    }
}
