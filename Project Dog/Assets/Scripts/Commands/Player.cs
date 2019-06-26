using System.Collections;
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

    public bool hastoRotate;

    public bool hasRotated;

    public bool hasMoved;

    public Move(Vector3 startPosition, float moveDistance, Quaternion startRotation, Quaternion rotationDelta)
    {
        this.startRotation = startRotation;
        this.targetRotation = startRotation * rotationDelta;
        this.rotationDelta = rotationDelta;

        this.startPosition = startPosition;
        this.targetPosition = startPosition + (targetRotation * (Vector3.forward * moveDistance));
        this.positionDelta = Vector3.forward * moveDistance;
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

    bool hasToTurn;

    private void Awake()
    {

    }

    public void OnCommandMoveFront(CommandContext context)
    {
        int count = 1;
        if (context.paramters.Length > 0)
            count = (int)context.paramters[0];

        AddNewMove(moveDistance * count, Quaternion.identity);
    }

    public void OnCommandMoveBack(CommandContext context)
    {
        int count = 1;
        if (context.paramters.Length > 0)
            count = (int)context.paramters[0];

        AddNewMove(moveDistance * count, Quaternion.Euler(0f, 180f, 0f));
    }

    public void OnCommandMoveLeft(CommandContext context)
    {
        int count = 1;
        if (context.paramters.Length > 0)
            count = (int)context.paramters[0];

        AddNewMove(moveDistance * count, Quaternion.Euler(0f, -90f, 0f));
    }

    public void OnCommandMoveRight(CommandContext context)
    {
        int count = 1;
        if (context.paramters.Length > 0)
            count = (int)context.paramters[0];

        AddNewMove(moveDistance * count, Quaternion.Euler(0f, 90f, 0f));
    }

    void AddNewMove(float moveDistance, Quaternion rotationDelta)
    {
        if (currentMove != null)
        {
            Move newMove;
            if (lastAddedMove != null)
            {
                newMove = new Move(lastAddedMove.targetPosition, moveDistance, lastAddedMove.targetRotation, rotationDelta);
            }
            else
            {
                newMove = new Move(currentMove.targetPosition, moveDistance, currentMove.targetRotation, rotationDelta);
            }

            lastAddedMove = newMove;

            moves.Enqueue(lastAddedMove);
        }
        else
        {
            SetCurrentMove(new Move(transform.position, moveDistance, transform.rotation, rotationDelta));
            lastAddedMove = null;
        }
    }

    void SetCurrentMove(Move newCurrent)
    {
        currentMove = newCurrent;

        if (newCurrent != null)
        {
            positionInterpolation = 0f;
            rotationInterpolation = 0f;

            positionInterpolationStep = positionSpeed / currentMove.positionDelta.magnitude;

            rotationInterpolationStep = rotationSpeed;
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
            if (!currentMove.hasRotated)
            {
                rotationInterpolation += rotationInterpolationStep * Time.deltaTime;

                transform.rotation = Quaternion.Lerp(currentMove.startRotation, currentMove.targetRotation, rotationInterpolation);

                Debug.Log(rotationInterpolation);

                if (rotationInterpolation >= 1f)
                {
                    currentMove.hasRotated = true;
                }
            }

            if (currentMove.hasRotated && !currentMove.hasMoved)
            {
                positionInterpolation += positionInterpolationStep * Time.deltaTime;

                Debug.Log(positionInterpolation);

                transform.position = Vector3.Lerp(currentMove.startPosition, currentMove.targetPosition, positionInterpolation);

                if (positionInterpolation >= 1f)
                {
                    currentMove.hasMoved = true;
                }
            }

            if (currentMove.hasMoved && currentMove.hasMoved)
            {
                SetCurrentMove(moves.Count > 0 ? moves.Dequeue() : null);
            }
        }
    }
}
