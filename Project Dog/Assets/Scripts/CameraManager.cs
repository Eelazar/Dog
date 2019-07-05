using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager current;

    public Camera[] cams;

    public Camera activeCamera;

    public void Awake()
    {
        current = this;

        activeCamera = cams[0];

        for (int i = 0; i < cams.Length; i++)
        {
            BaseObject baseObject = cams[i].GetComponent<BaseObject>();

            ObjectManager.AddObject(baseObject);
        }

        UpdateCameras();
    }

    public void UpdateCameras()
    {
        for (int i = 0; i < cams.Length; i++)
        {
            cams[i].enabled = cams[i] == activeCamera;
        }
    }
}
