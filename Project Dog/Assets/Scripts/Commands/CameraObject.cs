using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObject : BaseObject
{
    public string sceneName;

    GameObject camera;

    private bool cameraActivated = false;
    private TempManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<TempManager>();
        camera = transform.GetChild(0).gameObject;

        CameraManager.Add(this);
    }

    public void OnCommandActivate(CommandContext context)
    {
        CameraManager.activeCamera = this;

        CameraManager.UpdateCameras();

        SceneLightManager.Activate(sceneName);

        if (definedName == "cam1" || definedName == "cam2")
        {
            manager.LaunchFoxStartup();
            cameraActivated = true;
        }
    }

    public void SetState(bool state)
    {
        camera.SetActive(state);
    }
}
