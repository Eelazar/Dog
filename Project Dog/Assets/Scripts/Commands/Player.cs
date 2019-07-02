﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public Vector3 startPosition;

    public Vector3 targetPosition;

    public Vector3 positionDelta;

    public Quaternion startRotation;

    public Quaternion targetRotation;

    public Quaternion rotationDelta;

    public bool hasToRotate;

    public bool hasRotated;

    public bool hasMoved;

    public float angle;

    public Move(Vector3 startPosition, float moveDistance, Quaternion startRotation, Quaternion rotationDelta, float angle)
    {
        this.startRotation = startRotation;
        this.targetRotation = startRotation * rotationDelta;
        this.rotationDelta = rotationDelta;

        this.startPosition = startPosition;
        this.targetPosition = startPosition + (targetRotation * (Vector3.forward * moveDistance));
        this.positionDelta = Vector3.forward * moveDistance;

        this.angle = angle;

        hasToRotate = angle > 1f || angle < 1f;
    }
}

public class Player : BaseObject
{
    Queue<Move> moves = new Queue<Move>();

    Move currentMove;

    Move lastAddedMove;

    public float moveDistance;

    public float positionSpeed;

    public float rotationSpeed;

    public PlayerAnimation playerAnimation;

    private void Awake()
    {

    }

    public void OnCommandMoveFront(CommandContext context)
    {
        int count = 1;
        if (context.paramters.Length > 0)
            count = (int)context.paramters[0];

        AddNewMove(moveDistance * count, Quaternion.identity, 0f);
    }

    public void OnCommandMoveBack(CommandContext context)
    {
        int count = 1;
        if (context.paramters.Length > 0)
            count = (int)context.paramters[0];

        AddNewMove(moveDistance * count, Quaternion.Euler(0f, 180f, 0f), 180f);
    }

    public void OnCommandMoveLeft(CommandContext context)
    {
        int count = 1;
        if (context.paramters.Length > 0)
            count = (int)context.paramters[0];

        AddNewMove(moveDistance * count, Quaternion.Euler(0f, -90f, 0f), -90f);
    }

    public void OnCommandMoveRight(CommandContext context)
    {
        int count = 1;
        if (context.paramters.Length > 0)
            count = (int)context.paramters[0];

        AddNewMove(moveDistance * count, Quaternion.Euler(0f, 90f, 0f), 90f);
    }

    void AddNewMove(float moveDistance, Quaternion rotationDelta, float angle)
    {
        if (currentMove != null)
        {
            Move newMove;
            if (lastAddedMove != null)
            {
                newMove = new Move(lastAddedMove.targetPosition, moveDistance, lastAddedMove.targetRotation, rotationDelta, angle);
            }
            else
            {
                newMove = new Move(currentMove.targetPosition, moveDistance, currentMove.targetRotation, rotationDelta, angle);
            }

            lastAddedMove = newMove;

            moves.Enqueue(lastAddedMove);
        }
        else
        {
            SetCurrentMove(new Move(transform.position, moveDistance, transform.rotation, rotationDelta, angle));
            lastAddedMove = null;
        }
    }

    void SetCurrentMove(Move newCurrent)
    {
        currentMove = newCurrent;

        playerAnimation.Reset();

        if (newCurrent != null)
        {
            positionInterpolation = 0f;
            rotationInterpolation = 0f;

            positionInterpolationStep = positionSpeed / currentMove.positionDelta.magnitude;

            rotationInterpolationStep = rotationSpeed;

            newCurrent.hasRotated = newCurrent.hasToRotate;

            if (newCurrent.angle < -1f)
            {
                playerAnimation.SetLeft(true);
            }
        }
    }

    float positionInterpolation;

    float rotationInterpolation;

    float positionInterpolationStep;

    float rotationInterpolationStep;

    private void Update()
    {
        if (currentMove != null)
        {
            playerAnimation.SetIdle(false);

            if (!currentMove.hasRotated)
            {
                rotationInterpolation += rotationInterpolationStep * Time.deltaTime;

                transform.rotation = Quaternion.Lerp(currentMove.startRotation, currentMove.targetRotation, rotationInterpolation);

                if (rotationInterpolation >= 1f)
                {
                    currentMove.hasRotated = true;
                }
            }

            if (currentMove.hasRotated && !currentMove.hasMoved)
            {
                positionInterpolation += positionInterpolationStep * Time.deltaTime;

                transform.position = Vector3.Lerp(currentMove.startPosition, currentMove.targetPosition, positionInterpolation);

                if (positionInterpolation >= 1f)
                {
                    currentMove.hasMoved = true;
                }
            }

            if (currentMove.hasRotated && currentMove.hasMoved)
            {
                SetCurrentMove(moves.Count > 0 ? moves.Dequeue() : null);
            }
        }
        else
        {
            playerAnimation.SetIdle(true);
        }
    }
}
