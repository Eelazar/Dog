using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Text.RegularExpressions;

[RequireComponent(typeof(AudioSource), typeof(BootManager))]
public class BootConsole : MonoBehaviour
{
    //Constants
    private const float lineHeight = 20F;

    #region Editor Variables
    [Header("Object References")]
    [SerializeField]
    [Tooltip("The console InputField object")]
    private TMP_InputField console_InputField;
    [SerializeField]
    [Tooltip("The panel containing all the log TextFields")]
    private GameObject consolePanel;
    [SerializeField]
    [Tooltip("The log TextField prefab")]
    private GameObject log_Prefab;

    [Header("Configuration")]
    [SerializeField]
    [Tooltip("The amount of TextFields that will be generated (also max displayed)")]
    private int textFieldAmount;

    [Header("Text Animation")]
    [SerializeField]
    [Tooltip("The duration of the pause between each letter during typewriter animation")]
    private float textSpeed;
    #endregion Editor Varibles

    #region Private Variables
    ////Object References
    //The audio source for the key sounds
    private AudioSource keySource;
    //The dynamic list containing all the text fields that are currently active
    private List<GameObject> log_ActiveFields;
    //All the textfields from bottom to top
    private TMP_Text[] log_TextFields;
    //Manager
    private BootManager manager;
    //Explorer
    private BootExplorer explorer;

    ////Other Variables
    //Holder for the text input
    private string rawInput;
    //Index for the currently selected log entry
    private int currentLogIndex;
    //Index for the currently selected text field position i.e. slot
    private int selectedSlotIndex;
    //Backup of up to x log entries
    private string[] consoleLog = new string[50];
    #endregion Private Variables

    #region Public Variables
    //Boolean to lock console entries during animations
    [HideInInspector]
    public bool loading;
    //The panel containing the console panel and input panel
    [HideInInspector]
    public GameObject window;
    #endregion Public Variables

    void Start()
    {
        //Initialize
        log_ActiveFields = new List<GameObject>();
        log_TextFields = new TMP_Text[textFieldAmount];

        //Get some stuff
        window = consolePanel.transform.parent.gameObject;
        keySource = gameObject.GetComponent<AudioSource>();
        manager = transform.GetComponent<BootManager>();
        explorer = transform.GetComponent<BootExplorer>();

        GenerateTextFieldArray();

        //Turn off all text fields
        foreach (TMP_Text tmp in log_TextFields)
        {
            tmp.gameObject.SetActive(false);
        }        
    }

    void Update()
    {
        //Listen for key presses
        GetInput();
        
        //Dynamically resize the log
        ResizeLog();
    }

    public void Launch()
    {
        console_InputField.ActivateInputField();

    }

    void GetInput()
    {
        //Play a sound if a key is pressed
        if (Input.anyKeyDown)
        {
            keySource.pitch = UnityEngine.Random.Range(0.5F, 3F);
            keySource.Play();
        }

        //Listen for submissions
        if (Input.GetKeyDown(KeyCode.Return) && !loading)
        {
            //Get the input and clear the field
            rawInput = console_InputField.text;
            console_InputField.text = "";

            if (rawInput != "")
            {
                //Send the text to the log
                LogText(rawInput);

                if (rawInput.Contains("start"))
                {
                    //If start command launch start animation
                    StartCoroutine(manager.AnimateStart());
                }
                else if (rawInput.Contains("move up"))
                {
                    string resultString = Regex.Match(rawInput, @"\d+").Value;

                    if(resultString != "")
                    {
                        int amount = int.Parse(resultString);
                        explorer.NavigateUp(amount);
                    }
                    else
                    {
                        explorer.NavigateUp(1);
                    }                    
                }
                else if (rawInput.Contains("open"))
                {
                    //hard coded: must begin with "open "
                    string temp = rawInput.Remove(0, 5);
                    //Capitalize first letter
                    temp = temp.First().ToString().ToUpper() + temp.Substring(1);

                    explorer.NavigateDown(temp);
                }
                else if (rawInput.Contains("refresh"))
                {
                    //If 
                    StartCoroutine(explorer.UpdateData());
                }
                else if (rawInput.Contains("launch explorer"))
                {
                    StartCoroutine(manager.LaunchExplorer());
                }
            }

            //Refocus input field
            console_InputField.ActivateInputField();
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
            if (i == 0)
            {
                //If its a new entry animate it with a tyewriter effect
                StartCoroutine(AnimateText(log_TextFields[i], consoleLog[currentLogIndex - selectedSlotIndex + i]));
            }
            else
            {
                log_TextFields[i].text = consoleLog[currentLogIndex - selectedSlotIndex + i];
            }
        }
    }

    void ResizeLog()
    {
        //Measure how many pixels are unused (e.g. the top part of the window -> decoration)
        float yOffset = consolePanel.GetComponent<RectTransform>().sizeDelta.y;

        //Get the anchors of the log window, i.e. from where to where on the screen it spans
        Vector2 parentAnchorMin = new Vector2(window.GetComponent<RectTransform>().anchorMin.x, window.GetComponent<RectTransform>().anchorMin.y);
        Vector2 parentAnchorMax = new Vector2(window.GetComponent<RectTransform>().anchorMax.x, window.GetComponent<RectTransform>().anchorMax.y);

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

        Debug.Log("Pixel Height: " + canvasHeight + ", Log Window Size: " + logSize + ", Offset: " + yOffset + ", Slots: " + slotAmount + ", Active Slots: " + activeAmount);

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

    IEnumerator AnimateText(TMP_Text ui, string s)
    {
        ui.text = "";

        //Add a new character every X seconds for a typewriter effect
        foreach (char c in s)
        {
            ui.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        yield return null;
    }
    
    void GenerateTextFieldArray()
    {
        for (int i = 0; i < textFieldAmount; i++)
        {
            GameObject go = Instantiate<GameObject>(log_Prefab);
            go.transform.SetParent(consolePanel.transform, false);
            go.name = "Log TextField " + (textFieldAmount - i);
            log_TextFields[textFieldAmount - (i + 1)] = go.GetComponent<TMP_Text>();
        }
    }
}
