using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


// Data structure just to easily serialize the xml file. (XML File is a straight mirror of this class)
public class ObjectCommands
{
    public string baseName;

    [XmlArray("Commands"), XmlArrayItem("Command")]
    public Command[] commands;
}

//CommandManager holds all available Commands for an Object
public static class ObjectCommandManager
{
    //Used to look up the commands for a certain type of Object (indexed by name)
    private static Dictionary<string, ObjectCommands> baseNameCommandDicitionary = new Dictionary<string, ObjectCommands>();

    public static void AddCommands(ObjectCommands objectCommand)
    {
        baseNameCommandDicitionary.Add(objectCommand.baseName, objectCommand);
    }

    //Checks if a command string is a valid command for a given object
    //baseName is the class name of the object, not the object-specific name ("Door" or "ThrowBro")
    //words is each word out of the command {"Open","the","door"}
    //actions is an array of actions connected to the command, if the command is valid the keywords connected to this action are in this array, empty if not valid
    //function call is the actual Command that is called on the MonoBehaviour "OnCommandOpen"
    //parameters is a list of objects (int, float, string etc) that is passed to the command function
    public static bool IsValidCommand(string baseName, string[] words, out string[] actions, out string functionCall, out object[] paramters)
    {
        functionCall = "";

        ObjectCommands objectCommands = null;

        int bestFit = 0;

        int currentFit = 0;

        bool fullCommand = false;

        actions = new string[0];

        paramters = new object[0];

        //look for object
        if (baseNameCommandDicitionary.TryGetValue(baseName, out objectCommands))
        {
            for (int i = 0; i < objectCommands.commands.Length; i++)
            {
                currentFit = 0;
                Command command = objectCommands.commands[i];

                int start = 0;

                int paramterStart = 0;
                for (int j = 0; j < command.action.Length; j++)
                {
                    for (int k = start; k < words.Length; k++)
                    {
                        if (words[k].ToUpper().Equals(command.action[j].ToUpper()))
                        {
                            start = k;
                            currentFit++;
                            if (currentFit != command.action.Length)
                                break;

                            if (currentFit == command.action.Length)
                                continue;
                        }

                        if (currentFit == command.action.Length)
                        {
                            paramters = new object[command.paramters.Length];

                            for (int h = paramterStart; h < command.paramters.Length; h++)
                            {
                                paramters[h] = ParameterConverter.ConvertInt(words[k]);
                                paramterStart++;
                            }
                        }
                    }
                }

                if (currentFit > bestFit)
                {
                    bestFit = currentFit;
                    fullCommand = currentFit == command.action.Length;
                    functionCall = command.functionCall;
                    actions = command.action;
                }
            }
        }

        return fullCommand;
    }
}
