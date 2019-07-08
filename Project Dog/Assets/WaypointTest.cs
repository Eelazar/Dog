using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointTest : MonoBehaviour
{
    public Transform box;

    Vector3 startPosition;

    Vector3 endPosition;

    public AnimationCurve curve;

    public void TestStartBox()
    {
        startPosition = box.position;

        endPosition = startPosition + Vector3.up * 6f;
    }

    public void TestWaitBox(float normalizedtime)
    {
        Debug.Log(normalizedtime);

        box.position = Vector3.Lerp(startPosition, endPosition, curve.Evaluate(normalizedtime));
    }

    public void TestEndBox()
    {
        box.position = startPosition;
    }
}
