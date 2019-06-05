using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class KeywordRecognitionResult
{
    public bool matched;

    public string response;

}
public class KeywordRecogniton : MonoBehaviour
{

    public void RecognizeKeywords(string rawText)
    {
        string[] words = rawText.Split(' ');

        KeywordRecognitionResult result = null;

        for (int i = 0; i < KeywordDictionary.current.sentences.Length; i++)
        {
            KeywordDictionary.current.sentences[i].IsSentence(words, out result);

            if (result.matched)
            {
                break;
            }
        }

        if (result.matched)
        {
            Debug.Log(result.response);
        }
    }
}
