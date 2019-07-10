using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

[RequireComponent(typeof(AudioSource), typeof(XMLLoader))]
public class OldConsole : MonoBehaviour
{
    private const float lineHeight = 20F;

    #region Editor Variables
    [Header("Object References")]
    [SerializeField]
    [Tooltip("The console InputField object")]
    private TMP_InputField console_InputField;
    [SerializeField]
    [Tooltip("The Panel containing the log objects")]
    private GameObject log_Panel;

    [Header("Text Animation")]
    [SerializeField]
    [Tooltip("The duration of the pause between each letter during typewriter animation")]
    private float textSpeed;
    #endregion Editor Variables

    #region Private Variables
    ////Object References
    private AudioSource keySource;
    private XMLLoader xml;
    //The dynamic list containing all the text fields that are currently active
    private List<GameObject> log_ActiveFields;
    //The panel containing the log TextFields and the input field
    private GameObject console_Panel;
    //All the textfields from bottom to top
    private TMP_Text[] log_TextFields;

    ////Other Variables
    //Index for the currently selected log entry
    private int currentLogIndex;
    //Index for the currently selected text field position i.e. slot
    private int selectedSlotIndex;
    //The color of the text in the input field while scrolling through the log
    public Color inputFadeColor;
    //Holder for the text input
    private string rawInput;
    //Backup of up to x log entries
    private string[] consoleLog = new string[200];

    //"console" "documentation"
    private string focus;
    //"input, log" "main"
    private string subFocus;
    #endregion Private Variables

    void Start()
    {
        //Initialize
        keySource = gameObject.GetComponent<AudioSource>();
        xml = gameObject.GetComponent<XMLLoader>();

        console_Panel = log_Panel.transform.parent.gameObject;
        log_ActiveFields = new List<GameObject>();
        log_TextFields = new TMP_Text[log_Panel.transform.childCount];

        FillTextFieldArray();

        //Turn off all text fields
        foreach (TMP_Text tmp in log_TextFields)
        {
            tmp.gameObject.SetActive(false);
        }

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

        ResizeLog();
    }

    void GetInput()
    {
        if (focus == "console" && subFocus == "input")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                rawInput = console_InputField.text;
                console_InputField.text = "";

                if (rawInput != "")
                {
                    LogText(rawInput);

                    CommandFeedback commandFeedback = CommandManager.ExecuteCommand(rawInput.Split(' '));

                    //if (!commandFeedback.valid)
                    //{
                    //    LogText(commandFeedback.feedback);
                    //}

                    ////TEST CASES:
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

                    //if (rawInput.Contains("fullscreen"))
                    //{
                    //    StartCoroutine(gameObject.GetComponent<SizeManager>().EnterFullscreenCam());
                    //}

                    //if (rawInput.Contains("resize to (0.2, 0.2)"))
                    //{
                    //    StartCoroutine(gameObject.GetComponent<SizeManager>().ResizeWindows(new Vector2(0.2F, 0.2F)));
                    //}
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
                SetFocus("input");

                //Copy the selected text to input
                console_InputField.text = consoleLog[currentLogIndex];
                //Set the caret to the end of the line
                console_InputField.caretPosition = console_InputField.text.Length;
                //Remove the "> " from the selected slot
                log_TextFields[selectedSlotIndex].text = log_TextFields[selectedSlotIndex].text.Remove(0, 2);
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                ScrollText(true);
            }

            if (Input.GetKeyUp(KeyCode.DownArrow) && currentLogIndex == 0)
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

    void ResizeLog()
    {
        //Measure how many pixels are unused (e.g. the top part of the window -> decoration)
        float yOffset = log_Panel.GetComponent<RectTransform>().sizeDelta.y;

        //Get the anchors of the log window, i.e. from where to where on the screen it spans
        Vector2 parentAnchorMin = new Vector2(console_Panel.GetComponent<RectTransform>().anchorMin.x, console_Panel.GetComponent<RectTransform>().anchorMin.y);
        Vector2 parentAnchorMax = new Vector2(console_Panel.GetComponent<RectTransform>().anchorMax.x, console_Panel.GetComponent<RectTransform>().anchorMax.y);

        //From the anchors get the percentage of screen the log window occupies
        Vector2 parentScreenPercent = new Vector2(parentAnchorMax.x - parentAnchorMin.x, parentAnchorMax.y - parentAnchorMin.y);

        //Get the height in pixels of the entire game window
        float canvasHeight = Screen.height;

        //Calculate the actual height of the log by multiplying the screen height by the window percentage, and finally substracting the unused space
        float logSize = (canvasHeight * parentScreenPercent.y) + yOffset;

        //Calculate the highest possible amount of slots that can fit into the log
        int slotAmount = Mathf.FloorToInt(logSize / lineHeight);

        //Check how many slots are currently active
        int activeAmount = log_ActiveFields.Count;

        //Debug.Log("Pixel Height: " + canvasHeight + ", Log Window Size: " + logSize + ", Offset: " + yOffset + ", Slots: " + slotAmount + ", Active Slots: " + activeAmount);

        //If more slots could fit into the log, add them
        if (activeAmount < slotAmount)
        {
            for (int i = activeAmount; i < slotAmount; i++)
            {
                log_TextFields[i].gameObject.SetActive(true);
                log_ActiveFields.Add(log_TextFields[i].gameObject);
            }
        }
        //Otherwise remove the excess slots
        else if (activeAmount > slotAmount)
        {
            for (int i = activeAmount; i > slotAmount; i--)
            {
                log_TextFields[i - 1].gameObject.SetActive(false);
                log_ActiveFields.Remove(log_TextFields[i - 1].gameObject);
            }

            //Cleanup
            for (int i = activeAmount; i < log_TextFields.Length; i++)
            {
                log_TextFields[i].gameObject.SetActive(false);
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
        else if (up)
        {
            if (currentLogIndex >= 4)
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
            if (currentLogIndex > 0)
            {
                //If log is above 0, scroll down
                currentLogIndex--;
            }

            if (selectedSlotIndex > 0)
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
            //Add an indentation to currently selected text
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

    void FillTextFieldArray()
    {
        int count = log_TextFields.Length;

        for (int i = 0; i < count; i++)
        {
            log_TextFields[i] = log_Panel.transform.GetChild((count - 1) - i).GetComponent<TMP_Text>();
        }
    }

    void UpdateLog()
    {
        //Fill the text fields
        for (int i = 0; i < log_TextFields.Length; i++)
        {
            //if (i == 0)
            //{
            //    StartCoroutine(AnimateText(log_TextFields[i], consoleLog[currentLogIndex - selectedSlotIndex + i]));
            //}
            //else
            //{
            log_TextFields[i].text = consoleLog[currentLogIndex - selectedSlotIndex + i];
            //}
        }
    }

    IEnumerator AnimateText(TMP_Text ui, string s)
    {
        ui.text = "";

        foreach (char c in s)
        {
            ui.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        yield return null;
    }
}
