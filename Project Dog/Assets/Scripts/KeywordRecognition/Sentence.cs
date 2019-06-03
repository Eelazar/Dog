using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sentence
{
    public string[] keywordsInOrder;

    public int IsSentence(string[] words, out KeywordRecognitionResult result)
    {
        result = new KeywordRecognitionResult();

        List<string> keywords = new List<string>();

        int start = 0;

        for (int i = 0; i < keywordsInOrder.Length; i++)
        {
            for (int j = start; j < words.Length; j++)
            {
                if (words[j].ToUpper().Equals(keywordsInOrder[i].ToUpper()))
                {
                    start = j;

                    keywords.Add(keywordsInOrder[i]);
                }
            }
        }

        result.keywords = keywords.ToArray();

        return keywords.Count;
    }
}
