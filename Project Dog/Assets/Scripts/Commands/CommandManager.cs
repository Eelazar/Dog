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

    public object[] parameters;
}

public class CommandFeedback
{
    public bool valid;

    public string feedback;
}

public static class CommandManager
{

    //Exectute a Command
    //raw words is an array of all words in the command
    public static CommandFeedback ExecuteCommand(string[] rawWords)
    {
        bool objectFound = false;

        BaseObject baseObject = null;

        //iterate through all words and try to look for an object
        for (int i = 0; i < rawWords.Length; i++)
        {
            //Check if this word is an object
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

            //The Command has no defined object to interact with
            if (!actionFound)
                return new CommandFeedback() { valid = false, feedback = "Unknown Object" };
        }
        else
        {
            //iterate through all words and try to look for an action that fits the object
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

        //Call function on the player object
        GameManager.current.player.SendMessage(functionCall, new CommandContext() { sender = GameManager.current.player, receiver = baseObject, command = actions, parameters = paramters });

        //if an object was found call the function on the object
        if (objectFound)
            baseObject.SendMessage(functionCall, new CommandContext() { sender = GameManager.current.player, receiver = baseObject, command = actions });

        return new CommandFeedback() { valid = true, feedback = "Success" };

    }
}
