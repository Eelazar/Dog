using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class BootConsole : MonoBehaviour
{
    public TMP_InputField console_InputField;
    public TMP_Text[] log_TextFields;
    public GameObject consolePanel;

    public GameObject window;
    public GameObject programLogo;
    public GameObject transitionBlack;
    public GameObject loadingEye;
    public GameObject loadingText;
    public float textSpeed;
    public float windowAnimDuration;
    public float logoAnimDuration;
    public float loadingAnimDuration;
    public int loadingAnimTurnCount;
    public float logoAnimMaxScale;
    public AnimationCurve logoAnimCurve_Scale;
    public AnimationCurve logoAnimCurve_Rotation;

    public string sceneToLoad;

    private AudioSource keySource;
    private List<GameObject> log_ActiveFields;

    private string rawInput;
    private int currentLogIndex;
    private int selectedSlotIndex;

    private string[] consoleLog = new string[50];

    private bool loading;

    void Start()
    {
        keySource = gameObject.GetComponent<AudioSource>();

        console_InputField.ActivateInputField();

        log_ActiveFields = new List<GameObject>();

        //Suggest Help
        LogText("Type 'help' to see a list of available commands");
    }

    void Update()
    {
        GetInput();

        if (Input.anyKeyDown)
        {
            keySource.pitch = UnityEngine.Random.Range(0.5F, 3F);
            keySource.Play();
        }

        float yOffset = consolePanel.GetComponent<RectTransform>().sizeDelta.y;
        Vector2 parentAnchorMin = new Vector2(window.GetComponent<RectTransform>().anchorMin.x, window.GetComponent<RectTransform>().anchorMin.y);
        Vector2 parentAnchorMax = new Vector2(window.GetComponent<RectTransform>().anchorMax.x, window.GetComponent<RectTransform>().anchorMax.y);
        Vector2 parentScreenPercent = new Vector2(parentAnchorMax.x - parentAnchorMin.x, parentAnchorMax.y - parentAnchorMin.y);

        float canvasHeight = Screen.height;
        float logSize = (canvasHeight * parentScreenPercent.y) + yOffset;

        int slotAmount = Mathf.FloorToInt(logSize / 20);
        int activeAmount = log_ActiveFields.Count;
        Debug.Log("Pixel Height: " + canvasHeight + ", Log Window Size: " + logSize + ", Offset: " + yOffset + ", Slots: " + slotAmount + ", Active Slots: " + activeAmount);
        if(activeAmount < slotAmount)
        {
            for (int i = activeAmount; i < slotAmount; i++)
            {
                log_TextFields[i].gameObject.SetActive(true);
                log_ActiveFields.Add(log_TextFields[i].gameObject);
            }
        }
        else if (activeAmount > slotAmount)
        {
            for (int i = activeAmount; i > slotAmount; i--)
            {
                log_TextFields[i-1].gameObject.SetActive(false);
                log_ActiveFields.Remove(log_TextFields[i-1].gameObject);
            }

            //Cleanup
            for (int i = activeAmount; i < log_TextFields.Length; i++)
            {
                log_TextFields[i].gameObject.SetActive(false);
            }
        }


    }

    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !loading)
        {
            rawInput = console_InputField.text;
            console_InputField.text = "";

            if (rawInput != "")
            {
                LogText(rawInput);

                if (rawInput.Contains("start"))
                {
                    Debug.Log("Start Command");
                    StartCoroutine(AnimateStart());
                }
            }

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
                StartCoroutine(AnimateText(log_TextFields[i], consoleLog[currentLogIndex - selectedSlotIndex + i]));
            }
            else
            {
                log_TextFields[i].text = consoleLog[currentLogIndex - selectedSlotIndex + i];
            }
        }
    }

    IEnumerator AnimateText(TMP_Text ui, string s)
    {
        ui.text = "";

        foreach (char c in s)
        {
            ui.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        yield return null;
    }

    IEnumerator AnimateStart()
    {
        loading = true;

        float tLogo = 0;
        float start = Time.time;

        Vector2 minAnchor = transitionBlack.GetComponent<RectTransform>().anchorMin;
        Vector2 maxAnchor = transitionBlack.GetComponent<RectTransform>().anchorMax;



        while (tLogo < 1)
        {
            tLogo = (Time.time - start) / logoAnimDuration;

            float scale = 1 + logoAnimMaxScale * logoAnimCurve_Scale.Evaluate(tLogo);
            programLogo.transform.localScale = Vector2.LerpUnclamped(new Vector2(1, 1), new Vector2(scale, scale), tLogo);


            float rotation = 360 * logoAnimCurve_Rotation.Evaluate(tLogo);
            Vector3 rotationVector = new Vector3(0, 0, rotation);
            programLogo.transform.eulerAngles = rotationVector;

            yield return null;
        }

        yield return new WaitForSeconds(0.5F);

        float tFade = 0;
        start = Time.time;

        while (tFade < 1)
        {
            tFade = (Time.time - start) / windowAnimDuration;

            transitionBlack.GetComponent<RectTransform>().anchorMin = Vector2.Lerp(minAnchor, new Vector2(0F, 0F), tFade);
            transitionBlack.GetComponent<RectTransform>().anchorMax = Vector2.Lerp(maxAnchor, new Vector2(1F, 1F), tFade);

            yield return null;
        }

        window.SetActive(false);
        loadingEye.SetActive(true);
        loadingText.SetActive(true);

        for(int i = 0; i < loadingAnimTurnCount; i++)
        {
            tLogo = 0;
            start = Time.time;

            while (tLogo < 1)
            {
                tLogo = (Time.time - start) / loadingAnimDuration;

                float scale = 1 + logoAnimMaxScale * logoAnimCurve_Scale.Evaluate(tLogo);
                loadingEye.transform.localScale = Vector2.LerpUnclamped(new Vector2(1, 1), new Vector2(scale, scale), tLogo);


                float rotation = 360 * logoAnimCurve_Rotation.Evaluate(tLogo);
                Vector3 rotationVector = new Vector3(0, 0, rotation);
                loadingEye.transform.eulerAngles = rotationVector;

                yield return null;
            }

            yield return new WaitForSeconds(0.2F);
        }

        SceneManager.LoadSceneAsync(sceneToLoad);

        yield return null;
    }
}
