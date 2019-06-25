using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Output : MonoBehaviour
{
    private const float lineHeight = 20F;

    #region Editor Variables
    [Header("Object References")]
    [SerializeField]
    [Tooltip("The panel containing all the log TextFields")]
    private GameObject entryHolder;   


    #endregion Editor Variables


    //The panel containing the console panel and input panel
    private GameObject outputPanel;
    //All the textfields from bottom to top
    private TMP_Text[] textFields;
    //The dynamic list containing all the text fields that are currently active
    private List<GameObject> activeFields;

    void Start()
    {
        outputPanel = entryHolder.transform.parent.gameObject;

        activeFields = new List<GameObject>();
        textFields = new TMP_Text[entryHolder.transform.childCount];

        FillTextFieldArray();

        //Turn off all text fields
        foreach (TMP_Text tmp in textFields)
        {
            tmp.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        ResizeLog();
    }

    void ResizeLog()
    {
        //Measure how many pixels are unused (e.g. the top part of the window -> decoration)
        float yOffset = entryHolder.GetComponent<RectTransform>().sizeDelta.y;

        //Get the anchors of the log window, i.e. from where to where on the screen it spans
        Vector2 parentAnchorMin = new Vector2(outputPanel.GetComponent<RectTransform>().anchorMin.x, outputPanel.GetComponent<RectTransform>().anchorMin.y);
        Vector2 parentAnchorMax = new Vector2(outputPanel.GetComponent<RectTransform>().anchorMax.x, outputPanel.GetComponent<RectTransform>().anchorMax.y);

        //From the anchors get the percentage of screen the log window occupies
        Vector2 parentScreenPercent = new Vector2(parentAnchorMax.x - parentAnchorMin.x, parentAnchorMax.y - parentAnchorMin.y);

        //Get the height in pixels of the entire game window
        float canvasHeight = Screen.height;

        //Calculate the actual height of the log by multiplying the screen height by the window percentage, and finally substracting the unused space
        float logSize = (canvasHeight * parentScreenPercent.y) + yOffset;

        //Calculate the highest possible amount of slots that can fit into the log
        int slotAmount = Mathf.FloorToInt(logSize / lineHeight);

        //Check how many slots are currently active
        int activeAmount = activeFields.Count;

        Debug.Log("Pixel Height: " + canvasHeight + ", Log Window Size: " + logSize + ", Offset: " + yOffset + ", Slots: " + slotAmount + ", Active Slots: " + activeAmount);

        //If more slots could fit into the log, add them
        if (activeAmount < slotAmount)
        {
            for (int i = activeAmount; i < slotAmount; i++)
            {
                textFields[i].gameObject.SetActive(true);
                activeFields.Add(textFields[i].gameObject);
            }
        }
        //Otherwise remove the excess slots
        else if (activeAmount > slotAmount)
        {
            for (int i = activeAmount; i > slotAmount; i--)
            {
                textFields[i - 1].gameObject.SetActive(false);
                activeFields.Remove(textFields[i - 1].gameObject);
            }

            //Cleanup
            for (int i = activeAmount; i < textFields.Length; i++)
            {
                textFields[i].gameObject.SetActive(false);
            }
        }
    }

    void FillTextFieldArray()
    {
        int count = textFields.Length;

        for(int i = 0; i < count; i++)
        {
            textFields[i] = entryHolder.transform.GetChild((count - 1) - i).GetComponent<TMP_Text>();
        }
    }
}
