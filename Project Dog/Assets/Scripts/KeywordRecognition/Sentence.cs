using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sentence
{
    public string action;

    public string[] objects;

    public void IsSentence(string[] words, out KeywordRecognitionResult result)
    {
        result = new KeywordRecognitionResult();

        string[] actionParts = action.Split(' ');

        bool actionFound = false;

        result.matched = false;

        bool actionObjectFound = false;

        string actionObject = "";

        for (int j = 0; j < words.Length; j++)
        {
            if (actionFound)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (words[j].ToUpper().Equals(objects[i].ToUpper()))
                    {
                        actionObjectFound = true;
                        actionObject = objects[i];
                    }
                }
            }
            else
            {
                if (words[j].ToUpper().Equals(actionParts[0].ToUpper()))
                {
                    bool fits = true;

                    for (int x = 1; x < actionParts.Length; x++)
                    {
                        if (j + x >= words.Length)
                        {
                            fits = false;
                            break;
                        }

                        if (!words[j + x].ToUpper().Equals(actionParts[x].ToUpper()))
                        {
                            fits = false;
                        }
                    }

                    actionFound = fits;
                    result.matched = fits;
                }
            }
        }

        if (actionFound && actionObjectFound)
        {
            result.response = action.ToUpper() + " " + actionObject.ToUpper();
        }
        else if (actionFound)
        {
            result.response = action.ToUpper() + " WHAT?";
        }
    }
}
