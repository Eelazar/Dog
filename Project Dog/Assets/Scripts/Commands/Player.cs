using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BaseObject
{
    public Vector3 toPosition;

    public Quaternion toRotation;

    public float speed;

    private void Awake()
    {
        toPosition = transform.position;
    }

    public void OnCommandMoveFront(CommandContext context)
    {
        toPosition += (transform.forward * 1);
    }

    public void OnCommandMoveBack(CommandContext context)
    {
        toPosition += (-transform.forward * 1);

        toRotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
    }

    public void OnCommandMoveLeft(CommandContext context)
    {
        toPosition += (-transform.right * 1);

        toRotation = Quaternion.LookRotation(-transform.right, Vector3.up);
    }

    public void OnCommandMoveRight(CommandContext context)
    {
        toPosition += (transform.right * 1);

        toRotation = Quaternion.LookRotation(transform.right, Vector3.up);
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, toPosition, Time.deltaTime * speed);

        //Gibt Error raus
        //transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * speed);
    }
}
