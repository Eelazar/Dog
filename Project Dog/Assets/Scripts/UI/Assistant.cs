using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Assistant : MonoBehaviour
{
    public AudioClip newNotification;
    public AudioClip notification;

    public GameObject assistant_Object;
    public TMP_Text assistant_Text;
    public AnimationCurve animCurve;

    public float animDuration;

    private RectTransform assistant_Rect;

    private Vector2 hiddenPosMin;
    private Vector2 hiddenPosMax;
    private Vector2 shownPosMin;
    private Vector2 shownPosMax;

    private UIManager manager;

    private bool inUse;
    private Queue<Message> messageQueue;
    private Message currentMessage;
    [HideInInspector]
    public List<Message> messageLog;

    private AudioSource notificationSource;

    void Start()
    {
        assistant_Rect = assistant_Object.GetComponent<RectTransform>();
        manager = GetComponent<UIManager>();
        messageQueue = new Queue<Message>();
        messageLog = new List<Message>();

        shownPosMin = assistant_Rect.anchorMin;
        shownPosMax = assistant_Rect.anchorMax;
        hiddenPosMin = new Vector2(shownPosMin.x, 0 - shownPosMax.y);
        hiddenPosMax = new Vector2(shownPosMax.x, 0 - shownPosMin.y);

        assistant_Rect.anchorMin = hiddenPosMin;
        assistant_Rect.anchorMax = hiddenPosMax;
        assistant_Text.text = "";

        notificationSource = manager.gameObject.transform.GetChild(0).GetComponent<AudioSource>();
    }

    void Update()
    {
        ProcessQueue();
    }

    public void QueueMessage(Message m)
    {
        if(m.priority == true)
        {
            StopCoroutine("DisplayMessage");
            StopCoroutine("HideMessage");

            messageQueue.Clear();
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
                    StopCoroutine("DisplayMessage");
                    StopCoroutine("HideMessage");

                    inUse = false;
                }
            }
        }
    }

    public void Silence()
    {
        StopCoroutine("DisplayMessage");
        StopCoroutine("HideMessage");

        messageQueue.Clear();
    }

    public IEnumerator DisplayMessage(string s, float startDelay)
    {
        inUse = true;

        yield return new WaitForSeconds(startDelay);

        float t = 0;
        float start = Time.time;

        assistant_Text.text = s;

        notificationSource.Play();

        while (t < 1)
        {
            t = (Time.time - start) / animDuration;

            assistant_Rect.anchorMin = Vector2.LerpUnclamped(hiddenPosMin, shownPosMin, animCurve.Evaluate(t));
            assistant_Rect.anchorMax = Vector2.LerpUnclamped(hiddenPosMax, shownPosMax, animCurve.Evaluate(t));

            yield return null;
        }
    }

    public IEnumerator HideMessage(float startDelay)
    {
        yield return new WaitForSeconds(startDelay);

        float t = 0;
        float start = Time.time;

        while (t < 1)
        {
            t = (Time.time - start) / animDuration;

            assistant_Rect.anchorMin = Vector2.LerpUnclamped(shownPosMin, hiddenPosMin, animCurve.Evaluate(t));
            assistant_Rect.anchorMax = Vector2.LerpUnclamped(shownPosMax, hiddenPosMax, animCurve.Evaluate(t));

            yield return null;
        }

        assistant_Text.text = "";

        inUse = false;
    }
}

public class Message
{
    public string content;
    public float startDelay;
    public float duration;

    public bool newNotification;
    public bool weak;
    public bool priority;

    public Message(string _content, float _startDelay, float _duration, bool _newNotification = false, bool _weak = false, bool _priority = false)
    {
        content = _content;
        startDelay = _startDelay;
        duration = _duration;

        weak = _weak;
        priority = _priority;
        newNotification = _newNotification;
    }
}    
