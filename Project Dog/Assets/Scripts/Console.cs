using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Console : MonoBehaviour
{
    public TMP_InputField console_InputField;
    public TMP_Text[] log_TextFields;

    public string focus;

    private string rawInput;

    void Start()
    {        
        focus = "console";
        console_InputField.ActivateInputField();
    }

    void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        if(focus == "console" && Input.GetKeyUp(KeyCode.Return))
        {
            rawInput = console_InputField.text;
            console_InputField.text = "";

            if(rawInput != "")
            {
                LogText(rawInput + "\n");
            }

            console_InputField.ActivateInputField();
        }
    }

    void LogText(string s)
    {
        string temp1 = s;
        string temp2;

        for (int i = 0; i < log_TextFields.Length; i++)
        {
            temp2 = log_TextFields[i].text;
            log_TextFields[i].text = temp1;
            temp1 = temp2;
        }
    }
}
