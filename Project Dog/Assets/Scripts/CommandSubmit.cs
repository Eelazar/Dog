using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CommandSubmit : MonoBehaviour
{
    public TMP_InputField inputField;

    public KeywordRecogniton keywordRecognition;

    public void Submit(string text)
    {
        keywordRecognition.RecognizeKeywords(text);

        inputField.text = "";

        inputField.ActivateInputField();
    }
}
