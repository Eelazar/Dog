using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeManager : MonoBehaviour
{

    public GameObject docPanel;
    public GameObject camPanel;
    public GameObject consolePanel;
    public GameObject outputPanel;

    public float animDuration;

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


        StartCoroutine(ResizeWindows(new Vector2(0.1F, 0.2F)));
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

        //Anchor resize Animation:
        while (t < 1)
        {
            t = (Time.time - start) / animDuration;

            Vector2 docNewAnchorMin = new Vector2(minAnchorDoc.x, center.y);
            Vector2 docNewAnchorMax = new Vector2(center.x, maxAnchorDoc.y);
            docRect.anchorMin = Vector2.Lerp(minAnchorDoc, docNewAnchorMin, t);
            docRect.anchorMax = Vector2.Lerp(maxAnchorDoc, docNewAnchorMax, t);

            Vector2 camNewAnchorMin = new Vector2(center.x, center.y);
            Vector2 camNewAnchorMax = new Vector2(maxAnchorCam.x, maxAnchorCam.y);
            camRect.anchorMin = Vector2.Lerp(minAnchorCam, camNewAnchorMin, t);
            camRect.anchorMax = Vector2.Lerp(maxAnchorCam, camNewAnchorMax, t);

            Vector2 consoleNewAnchorMin = new Vector2(center.x, minAnchorConsole.y);
            Vector2 consoleNewAnchorMax = new Vector2(maxAnchorConsole.x, center.y);
            consoleRect.anchorMin = Vector2.Lerp(minAnchorConsole, consoleNewAnchorMin, t);
            consoleRect.anchorMax = Vector2.Lerp(maxAnchorConsole, consoleNewAnchorMax, t);

            Vector2 outputNewAnchorMin = new Vector2(minAnchorOutput.x, minAnchorOutput.y);
            Vector2 outputNewAnchorMax = new Vector2(center.x, center.y);
            outputRect.anchorMin = Vector2.Lerp(minAnchorOutput, outputNewAnchorMin, t);
            outputRect.anchorMax = Vector2.Lerp(maxAnchorOutput, outputNewAnchorMax, t);

            yield return null;
        }

    }
}
