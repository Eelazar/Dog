using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : BaseObject
{
    public void OnCommandOpen(CommandContext context)
    {
        Vector3 pos = transform.position;

        pos.y = 1f;

        transform.position = pos;
    }

    public void OnCommandClose(CommandContext context)
    {
        Vector3 pos = transform.position;

        pos.y = 0f;

        transform.position = pos;
    }
}
