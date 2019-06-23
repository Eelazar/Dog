using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


public class ObjectCommands
{
    public int id;

    [XmlArray("Commands"), XmlArrayItem("Command")]
    public Command[] commands;
}

public static class ObjectCommandManager
{
    private static Dictionary<int, ObjectCommands> idCommandDicitionary = new Dictionary<int, ObjectCommands>();

    public static void AddCommands(ObjectCommands objectCommand)
    {
        idCommandDicitionary.Add(objectCommand.id, objectCommand);
    }

    public static bool IsValidCommand(int id, string action, out string functionCall)
    {
        functionCall = "";

        ObjectCommands objectCommands = null;

        if (idCommandDicitionary.TryGetValue(id, out objectCommands))
        {
            for (int i = 0; i < objectCommands.commands.Length; i++)
            {
                if (objectCommands.commands[i].action.ToUpper().Equals(action.ToUpper()))
                {
                    functionCall = objectCommands.commands[i].functionCall;
                    return true;
                }
            }
        }

        return false;
    }
}
