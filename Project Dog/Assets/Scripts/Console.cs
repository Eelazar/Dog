using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

[RequireComponent(typeof(AudioSource), typeof(XMLLoader))]
public class Console : MonoBehaviour
{
    public TMP_InputField console_InputField;
    public TMP_Text[] log_TextFields;

    private AudioSource keySource;
    private XMLLoader xml;

    //"console" "documentation"
    private string focus;
    //"input, log" "main"
    private string subFocus;

    private int currentLogIndex;
    private int selectedSlotIndex;

    public Color inputFadeColor;

    private string rawInput;

    private string[] consoleLog = new string[50];

    void Start()
    {
        keySource = gameObject.GetComponent<AudioSource>();
        xml = gameObject.GetComponent<XMLLoader>();

        SetFocus("input");
    }

    void Update()
    {
        GetInput();

        if (Input.anyKeyDown)
        {
            keySource.pitch = Random.Range(0.5F, 3F);
            keySource.Play();
        }
    }

    void GetInput()
    {
        if(focus == "console" && subFocus == "input")
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                rawInput = console_InputField.text;
                console_InputField.text = "";

                if (rawInput != "")
                {
                    LogText(rawInput);

                    //TEST CASES:
                    if (rawInput.Contains("open"))
                    {
                        //hard coded: must begin with "open "
                        string temp = rawInput.Remove(0, 5);
                        //Capitalize first letter
                        temp = temp.First().ToString().ToUpper() + temp.Substring(1);

                        Debug.Log(temp);

                        xml.MoveDown(temp);
                    }
                    if (rawInput.Contains("move up"))
                    {
                        xml.MoveUp();
                    }
                }

                console_InputField.ActivateInputField();
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                SetFocus("log");

                ScrollText(true, true);
            }
        }
        else if (focus == "console" && subFocus == "log")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                console_InputField.text = consoleLog[currentLogIndex];
                SetFocus("input");
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                ScrollText(true);
            }

            if(Input.GetKeyUp(KeyCode.DownArrow) && currentLogIndex == 0)
            {
                ScrollText(false, true);
                SetFocus("input");
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                ScrollText(false);
            }

        }
    }


    void SetFocus(string f)
    {
        if (f == "input")
        {
            focus = "console";
            subFocus = "input";

            console_InputField.ActivateInputField();
        }
        else if (f == "log")
        {
            focus = "console";
            subFocus = "log";

            console_InputField.DeactivateInputField();
        }
        else if (f == "documentation")
        {
            focus = "documentation";
            subFocus = "main";

            console_InputField.DeactivateInputField();
        }
    }

    void ScrollText(bool up, bool special = false)
    {
        //First time scrolling up, e.g. switching from input to log
        if (special && up)
        {
            currentLogIndex = 0;
            selectedSlotIndex = 0;
        }        
        else if(up)
        {
            if(currentLogIndex >= 4)
            {
                //If selection is at the top of window, only scroll
                currentLogIndex++;
            }
            else
            {
                //If selection is not at the top of window, also move selection
                currentLogIndex++;
                selectedSlotIndex++;
            }
        }
        else if (!up)
        {
            if(currentLogIndex > 0)
            {
                //If log is above 0, scroll down
                currentLogIndex--;
            }

            if(selectedSlotIndex > 0)
            {
                //if selection is not at the bottom, also move selection
                selectedSlotIndex--;
            }
        }

        UpdateLog();

        //If we're switching from log to input, reset the input
        if (special && !up)
        {
            console_InputField.text = "";
        }
        else 
        {
            //Add an indentation to currntly selected text
            log_TextFields[selectedSlotIndex].text = "> " + log_TextFields[selectedSlotIndex].text;
            //Set Input Text to selected text, also lower alpha
            console_InputField.text = "<color=#" + ColorUtility.ToHtmlStringRGBA(inputFadeColor) + ">" + consoleLog[currentLogIndex] + "</color>";
        }

    }

    void LogText(string s)
    {
        //temp1 gets the new text
        string temp1 = s;
        string temp2;

        for (int i = 0; i < consoleLog.Length; i++)
        {
            //Temp2 gets the old text
            temp2 = consoleLog[i];
            //Old text is replaced by the new text
            consoleLog[i] = temp1;
            //Old text is replaced by new text
            temp1 = temp2;
        }

        UpdateLog();
    }

    void UpdateLog()
    {
        //Fill the text fields
        for (int i = 0; i < log_TextFields.Length; i++)
        {
            log_TextFields[i].text = consoleLog[currentLogIndex - selectedSlotIndex + i];
        }
    }
}
