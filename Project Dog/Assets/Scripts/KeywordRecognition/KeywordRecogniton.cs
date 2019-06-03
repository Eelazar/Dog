using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class KeywordRecognitionResult
{

    public string[] keywords;

}
public class KeywordRecogniton : MonoBehaviour
{

    public void RecognizeKeywords(string rawText)
    {
        string[] words = rawText.Split(' ');

        int bestMatchCount = 0;

        KeywordRecognitionResult bestMatch = null;

        for (int i = 0; i < KeywordDictionary.current.sentences.Length; i++)
        {
            KeywordRecognitionResult result;

            int currentMatch = KeywordDictionary.current.sentences[i].IsSentence(words, out result);

            if (currentMatch > bestMatchCount)
            {
                bestMatchCount = currentMatch;

                bestMatch = result;
            }
        }

        if (bestMatch != null)
        {
            StringBuilder str = new StringBuilder();

            for (int i = 0; i < bestMatch.keywords.Length; i++)
            {
                if (i == bestMatch.keywords.Length - 1)
                {
                    str.Append(bestMatch.keywords[i]);
                }
                else
                {
                    str.Append(bestMatch.keywords[i] + ", ");
                }

            }

            Debug.Log(str.ToString());
        }
    }
}
