using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using System.Xml;

[RequireComponent(typeof(AudioSource))]
public class Console : MonoBehaviour
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
    [SerializeField]
    [Tooltip("The color of not-selected text when scrolling the log")]
    private Color altTextColor;
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
    //Assistant
    private Assistant assistant;

    ////Other Variables
    //Holder for the text input
    private string rawInput;
    //Index for the currently selected log entry
    private int currentLogIndex;
    //Index for the currently selected text field position i.e. slot
    private int selectedSlotIndex;
    //Backup of up to x log entries
    private string[] consoleLog = new string[200];

    //XML Navigation Variables
    private XPathNavigator nav;
    private XmlDocument xmlDoc;

    private string focus;
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
        assistant = transform.GetComponent<Assistant>();

        GenerateTextFieldArray();

        //Turn off all text fields
        foreach (TMP_Text tmp in log_TextFields)
        {
            tmp.gameObject.SetActive(false);
        }

        // Open the XML.
        xmlDoc = new XmlDocument();
        xmlDoc.Load("Assets\\Scripts\\CommandTree.xml");
        // Create a navigator to query with XPath.
        nav = xmlDoc.CreateNavigator();
        //Initial XPathNavigator to start at the root.
        nav.MoveToRoot();
        nav.MoveToFirstChild();

        Activate();
    }

    void Update()
    {
        //Listen for key presses
        GetInput();

        //Dynamically resize the log
        ResizeLog();
    }

    public void Activate()
    {
        console_InputField.ActivateInputField();

        focus = "console";
    }

    public IEnumerator Deactivate()
    {
        yield return new WaitForEndOfFrame();

        console_InputField.DeactivateInputField();
    }

    void GetInput()
    {
        //Play a sound if a key is pressed
        if (Input.anyKeyDown)
        {
            keySource.pitch = UnityEngine.Random.Range(0.5F, 3F);
            keySource.Play();
        }

        if (focus == "console")
        {
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
                    FindMethod(new XMLObject(rawInput));
                }

                //Refocus input field
                console_InputField.ActivateInputField();
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                StartCoroutine(Deactivate());

                focus = "log";

                ScrollText(true, true);
            }
        }
        else if (focus == "log")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Activate();

                //Copy the selected text to input
                console_InputField.text = consoleLog[currentLogIndex];
                //Set the caret to the end of the line
                console_InputField.caretPosition = console_InputField.text.Length;
                //Remove the "> " from the selected slot
                log_TextFields[selectedSlotIndex].text = log_TextFields[selectedSlotIndex].text.Remove(0, 2);

                currentLogIndex = 0;
                selectedSlotIndex = 0;
                UpdateLog();
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                ScrollText(true);
            }

            if (Input.GetKeyUp(KeyCode.DownArrow) && currentLogIndex == 0)
            {
                ScrollText(false, true);
                Activate();
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                ScrollText(false);
            }

        }
    }

    void FindMethod(XMLObject obj)
    {
        if (obj.progressionIndex == 0)
        {
            nav.MoveToRoot();
            nav.MoveToFirstChild();
        }

        if (nav.HasChildren)
        {
            if (obj.commandWords.Length >= obj.progressionIndex)
            {
                int childCount = nav.SelectChildren(XPathNodeType.All).Count;
                nav.MoveToFirstChild();

                for (int i = 0; i < childCount; i++)
                {
                    if (nav.GetAttribute("name", string.Empty) != "")
                    {
                        string name = nav.GetAttribute("name", string.Empty);

                        if (obj.commandWords[obj.progressionIndex].StartsWith(name))
                        {
                            obj.objectName = obj.commandWords[obj.progressionIndex];

                            obj.xmlPath.Add(nav.Name);
                            obj.progressionIndex++;
                            FindMethod(obj);
                            return;
                        }
                    }
                    else if (nav.GetAttribute("synonyms", string.Empty) != "")
                    {

                        string[] synonyms = nav.GetAttribute("synonyms", string.Empty).Split(' ');

                        foreach (string synonym in synonyms)
                        {
                            if (synonym == obj.commandWords[obj.progressionIndex])
                            {
                                obj.xmlPath.Add(nav.Name);
                                obj.progressionIndex++;
                                FindMethod(obj);
                                return;
                            }
                        }
                    }
                    else if (nav.Name[0] == '_')
                    {
                        obj.xmlPath.Add(nav.Name);
                        obj.parameters.Add(obj.commandWords[obj.progressionIndex]);
                        obj.progressionIndex++;
                        FindMethod(obj);
                        return;
                    }
                    else if (nav.Name == "method")
                    {
                        obj.xmlPath.Add(nav.Name);
                        obj.progressionIndex++;
                        obj.methodName = nav.Value;
                        ExecuteMethod(obj);
                        return;
                    }

                    nav.MoveToNext();
                }
            }
            else
            {
                /////////////DEFAULT
            }
        }
        else
        {
            Debug.Log("XML has no child nodes at level: " + nav.Name);
        }
    }

    void ExecuteMethod(XMLObject finalObj)
    {
        BaseObject baseObject;

        ObjectManager.GetObject(finalObj.objectName, out baseObject);

        if (baseObject != null)
            baseObject.SendMessage(finalObj.methodName, new CommandContext() { parameters = finalObj.parameters.ToArray() });

        //Debug.Log(finalObj.methodName + " / " + string.Join(", ", finalObj.parameters.ToArray()));
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
            if (selectedSlotIndex >= log_ActiveFields.Count - 1)
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
            currentLogIndex = 0;
            selectedSlotIndex = 0;
        }
        else
        {
            //Add an indentation to currently selected text
            log_TextFields[selectedSlotIndex].text = "> " + log_TextFields[selectedSlotIndex].text;
            //Set Input Text to selected text, also lower alpha
            console_InputField.text = "<color=#" + ColorUtility.ToHtmlStringRGBA(altTextColor) + ">" + consoleLog[currentLogIndex] + "</color>";
        }

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
        yOffset -= 20F;

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
            go.transform.SetParent(consolePanel.transform.GetChild(0).transform, false);
            go.name = "Log TextField " + (textFieldAmount - i);
            go.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
            log_TextFields[textFieldAmount - (i + 1)] = go.GetComponent<TMP_Text>();
        }
    }
}

public class XMLQuery
{
    public string[] commandWords;
    public List<string> xmlPath;
    public List<string> parameters;
    public int progressionIndex;

    public string methodName;

    public XMLQuery(string text)
    {
        commandWords = text.Split(' ');

        xmlPath = new List<string>();
        parameters = new List<string>();
    }
}
