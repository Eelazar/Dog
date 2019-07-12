using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AssistantDocked : MonoBehaviour
{
    //Constants
    private const float lineHeight = 40F;

    public AudioClip newNotification;
    public AudioClip notification;

    public float letterPause;
    public float shortPause;
    public float normalPause;
    public float longPause;

    public float textAnimPause;

    public GameObject assistant_Window;
    public TMP_Text assistant_Text;
    public GameObject assistant_Icon;
    public GameObject log_Panel;
    public GameObject logEntry_Prefab;
    public AnimationCurve animCurve;

    public int logEntryAmount;

    public float animDuration;

    private RectTransform assistantIcon_Rect;

    private Vector2 hiddenPosMin;
    private Vector2 hiddenPosMax;
    private Vector2 shownPosMin;
    private Vector2 shownPosMax;

    private BootManager manager;

    private bool inUse;
    private Queue<Message> messageQueue;
    private Message currentMessage;
    public List<Message> messageLog;

    private AudioSource notificationSource;

    private char[] shortCharList = { '-' , '"' , '(' , ')' , '\'' , ' ' };
    private char[] normalCharList = { ',' , ':' , ';' };
    private char[] longCharList = { '.' , '!' , '?' };

    //The dynamic list containing all the text fields that are currently active
    private List<GameObject> log_ActiveFields;
    //All the textfields from bottom to top
    private TMP_Text[] log_TextFields;

    void Start()
    {
        assistantIcon_Rect = assistant_Icon.GetComponent<RectTransform>();
        manager = GetComponent<BootManager>();
        messageQueue = new Queue<Message>();
        messageLog = new List<Message>();
        log_ActiveFields = new List<GameObject>();
        log_TextFields = new TMP_Text[logEntryAmount];

        shownPosMin = assistantIcon_Rect.anchorMin;
        shownPosMax = assistantIcon_Rect.anchorMax;
        hiddenPosMin = new Vector2(0 - shownPosMax.x, shownPosMin.y);
        hiddenPosMax = new Vector2(0 - shownPosMin.x, shownPosMax.y);

        assistantIcon_Rect.anchorMin = hiddenPosMin;
        assistantIcon_Rect.anchorMax = hiddenPosMax;
        assistant_Text.text = "";

        notificationSource = transform.GetChild(0).GetComponent<AudioSource>();

        GenerateTextFieldArray();

        //Turn off all text fields
        foreach (TMP_Text tmp in log_TextFields)
        {
            tmp.gameObject.SetActive(false);
        }

        Message test = new Message("Hi there! I'm Neptune, still your assistant, only difference is that I'm in here now...", 2F, 10F, true);
        QueueMessage(test);
        Message test2 = new Message("As you can see I have started to log my messages so you can review anything you missed.", 1F, 10F);
        QueueMessage(test2);
    }

    void Update()
    {
        ProcessQueue();

        ResizeLog();
    }

    public void QueueMessage(Message m)
    {
        if(m.priority == true)
        {
            ClearQueue();
        }

        messageQueue.Enqueue(m);
    }

    private void ClearQueue()
    {
        StopCoroutine("DisplayMessage");
        StopCoroutine("HideMessage");

        messageQueue.Clear();

        assistantIcon_Rect.anchorMin = hiddenPosMin;
        assistantIcon_Rect.anchorMax = hiddenPosMax;

        assistant_Text.text = "";

        inUse = false;
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

                if (m.newNotification == true)
                {
                    notificationSource.clip = newNotification;
                }
                else
                {
                    notificationSource.clip = notification;
                }
                StartCoroutine(DisplayMessage(m.content, m.startDelay));
                StartCoroutine(HideMessage(m.startDelay + m.duration));
            }
            else
            {
                if(currentMessage.weak == true)
                {
                    ClearQueue();
                }
            }
        }
    }

    public IEnumerator DisplayMessage(string s, float startDelay)
    {
        inUse = true;

        yield return new WaitForSeconds(startDelay);        

        notificationSource.Play();

        float t = 0;
        float start = Time.time;

        while (t < 1)
        {
            t = (Time.time - start) / animDuration;

            assistantIcon_Rect.anchorMin = Vector2.LerpUnclamped(hiddenPosMin, shownPosMin, animCurve.Evaluate(t));
            assistantIcon_Rect.anchorMax = Vector2.LerpUnclamped(hiddenPosMax, shownPosMax, animCurve.Evaluate(t));

            yield return null;
        }

        int i = 0;
        char[] charArray = s.ToCharArray();
        while(i < charArray.Length)
        {    
            assistant_Text.text += charArray[i];

            if (char.IsLetterOrDigit(charArray[i]))
            {
                yield return new WaitForSeconds(letterPause);
            }
            else if (Array.Exists(shortCharList, element => element == charArray[i]))
            {
                yield return new WaitForSeconds(shortPause);
            }
            else if (Array.Exists(normalCharList, element => element == charArray[i]))
            {
                yield return new WaitForSeconds(normalPause);
            }
            else if (Array.Exists(longCharList, element => element == charArray[i]))
            {
                yield return new WaitForSeconds(longPause);
            }
            else
            {
                Debug.Log("Char: " + charArray[i] + " could not be matched to a list");
            }

            i++;

        }
    }

    public IEnumerator HideMessage(float startDelay)
    {
        yield return new WaitForSeconds(startDelay);

        float t = 0;
        float start = Time.time;

        Color32 full = assistant_Text.color;
        full = new Color32(full.r, full.g, full.b, 255);
        Color32 clear = new Color32(full.r, full.g, full.b, 0);

        while (t < 1)
        {
            t = (Time.time - start) / animDuration;

            assistantIcon_Rect.anchorMin = Vector2.LerpUnclamped(shownPosMin, hiddenPosMin, animCurve.Evaluate(t));
            assistantIcon_Rect.anchorMax = Vector2.LerpUnclamped(shownPosMax, hiddenPosMax, animCurve.Evaluate(t));

            assistant_Text.color = Color32.Lerp(full, clear, t);

            yield return null;
        }

        assistant_Text.text = "";

        assistant_Text.color = full;


        UpdateLog();

        inUse = false;
    }

    void UpdateLog()
    {
        //Fill the text fields
        for (int i = 0; i < log_TextFields.Length; i++)
        {
            int currentLogIndex = messageLog.Count - (i + 1);

            if(i == 0)
            {
                StartCoroutine(AnimateText(log_TextFields[i], messageLog[currentLogIndex].content));
            }            
            else if (i < messageLog.Count)
            {
                log_TextFields[i].text = messageLog[currentLogIndex].content;
            }
        }
    }

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
            yield return new WaitForSeconds(textAnimPause);
        }

        yield return null;
    }
}



public class MessageDocked
{
    public string content;
    public float startDelay;
    public float duration;

    public bool newNotification;
    public bool priority;

    public MessageDocked(string _content, float _startDelay, float _duration, bool _newNotification = false, bool _priority = false)
    {
        content = _content;
        startDelay = _startDelay;
        duration = _duration; 
        priority = _priority;
        newNotification = _newNotification;
    }
}    
