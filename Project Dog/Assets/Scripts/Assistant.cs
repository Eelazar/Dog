using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Assistant : MonoBehaviour
{

    public GameObject assistant_Object;
    public TMP_Text assistant_Text;
    public AnimationCurve animCurve;

    public float animDuration;

    private RectTransform assistant_Rect;

    private Vector2 hiddenPosMin;
    private Vector2 hiddenPosMax;
    private Vector2 shownPosMin;
    private Vector2 shownPosMax;

    private BootManager manager;

    void Start()
    {
        assistant_Rect = assistant_Object.GetComponent<RectTransform>();
        manager = GetComponent<BootManager>();

        shownPosMin = assistant_Rect.anchorMin;
        shownPosMax = assistant_Rect.anchorMax;
        hiddenPosMin = new Vector2(shownPosMin.x, 0 - shownPosMax.y);
        hiddenPosMax = new Vector2(shownPosMax.x, 0 - shownPosMin.y);

        assistant_Rect.anchorMin = hiddenPosMin;
        assistant_Rect.anchorMax = hiddenPosMax;
        assistant_Text.text = "";

        ////Test
        //string s = "Welcome " + PlayerPrefs.GetString("Username", "UNKNOWN") + ", how are you today?";
        //StartCoroutine(DisplayMessage(s, 1.5F));
        //StartCoroutine(HideMessage(5F));

        //s = "Try typing 'help' in the console to see a list of available commands";
        //StartCoroutine(DisplayMessage(s, 6F));
        //StartCoroutine(HideMessage(10F));
    }

    void Update()
    {
        
    }

    public IEnumerator DisplayMessage(string s, float startDelay)
    {
        yield return new WaitForSeconds(startDelay);

        float t = 0;
        float start = Time.time;

        assistant_Text.text = s;

        manager.gameObject.transform.GetChild(0).GetComponent<AudioSource>().Play();

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
    }
}
