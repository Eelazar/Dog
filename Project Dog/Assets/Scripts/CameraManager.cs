using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraManager
{
    public static List<CameraObject> cams = new List<CameraObject>();

    public static CameraObject activeCamera;

    public static void Add(CameraObject newCamera)
    {
        cams.Add(newCamera);
    }

    public static void UpdateCameras()
    {
        for (int i = 0; i < cams.Count; i++)
        {
            cams[i].SetState(cams[i] == activeCamera);
        }
    }
}
