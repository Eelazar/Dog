using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SizeManager : MonoBehaviour
{

    public GameObject docPanel;
    public GameObject camPanel;
    public GameObject consolePanel;
    public GameObject outputPanel;

    public float animDuration;
    public float fullscreenConsoleAlpha;

    private RectTransform docRect;
    private RectTransform camRect;
    private RectTransform consoleRect;
    private RectTransform outputRect;

    void Start()
    {
        docRect = docPanel.GetComponent<RectTransform>();
        camRect = camPanel.GetComponent<RectTransform>();
        consoleRect = consolePanel.GetComponent<RectTransform>();
        outputRect = outputPanel.GetComponent<RectTransform>();


        //StartCoroutine(EnterFullscreenCam());
        //StartCoroutine(ResizeWindows(new Vector2(0.1F, 0.2F)));
    }

    void Update()
    {
        
    }

    public IEnumerator ResizeWindows(Vector2 center)
    {
        //Initialize Lerp
        float t = 0;
        float start = Time.time;

        //Get the anchors of the panels
        Vector2 minAnchorDoc = docRect.anchorMin;
        Vector2 maxAnchorDoc = docRect.anchorMax;
        Vector2 minAnchorCam = camRect.anchorMin;
        Vector2 maxAnchorCam = camRect.anchorMax;
        Vector2 minAnchorConsole = consoleRect.anchorMin;
        Vector2 maxAnchorConsole = consoleRect.anchorMax;
        Vector2 minAnchorOutput = outputRect.anchorMin;
        Vector2 maxAnchorOutput = outputRect.anchorMax;

        Color consoleColor = consolePanel.GetComponent<Image>().color;
        float a = consoleColor.a;

        //Anchor resize Animation:
        while (t < 1)
        {
            t = (Time.time - start) / animDuration;

            Vector2 docNewAnchorMin = new Vector2(0F, center.y);
            Vector2 docNewAnchorMax = new Vector2(center.x, 1F);
            docRect.anchorMin = Vector2.Lerp(minAnchorDoc, docNewAnchorMin, t);
            docRect.anchorMax = Vector2.Lerp(maxAnchorDoc, docNewAnchorMax, t);

            Vector2 camNewAnchorMin = new Vector2(center.x, center.y);
            Vector2 camNewAnchorMax = new Vector2(1F, 1F);
            camRect.anchorMin = Vector2.Lerp(minAnchorCam, camNewAnchorMin, t);
            camRect.anchorMax = Vector2.Lerp(maxAnchorCam, camNewAnchorMax, t);

            Vector2 outputNewAnchorMin = new Vector2(0F, 0F);
            Vector2 outputNewAnchorMax = new Vector2(center.x, center.y);
            outputRect.anchorMin = Vector2.Lerp(minAnchorOutput, outputNewAnchorMin, t);
            outputRect.anchorMax = Vector2.Lerp(maxAnchorOutput, outputNewAnchorMax, t);

            Vector2 consoleNewAnchorMin = new Vector2(center.x, 0F);
            Vector2 consoleNewAnchorMax = new Vector2(1F, center.y);
            consoleRect.anchorMin = Vector2.Lerp(minAnchorConsole, consoleNewAnchorMin, t);
            consoleRect.anchorMax = Vector2.Lerp(maxAnchorConsole, consoleNewAnchorMax, t);

            consoleColor.a = Mathf.Lerp(a, 1, t);
            consolePanel.GetComponent<Image>().color = consoleColor;

            yield return null;
        }

    }

    public IEnumerator EnterFullscreenCam()
    {
        //Initialize Lerp
        float t = 0;
        float start = Time.time;

        //Get the anchors of the panels
        Vector2 minAnchorDoc = docRect.anchorMin;
        Vector2 maxAnchorDoc = docRect.anchorMax;
        Vector2 minAnchorCam = camRect.anchorMin;
        Vector2 maxAnchorCam = camRect.anchorMax;
        Vector2 minAnchorConsole = consoleRect.anchorMin;
        Vector2 maxAnchorConsole = consoleRect.anchorMax;
        Vector2 minAnchorOutput = outputRect.anchorMin;
        Vector2 maxAnchorOutput = outputRect.anchorMax;

        Color consoleColor = consolePanel.GetComponent<Image>().color;

        //Anchor resize Animation:
        while (t < 1)
        {
            t = (Time.time - start) / animDuration;

            Vector2 docNewAnchorMin = new Vector2(0F, 1F);
            Vector2 docNewAnchorMax = new Vector2(0F, 1F);
            docRect.anchorMin = Vector2.Lerp(minAnchorDoc, docNewAnchorMin, t);
            docRect.anchorMax = Vector2.Lerp(maxAnchorDoc, docNewAnchorMax, t);

            Vector2 camNewAnchorMin = new Vector2(0F, 0F);
            Vector2 camNewAnchorMax = new Vector2(1F, 1F);
            camRect.anchorMin = Vector2.Lerp(minAnchorCam, camNewAnchorMin, t);
            camRect.anchorMax = Vector2.Lerp(maxAnchorCam, camNewAnchorMax, t);
            
            Vector2 outputNewAnchorMin = new Vector2(0F, 0F);
            Vector2 outputNewAnchorMax = new Vector2(0F, 0F);
            outputRect.anchorMin = Vector2.Lerp(minAnchorOutput, outputNewAnchorMin, t);
            outputRect.anchorMax = Vector2.Lerp(maxAnchorOutput, outputNewAnchorMax, t);

            Vector2 consoleNewAnchorMin = new Vector2(0.1F, 0.05F);
            Vector2 consoleNewAnchorMax = new Vector2(0.9F, 0.3F);
            consoleRect.anchorMin = Vector2.Lerp(minAnchorConsole, consoleNewAnchorMin, t);
            consoleRect.anchorMax = Vector2.Lerp(maxAnchorConsole, consoleNewAnchorMax, t);

            consoleColor.a = Mathf.Lerp(1, fullscreenConsoleAlpha, t);
            consolePanel.GetComponent<Image>().color = consoleColor;

            yield return null;
        }

    }
}
