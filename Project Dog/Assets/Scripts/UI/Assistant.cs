using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEditor;

public class Assistant : MonoBehaviour
{
    //Constants
    private const float lineHeight = 40F;

    #region Editor Variables
    public AudioClip newNotification;
    public AudioClip notification;

    public float letterPause;
    public float shortPause;
    public float normalPause;
    public float longPause;


    public GameObject assistant_Window;
    public TMP_Text assistant_Text_Red;
    public TMP_Text assistant_Text_Blue;
    public TMP_Text assistant_Text_White;
    public AnimationCurve animCurve;

    #region Docked Mode
    public bool docked;

    public GameObject assistant_Icon;
    public GameObject log_Panel;
    public GameObject logEntry_Prefab;

    public int logEntryAmount;

    public float textAnimPause;
    #endregion Docked Mode

    public float animDuration;
    #endregion Editor Variables

    #region Private Variables
    private RectTransform assistant_Rect;
    private RectTransform assistantIcon_Rect;

    private Vector2 hiddenPosMin;
    private Vector2 hiddenPosMax;
    private Vector2 shownPosMin;
    private Vector2 shownPosMax;

    private UIManager manager;

    private bool inUse;
    private bool writing;

    private Queue<Message> messageQueue;
    private Message currentMessage;
    public List<Message> messageLog;

    private AudioSource notificationSource;

    private char[] shortCharList = { '-', '"', '(', ')', '\'', };
    private char[] normalCharList = { ',', ':', ';' };
    private char[] longCharList = { '.', '!', '?' };

    //The dynamic list containing all the text fields that are currently active
    private List<GameObject> log_ActiveFields;
    //All the textfields from bottom to top
    private TMP_Text[] log_TextFields;
    #endregion PrivateVariables

    void Awake()
    {
        assistant_Rect = assistant_Window.GetComponent<RectTransform>();
        //manager = transform.GetComponent<UIManager>();
        messageQueue = new Queue<Message>();
        messageLog = new List<Message>();      

        if (docked)
        {
            assistantIcon_Rect = assistant_Icon.GetComponent<RectTransform>();
            log_ActiveFields = new List<GameObject>();
            log_TextFields = new TMP_Text[logEntryAmount];

            shownPosMin = assistantIcon_Rect.anchorMin;
            shownPosMax = assistantIcon_Rect.anchorMax;
            hiddenPosMin = new Vector2(0 - shownPosMax.x, shownPosMin.y);
            hiddenPosMax = new Vector2(0 - shownPosMin.x, shownPosMax.y);

            assistantIcon_Rect.anchorMin = hiddenPosMin;
            assistantIcon_Rect.anchorMax = hiddenPosMax;
        }
        else
        {
            shownPosMin = assistant_Rect.anchorMin;
            shownPosMax = assistant_Rect.anchorMax;
            hiddenPosMin = new Vector2(shownPosMin.x, 0 - shownPosMax.y);
            hiddenPosMax = new Vector2(shownPosMax.x, 0 - shownPosMin.y);

            assistant_Rect.anchorMin = hiddenPosMin;
            assistant_Rect.anchorMax = hiddenPosMax;
        }

        assistant_Text_Red.text = "";
        assistant_Text_Blue.text = "";
        assistant_Text_White.text = "";

        notificationSource = transform.GetChild(0).GetComponent<AudioSource>();

        if (docked)
        {
            GenerateTextFieldArray();

            //Turn off all text fields
            foreach (TMP_Text tmp in log_TextFields)
            {
                tmp.gameObject.SetActive(false);
            }
        }        
    }

    void Update()
    {
        ProcessQueue();

        if (docked)
        {
            ResizeLog();
        }
    }

    public void QueueMessage(Message m)
    {
        if(m.priority == true)
        {
            Silence();
        }

        messageQueue.Enqueue(m);
    }

