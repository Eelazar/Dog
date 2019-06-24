using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObject : BaseObject
{
    public Camera camera;

    public void OnCommandOpen(CommandContext context)
    {
        Debug.Log("CAMERA OPEN");

        CameraManager.current.activeCamera = camera;

        CameraManager.current.UpdateCameras();
    }
}
