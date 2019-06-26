using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


public class Command
{
    [XmlArray("Actions"), XmlArrayItem("Action")]
    public string[] action;

    public string functionCall;

    [XmlArray("Parameters"), XmlArrayItem("Parameter")]
    public string[] paramters;
}

public class CommandContext
{
    public BaseObject sender;

    public BaseObject receiver;

    public string[] command;

    public object[] paramters;
}

public class CommandFeedback
{
    public bool valid;

    public string feedback;
}

public static class CommandManager
{

    public static CommandFeedback ExecuteCommand(string[] rawWords)
    {
        bool objectFound = false;

        BaseObject baseObject = null;

        for (int i = 0; i < rawWords.Length; i++)
        {
            if (ObjectManager.GetObject(rawWords[i], out baseObject))
            {
                objectFound = true;
                break;
            }
        }

        bool actionFound = false;

        string[] actions = new string[0];

        object[] paramters = new object[0];

        string functionCall = "";

        if (!objectFound)
        {
            //Object not found
            //Check if its a Player Command
            if (ObjectCommandManager.IsValidCommand(GameManager.current.player.baseName, rawWords, out actions, out functionCall, out paramters))
            {
                actionFound = true;
            }

            if (!actionFound)
                return new CommandFeedback() { valid = false, feedback = "Unknown Object" };
        }
        else
        {
            for (int i = 0; i < rawWords.Length; i++)
            {
                if (ObjectCommandManager.IsValidCommand(baseObject.baseName, rawWords, out actions, out functionCall, out paramters))
                {
                    actionFound = true;
                    break;
                }
            }
        }

        if (!actionFound)
        {
            //Action not found
            return new CommandFeedback() { valid = false, feedback = "Unknown Action" };
        }

        GameManager.current.player.SendMessage(functionCall, new CommandContext() { sender = GameManager.current.player, receiver = baseObject, command = actions, paramters = paramters });

        if (objectFound)
            baseObject.SendMessage(functionCall, new CommandContext() { sender = GameManager.current.player, receiver = baseObject, command = actions });

        return new CommandFeedback() { valid = true, feedback = "Suceccss" };

    }
}