    public void ProcessQueue()
    {
        if(messageQueue.Count > 0)
        {
            if (!inUse)
            {
                Message m = messageQueue.Dequeue();
                currentMessage = m;
                messageLog.Add(m);

                //Play appropriate notification sound
                if (m.newNotification == true)
                {
                    notificationSource.clip = newNotification;
                }
                else
                {
                    notificationSource.clip = notification;
                }

                //Play appropriate notification
                if (docked)
                {
                    StartCoroutine(DisplayDockedMessage(m.content, m.startDelay, m.endDelay, m.stay));
                }
                else if (!docked)
                {
                    StartCoroutine(DisplayMessage(m.content, m.startDelay, m.endDelay, m.stay));
                }                
            }
            else
            {
                if(currentMessage.stay == true && writing == false)
                {
                    if (docked)
                    {
                        StartCoroutine(HideDockedMessage());
                        writing = true;
                    }                        
                    else
                    {
                        StartCoroutine(HideMessage());
                        writing = true;
                    }
                }
            }
        }
    }

    public void Silence()
    {
        StopCoroutine("DisplayMessage");
        StopCoroutine("HideMessage");
        StopCoroutine("DisplayDockedMessage");
        StopCoroutine("HideDockedMessage");

        messageQueue.Clear();
    }

    void UpdateLog()
    {
        //Fill the text fields
        for (int i = 0; i < log_TextFields.Length; i++)
        {
            int currentLogIndex = messageLog.Count - (i + 1);

            if (i == 0)
            {
                //StartCoroutine(AnimateText(log_TextFields[i], messageLog[currentLogIndex].content));
                log_TextFields[i].text = messageLog[currentLogIndex].content;
            }
            else if (i < messageLog.Count)
            {
                log_TextFields[i].text = messageLog[currentLogIndex].content;
            }
        }
    }

    #region Animations
    public IEnumerator DisplayMessage(string s, float startDelay, float endDelay, bool stay)
    {
        assistant_Text_Red.text = s;
        assistant_Text_Red.maxVisibleCharacters = 0;
        assistant_Text_Blue.text = s;
        assistant_Text_Blue.maxVisibleCharacters = 0;
        assistant_Text_White.text = s;
        assistant_Text_White.maxVisibleCharacters = 0;

        inUse = true;
        writing = true;

        yield return new WaitForSeconds(startDelay);

        float t = 0;
        float start = Time.time;

        notificationSource.Play();

        while (t < 1)
        {
            t = (Time.time - start) / animDuration;

            assistant_Rect.anchorMin = Vector2.LerpUnclamped(hiddenPosMin, shownPosMin, animCurve.Evaluate(t));
            assistant_Rect.anchorMax = Vector2.LerpUnclamped(hiddenPosMax, shownPosMax, animCurve.Evaluate(t));

            yield return null;
        }

        var letterWait = new WaitForSeconds(letterPause);
        var shortWait = new WaitForSeconds(shortPause);
        var normalWait = new WaitForSeconds(normalPause);
        var longWait = new WaitForSeconds(longPause);

        int i = 0;
        char[] charArray = s.ToCharArray();
        while (i < charArray.Length)
        {
            assistant_Text_Red.maxVisibleCharacters = i + 1;
            assistant_Text_Blue.maxVisibleCharacters = i + 1;
            assistant_Text_White.maxVisibleCharacters = i + 1;

            if (char.IsLetterOrDigit(charArray[i]))
            {
                yield return letterWait;
            }
            else if (Array.Exists(shortCharList, element => element == charArray[i]))
            {
                yield return shortWait;
            }
            else if (Array.Exists(normalCharList, element => element == charArray[i]))
            {
                yield return normalWait;
            }
            else if (Array.Exists(longCharList, element => element == charArray[i]))
            {
                yield return longWait;
            }
            else
            {
                yield return letterWait;
            }

            i++;
        }

        if (!stay)
        {
            StartCoroutine(HideMessage(endDelay));
        }
        else
        {
            yield return new WaitForSeconds(endDelay);
            writing = false;
        }
    }

