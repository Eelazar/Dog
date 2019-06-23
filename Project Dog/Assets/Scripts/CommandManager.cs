using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Command
{
    public string action;

    public string functionCall;
}

public class CommandContext
{
    public BaseObject sender;

    public BaseObject receiver;

    public string command;
}

public class CommandFeedback
{
    public bool valid;

    public string feedback;
}

public static class CommandManager
{
    public static CommandFeedback ExecuteCommand(string action, string objectName)
    {
        BaseObject baseObject = ObjectManager.GetObject(objectName);

        if (baseObject != null)
        {
            string functionCall;
            if (ObjectCommandManager.IsValidCommand(baseObject.id, action, out functionCall))
            {
                GameManager.current.player.SendMessage(functionCall, new CommandContext() { sender = GameManager.current.player, receiver = baseObject, command = action });

                baseObject.SendMessage(functionCall, new CommandContext() { sender = GameManager.current.player, receiver = baseObject, command = action });
                return new CommandFeedback() { valid = true };
            }
            return new CommandFeedback() { valid = false, feedback = "Can not " + action + " " + objectName + "!" };
        }
        else
        {
            return new CommandFeedback() { valid = false, feedback = "No Object called " + objectName + " found!" };
        }
    }
}
