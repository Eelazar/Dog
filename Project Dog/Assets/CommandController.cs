using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct KeyCommand
{
    public KeyCode key;
    public string function;
}

public class CommandController : MonoBehaviour
{
    public KeyCommand[] commands;

    private void Update()
    {
        foreach (KeyCommand command in commands)
        {
            if (Input.GetKeyDown(command.key))
            {
                SendMessage(command.function, new CommandContext() { paramters = new object[] { 1 } });
            }
        }
    }
}