    public IEnumerator HideMessage(float startDelay = 0)
    {
        yield return new WaitForSeconds(startDelay);

        float t = 0;
        float start = Time.time;

        while (t < 1)
        {
            t = (Time.time - start) / animDuration;
            float tE = animCurve.Evaluate(t);

            assistant_Rect.anchorMin = Vector2.LerpUnclamped(shownPosMin, hiddenPosMin, tE);
            assistant_Rect.anchorMax = Vector2.LerpUnclamped(shownPosMax, hiddenPosMax, tE);

            yield return null;
        }

        assistant_Text_Red.text = "";
        assistant_Text_Blue.text = "";
        assistant_Text_White.text = "";

        inUse = false;
    }

    public IEnumerator DisplayDockedMessage(string s, float startDelay, float endDelay, bool stay)
    {
        assistant_Text_Red.text = s;
        assistant_Text_Red.maxVisibleCharacters = 0;
        assistant_Text_Blue.text = s;
        assistant_Text_Blue.maxVisibleCharacters = 0;
        assistant_Text_White.text = s;
        assistant_Text_White.maxVisibleCharacters = 0;

        inUse = true;
        writing = true;

        yield return new WaitForSeconds(startDelay);

        notificationSource.Play();

        float t = 0;
        float start = Time.time;

        while (t < 1)
        {
            t = (Time.time - start) / animDuration;
            float tE = animCurve.Evaluate(t);

            assistantIcon_Rect.anchorMin = Vector2.LerpUnclamped(hiddenPosMin, shownPosMin, tE);
            assistantIcon_Rect.anchorMax = Vector2.LerpUnclamped(hiddenPosMax, shownPosMax, tE);

            yield return null;
        }

        var letterWait = new WaitForSeconds(letterPause);
        var shortWait = new WaitForSeconds(shortPause);
        var normalWait = new WaitForSeconds(normalPause);
        var longWait = new WaitForSeconds(longPause);

        int i = 0;
        char[] charArray = s.ToCharArray();
        while (i < charArray.Length)
        {
            assistant_Text_Red.maxVisibleCharacters = i + 1;
            assistant_Text_Blue.maxVisibleCharacters = i + 1;
            assistant_Text_White.maxVisibleCharacters = i + 1;

            if (char.IsLetterOrDigit(charArray[i]))
            {
                yield return letterWait;
            }
            else if (Array.Exists(shortCharList, element => element == charArray[i]))
            {
                yield return shortWait;
            }
            else if (Array.Exists(normalCharList, element => element == charArray[i]))
            {
                yield return normalWait;
            }
            else if (Array.Exists(longCharList, element => element == charArray[i]))
            {
                yield return longWait;
            }
            else
            {
                yield return letterWait;
            }

            i++;

        }
        
        if (!stay)
        {
            StartCoroutine(HideDockedMessage(endDelay));
        }
        else
        {
            yield return new WaitForSeconds(endDelay);
            writing = false;
        }
    }

    public IEnumerator HideDockedMessage(float startDelay = 0)
    {
        yield return new WaitForSeconds(startDelay);

        float t = 0;
        float start = Time.time;

        Color32 fullRed = assistant_Text_Red.color;
        fullRed = new Color32(fullRed.r, fullRed.g, fullRed.b, 255);
        Color32 clearRed = new Color32(fullRed.r, fullRed.g, fullRed.b, 0);
        Color32 fullBlue = assistant_Text_Blue.color;
        fullBlue = new Color32(fullBlue.r, fullBlue.g, fullBlue.b, 255);
        Color32 clearBlue = new Color32(fullBlue.r, fullBlue.g, fullBlue.b, 0);
        Color32 fullWhite = assistant_Text_White.color;
        fullWhite = new Color32(fullWhite.r, fullWhite.g, fullWhite.b, 255);
        Color32 clearWhite = new Color32(fullWhite.r, fullWhite.g, fullWhite.b, 0);

        while (t < 1)
        {
            t = (Time.time - start) / animDuration;
            float tE = animCurve.Evaluate(t);

            assistantIcon_Rect.anchorMin = Vector2.LerpUnclamped(shownPosMin, hiddenPosMin, tE);
            assistantIcon_Rect.anchorMax = Vector2.LerpUnclamped(shownPosMax, hiddenPosMax, tE);

            assistant_Text_Red.color = Color32.Lerp(fullRed, clearRed, t);
            assistant_Text_Blue.color = Color32.Lerp(fullBlue, clearBlue, t);
            assistant_Text_White.color = Color32.Lerp(fullWhite, clearWhite, t);

            yield return null;
        }

        assistant_Text_Red.text = "";
        assistant_Text_Blue.text = "";
        assistant_Text_White.text = "";

        assistant_Text_Red.color = fullRed;
        assistant_Text_Blue.color = fullBlue;
        assistant_Text_White.color = fullWhite;


        UpdateLog();

        inUse = false;
    }
    #endregion Animations

