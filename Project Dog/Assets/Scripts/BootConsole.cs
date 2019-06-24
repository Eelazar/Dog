using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class BootConsole : MonoBehaviour
{
    //Constants
    private const float lineHeight = 20F;

    #region Editor Variables
    [Header("Object References")]
    [SerializeField]
    [Tooltip("All the textfields from bottom to top")]
    private TMP_Text[] log_TextFields;
    [SerializeField]
    [Tooltip("The console InputField object")]
    private TMP_InputField console_InputField;
    [SerializeField]
    [Tooltip("The object containing the taskbar logo")]
    private GameObject programLogo;
    [SerializeField]
    [Tooltip("The object that is used for the fade-to-black transition")]
    private GameObject transitionBlack;
    [SerializeField]
    [Tooltip("The object containing the loading screen logo")]
    private GameObject loadingEye;
    [SerializeField]
    [Tooltip("The object containing the loading screen text")]
    private GameObject loadingText;

    [Header("Text Animation")]
    [SerializeField]
    [Tooltip("The duration of the pause between each letter during typewriter animation")]
    private float textSpeed;

    [Header("Logo Animation (Taskbar & Loading Screen)")]
    [SerializeField]
    [Tooltip("The duration of the taskbar-logo animation")]
    private float logoAnimDuration;
    [SerializeField]
    [Tooltip("The highest scale the logo will reach during its animation")]
    private float logoAnimMaxScale;
    [SerializeField]
    [Tooltip("The Animation Curve for the scaling changes during the logo animation")]
    private AnimationCurve logoAnimCurve_Scale;
    [SerializeField]
    [Tooltip("The Animation Curve for the rotation speed during the logo animation")]
    private AnimationCurve logoAnimCurve_Rotation;

    [Header("Fade-To-Black Animation")]
    [SerializeField]
    [Tooltip("The duration of the fade-to-black animation")]
    private float windowAnimDuration;

    [Header("Logo Animation (Loading Screen)")]
    [SerializeField]
    [Tooltip("The duration of one spin in the loading screen")]
    private float loadingAnimDuration;
    [SerializeField]
    [Tooltip("The amount of logo spins in the loading screen")]
    private int loadingAnimTurnCount;

    [Header("Other")]
    [SerializeField]
    [Tooltip("The Scene that will be loaded upon finsihing the loading animation")]
    private string sceneToLoad;
    #endregion Editor Varibles

    #region Private Variables
    ////Object References
    //The panel containing the console panel and input panel
    private GameObject window;
    //The panel containing all the log TextFields
    private GameObject consolePanel;
    //The audio source for the key sounds
    private AudioSource keySource;
    //The dynamic list containing all the text fields that are currently active
    private List<GameObject> log_ActiveFields;

    ////Other Variables
    //Holder for the text input
    private string rawInput;
    //Index for the currently selected log entry
    private int currentLogIndex;
    //Index for the currently selected text field position i.e. slot
    private int selectedSlotIndex;
    //Backup of up to x log entries
    private string[] consoleLog = new string[50];
    //Boolean to lock console entries during animations
    private bool loading;
    #endregion Private Variables

    void Start()
    {
        //Initialize
        log_ActiveFields = new List<GameObject>();

        //Get some stuff
        consolePanel = log_TextFields[0].transform.parent.gameObject;
        window = consolePanel.transform.parent.gameObject;
        keySource = gameObject.GetComponent<AudioSource>();

        //Activate the input field to get the cursor on it
        console_InputField.ActivateInputField();

        //Turn off all text fields
        foreach(TMP_Text tmp in log_TextFields)
        {
            tmp.gameObject.SetActive(false);
        }

        //Suggest Help
        LogText("Type 'help' to see a list of available commands");
    }

    void Update()
    {
        //Listen for key presses
        GetInput();
        
        //Dynamically resize the log
        ResizeLog();
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
                    StartCoroutine(AnimateStart());
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

    IEnumerator AnimateStart()
    {
        //Block input
        loading = true;

        //Initialize Lerp
        float tLogo = 0;
        float start = Time.time;

        //Get the anchors of the fade-to-black image in advance
        Vector2 minAnchor = transitionBlack.GetComponent<RectTransform>().anchorMin;
        Vector2 maxAnchor = transitionBlack.GetComponent<RectTransform>().anchorMax;

        //Taskbar Logo Animation:
        while (tLogo < 1)
        {
            tLogo = (Time.time - start) / logoAnimDuration;

            //Get the appropriate scale from the curve and lerp the logo 
            float scale = 1 + logoAnimMaxScale * logoAnimCurve_Scale.Evaluate(tLogo);
            programLogo.transform.localScale = Vector2.LerpUnclamped(new Vector2(1, 1), new Vector2(scale, scale), tLogo);

            //Get the appropriate rotation from the curve and rotate the logo
            float rotation = 360 * logoAnimCurve_Rotation.Evaluate(tLogo);
            Vector3 rotationVector = new Vector3(0, 0, rotation);
            programLogo.transform.eulerAngles = rotationVector;

            yield return null;
        }

        yield return new WaitForSeconds(0.5F);

        //Initialize Lerp
        float tFade = 0;
        start = Time.time;

        //Fade-To-Black Animation
        while (tFade < 1)
        {
            tFade = (Time.time - start) / windowAnimDuration;

            //Use the anchors to scale the image evenly to achieve a smooth transition to a black screen
            transitionBlack.GetComponent<RectTransform>().anchorMin = Vector2.Lerp(minAnchor, new Vector2(0F, 0F), tFade);
            transitionBlack.GetComponent<RectTransform>().anchorMax = Vector2.Lerp(maxAnchor, new Vector2(1F, 1F), tFade);

            yield return null;
        }

        window.SetActive(false);
        loadingEye.SetActive(true);
        loadingText.SetActive(true);

        //Loading Screen Animation
        //Repeats as many times as there are spins 
        for(int i = 0; i < loadingAnimTurnCount; i++)
        {
            tLogo = 0;
            start = Time.time;

            while (tLogo < 1)
            {
                tLogo = (Time.time - start) / loadingAnimDuration;

                //Get the appropriate scale from the curve and lerp the logo 
                float scale = 1 + logoAnimMaxScale * logoAnimCurve_Scale.Evaluate(tLogo);
                loadingEye.transform.localScale = Vector2.LerpUnclamped(new Vector2(1, 1), new Vector2(scale, scale), tLogo);

                //Get the appropriate rotation from the curve and rotate the logo
                float rotation = 360 * logoAnimCurve_Rotation.Evaluate(tLogo);
                Vector3 rotationVector = new Vector3(0, 0, rotation);
                loadingEye.transform.eulerAngles = rotationVector;

                yield return null;
            }

            yield return new WaitForSeconds(0.2F);
        }

        //Load the scene
        SceneManager.LoadSceneAsync(sceneToLoad);

        yield return null;
    }
}
