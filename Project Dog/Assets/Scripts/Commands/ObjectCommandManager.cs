using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


public class ObjectCommands
{
    public string baseName;

    [XmlArray("Commands"), XmlArrayItem("Command")]
    public Command[] commands;
}

public static class ObjectCommandManager
{
    private static Dictionary<string, ObjectCommands> baseNameCommandDicitionary = new Dictionary<string, ObjectCommands>();

    public static void AddCommands(ObjectCommands objectCommand)
    {
        baseNameCommandDicitionary.Add(objectCommand.baseName, objectCommand);
    }

    public static bool IsValidCommand(string baseName, string[] words, out string[] actions, out string functionCall, out int count)
    {
        functionCall = "";

        ObjectCommands objectCommands = null;

        count = 1;

        int bestFit = 0;

        int currentFit = 0;

        bool fullCommand = false;

        actions = new string[0];

        if (baseNameCommandDicitionary.TryGetValue(baseName, out objectCommands))
        {
            for (int i = 0; i < objectCommands.commands.Length; i++)
            {
                currentFit = 0;
                Command command = objectCommands.commands[i];

                int start = 0;
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
                        }

                        if (currentFit == command.action.Length)
                        {
                            int n;
                            bool isNumeric = int.TryParse(words[k], out n);

                            if (isNumeric)
                            {
                                count = n;
                                break;
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