    #region Utility
    void GenerateTextFieldArray()
    {
        for (int i = 0; i < logEntryAmount; i++)
        {
            GameObject go = Instantiate<GameObject>(logEntry_Prefab);
            go.transform.SetParent(log_Panel.transform, false);
            go.name = "Log TextField " + (logEntryAmount - i);
            go.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
            log_TextFields[logEntryAmount - (i + 1)] = go.GetComponent<TMP_Text>();
        }
    }

    void ResizeLog()
    {
        //Measure how many pixels are unused (e.g. the top part of the window -> decoration)
        float yOffset = log_Panel.GetComponent<RectTransform>().sizeDelta.y;

        //Get the anchors of the log window, i.e. from where to where on the screen it spans
        Vector2 parentAnchorMin = new Vector2(assistant_Window.GetComponent<RectTransform>().anchorMin.x, assistant_Window.GetComponent<RectTransform>().anchorMin.y);
        Vector2 parentAnchorMax = new Vector2(assistant_Window.GetComponent<RectTransform>().anchorMax.x, assistant_Window.GetComponent<RectTransform>().anchorMax.y);

        //From the anchors get the percentage of screen the log window occupies
        Vector2 parentScreenPercent = new Vector2(parentAnchorMax.x - parentAnchorMin.x, parentAnchorMax.y - parentAnchorMin.y);

        //Get the anchors of the log_Panel, i.e. from where to where on the screen it spans
        Vector2 anchorMin = new Vector2(log_Panel.GetComponent<RectTransform>().anchorMin.x, log_Panel.GetComponent<RectTransform>().anchorMin.y);
        Vector2 anchorMax = new Vector2(log_Panel.GetComponent<RectTransform>().anchorMax.x, log_Panel.GetComponent<RectTransform>().anchorMax.y);

        //From the anchors get the percentage of screen the log_Panel occupies
        Vector2 screenPercent = new Vector2(anchorMax.x - anchorMin.x, anchorMax.y - anchorMin.y);

        //Get the height in pixels of the entire game window
        float canvasHeight = Screen.height;

        //Calculate the actual height of the log by multiplying the screen height by the window percentage, and finally substracting the unused space
        float logSize = ((canvasHeight * parentScreenPercent.y) * screenPercent.y) + yOffset;

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
        var wait = new WaitForSeconds(textAnimPause);

        ui.text = s;
        ui.maxVisibleCharacters = 0;

        yield return new WaitForEndOfFrame();

        int i = 0;
        char[] charArray = s.ToCharArray();
        while (i < charArray.Length)
        {
            ui.maxVisibleCharacters = i + 1;
            i++;
            yield return wait;
        }
    }
    #endregion Utility
}

public class Message
{
    public string content;
    public float startDelay;
    public float endDelay;

    public bool newNotification;
    public bool stay;
    public bool priority;

    public Message(string _content, float _startDelay = 0, float _endDelay = 0, bool _newNotification = false, bool _stay = false, bool _priority = false)
    {
        content = _content;
        startDelay = _startDelay;
        endDelay = _endDelay;

        stay = _stay;
        priority = _priority;
        newNotification = _newNotification;
    }
    
}



