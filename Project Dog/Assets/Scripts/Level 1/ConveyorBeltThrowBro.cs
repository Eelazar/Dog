using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltThrowBro : MonoBehaviour
{
    public Vector3 startPosition;

    public Vector3 endPosition;

    public float speed;

    Vector3 fromPos;

    Vector3 toPos;

    bool move;

    private void Start()
    {
        fromPos = toPos = startPosition;
    }

    public void MoveToConveyor()
    {
        if (move)
            return;

        move = true;
        t = 0f;
        fromPos = startPosition;
        toPos = endPosition;

        Invoke("MoveBack", 4f);
    }

    public void MoveBack()
    {
        move = true;
        t = 0f;
        fromPos = endPosition;
        toPos = startPosition;
    }

    float t = 0f;

    private void Update()
    {
        t += Time.deltaTime * speed;

        transform.localPosition = Vector3.Lerp(fromPos, toPos, t);
    }
}
