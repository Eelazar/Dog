using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BootManager : MonoBehaviour
{
    #region Editor Variables
    [Header("Object References")]
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
    [SerializeField]
    [Tooltip("The login loading screen logo")]
    private GameObject loginAnimLogo;
    [SerializeField]
    [Tooltip("The explorer window")]
    private GameObject explorerWindow;
    [SerializeField]
    [Tooltip("The decryption window")]
    private GameObject decryptionWindow;

    [Header("Login Logo Animation")]
    [SerializeField]
    [Tooltip("The duration of the login-loading logo animation")]
    private float loginAnimDuration;
    [SerializeField]
    [Tooltip("The amout of turns during the login-loading logo animation")]
    private int loginAnimTurnCount;
    [SerializeField]
    [Tooltip("The Animation Curve for the rotation speed during the login animation")]
    private AnimationCurve loginAnimCurve;

    [Header("Explorer Launch Animation")]
    [SerializeField]
    [Tooltip("The duration of the explorer launch Animation")]
    private float explorerAnimDuration;

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
    [SerializeField]
    [Tooltip("The duration of the pause between the Logo Animation and the Fade Animation")]
    private float logoAnimPause;

    [Header("Fade-To-Black Animation")]
    [SerializeField]
    [Tooltip("The duration of the fade-to-black animation")]
    private float windowAnimDuration;
    [SerializeField]
    [Tooltip("The duration of the pause between the Fade Animation and the Loading Animation ")]
    private float fadeAnimPause;

    [Header("Logo Animation (Loading Screen)")]
    [SerializeField]
    [Tooltip("The duration of one spin in the loading screen")]
    private float loadingAnimDuration;
    [SerializeField]
    [Tooltip("The amount of logo spins in the loading screen")]
    private int loadingAnimTurnCount;
    [SerializeField]
    [Tooltip("The duration of the pause between each spin during the Loading Animation")]
    private float spinAnimPause;

    [Header("Other")]
    [SerializeField]
    [Tooltip("The Scene that will be loaded upon finsihing the loading animation")]
    private string sceneToLoad;
    #endregion Editor Variables

    public GameObject loginMaster_Object;
    public GameObject login_Panel;
    public GameObject access_Panel;

    public GameObject osMaster_Object;


    private Assistant assistant;
    private BootConsole console;
    private BootExplorer explorer;
    private DecryptionSoftware decryptor;

    private TMP_InputField login_Input;

    private bool login;


    void Start()
    {
        login_Input = login_Panel.GetComponentInChildren<TMP_InputField>();

        assistant = transform.GetComponent<Assistant>();
        console = transform.GetComponent<BootConsole>();
        explorer = transform.GetComponent<BootExplorer>();
        decryptor = transform.GetComponent<DecryptionSoftware>();

        loadingEye.SetActive(false);
        osMaster_Object.SetActive(false);
        loginMaster_Object.SetActive(true);
        access_Panel.SetActive(false);
        login_Panel.SetActive(true);
        loginAnimLogo.SetActive(false);
        login_Input.ActivateInputField();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && login_Input.text != "" && login == false)
        {
            login = true;

            PlayerPrefs.SetString("Username", login_Input.text);

            

            StartCoroutine(LaunchFakeOS());
        }
    }



    IEnumerator LaunchFakeOS()
    {
        //Initialize Lerp
        float t = 0;
        float start = Time.time;

        login_Panel.SetActive(false);
        loginAnimLogo.SetActive(true);
        float duration = (1.0F + Random.Range(0.5F, 2F));
        yield return new WaitForSeconds(duration);

        loginAnimLogo.SetActive(false);


        access_Panel.SetActive(true);

        yield return new WaitForSeconds(1.5F);

        access_Panel.SetActive(false);

        //Initialize Lerp
        t = 0;
        start = Time.time;
        Color full = loginMaster_Object.GetComponent<Image>().color;
        Color fade = new Color(full.r, full.g, full.b, 0);

        osMaster_Object.SetActive(true);

        //Backfround fade Animation:
        while (t < 1)
        {
            t = (Time.time - start) / 1;

            loginMaster_Object.GetComponent<Image>().color = Vector4.Lerp(full, fade, t);

            yield return null;
        }

        loginMaster_Object.SetActive(false);        

        console.Activate();

        DialogueShortcut(1);
    }

    public IEnumerator LaunchExplorer()
    {
        //Initialize Lerp
        float t = 0;
        float start = Time.time;

        Vector2 oldMin = explorerWindow.GetComponent<RectTransform>().anchorMin;
        Vector2 oldMax = explorerWindow.GetComponent<RectTransform>().anchorMax;

        Color full = explorerWindow.GetComponent<Image>().color;
        Color clear = new Color(full.r, full.g, full.b, 0);

        while (t < 1)
        {
            t = (Time.time - start) / explorerAnimDuration;

            explorerWindow.GetComponent<RectTransform>().anchorMin = Vector2.Lerp(oldMin, explorer.anchorMin, t);
            explorerWindow.GetComponent<RectTransform>().anchorMax = Vector2.Lerp(oldMax, explorer.anchorMax, t);

            explorerWindow.GetComponent<Image>().color = Vector4.Lerp(clear, full, t);

            yield return null;
        }

        yield return new WaitForSeconds(0.2F);

        StartCoroutine(explorer.UpdateData());

        yield return new WaitForSeconds(1F);

        DialogueShortcut(2);  
    }

    public IEnumerator LaunchDecryptor()
    {
        StartCoroutine(console.Deactivate());

        //Initialize Lerp
        float t = 0;
        float start = Time.time;

        Vector2 oldMin = decryptionWindow.GetComponent<RectTransform>().anchorMin;
        Vector2 oldMax = decryptionWindow.GetComponent<RectTransform>().anchorMax;

        Color full = decryptionWindow.GetComponent<Image>().color;
        full = new Color(full.r, full.g, full.b, 255);
        Color clear = new Color(full.r, full.g, full.b, 0);

        while (t < 1)
        {
            t = (Time.time - start) / explorerAnimDuration;

            decryptionWindow.GetComponent<RectTransform>().anchorMin = Vector2.Lerp(oldMin, decryptor.anchorMin, t);
            decryptionWindow.GetComponent<RectTransform>().anchorMax = Vector2.Lerp(oldMax, decryptor.anchorMax, t);

            decryptionWindow.GetComponent<Image>().color = Vector4.Lerp(clear, full, t);

            yield return null;
        }
    }

    public IEnumerator CloseDecryptor()
    {
        console.Activate();

        //Initialize Lerp
        float t = 0;
        float start = Time.time;

        Vector2 oldMin = decryptionWindow.GetComponent<RectTransform>().anchorMin;
        Vector2 oldMax = decryptionWindow.GetComponent<RectTransform>().anchorMax;

        Color full = decryptionWindow.GetComponent<Image>().color;
        full = new Color(full.r, full.g, full.b, 255);
        Color clear = new Color(full.r, full.g, full.b, 0);

        while (t < 1)
        {
            t = (Time.time - start) / explorerAnimDuration;

            decryptionWindow.GetComponent<RectTransform>().anchorMin = Vector2.Lerp(oldMin, new Vector2(0.15F, 0.05F), t);
            decryptionWindow.GetComponent<RectTransform>().anchorMax = Vector2.Lerp(oldMax, new Vector2(0.15F, 0.05F), t);

            decryptionWindow.GetComponent<Image>().color = Vector4.Lerp(full, clear, t);

            yield return null;
        }
    }

    public IEnumerator AnimateStart()
    {
        //Block input
        console.loading = true;

        //Initialize Lerp
        float tLogo = 0;
        float start = Time.time;


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

        yield return new WaitForSeconds(logoAnimPause);

        //Initialize Lerp
        float tFade = 0;
        start = Time.time;

        transitionBlack.SetActive(true);

        //Get the anchors of the fade-to-black image in advance
        Vector2 minAnchor = transitionBlack.GetComponent<RectTransform>().anchorMin;
        Vector2 maxAnchor = transitionBlack.GetComponent<RectTransform>().anchorMax;

        //Fade-To-Black Animation
        while (tFade < 1)
        {
            tFade = (Time.time - start) / windowAnimDuration;

            //Use the anchors to scale the image evenly to achieve a smooth transition to a black screen
            transitionBlack.GetComponent<RectTransform>().anchorMin = Vector2.Lerp(minAnchor, new Vector2(0F, 0F), tFade);
            transitionBlack.GetComponent<RectTransform>().anchorMax = Vector2.Lerp(maxAnchor, new Vector2(1F, 1F), tFade);

            yield return null;
        }

        yield return new WaitForSeconds(fadeAnimPause);

        console.window.SetActive(false);
        loadingEye.SetActive(true);
        loadingText.SetActive(true);

        loadingEye.SetActive(true);
        float duration = (1.933F + Random.Range(0.5F, 3F));
        yield return new WaitForSeconds(duration);

        //Load the scene
        SceneManager.LoadSceneAsync(sceneToLoad);

        yield return null;
    }

    public void DialogueShortcut(int i)
    {
        string s = "";

        switch (i)
        {
            case 1:
                s = "Welcome back " + PlayerPrefs.GetString("Username", "UNKNOWN") + ", how are you today?";
                assistant.QueueMessage(new Message(s, 0, 5F, true));

                s = "I'm Neptune, your personal assistant, here to help whenever you need me, as usual.";
                assistant.QueueMessage(new Message(s, 0, 5F));

                s = "A new software update has been deployed while you were absent.";
                assistant.QueueMessage(new Message(s, 0, 4F));

                s = "Update v1.0.4 has improved the security and wireless access protocols.";
                assistant.QueueMessage(new Message(s, 0, 6F));

                s = "Try typing 'launch explorer' in the console to see available information and get started on your daily tasks.";
                assistant.QueueMessage(new Message(s, 0, 60F, false, true));

                break;

            case 2:
                s = "Well done! This is your new content explorer.";
                assistant.QueueMessage(new Message(s, 0, 4F, true, false, true));

                s = "The new update allows you to type 'open ' followed by a node name to access it.";
                assistant.QueueMessage(new Message(s, 0, 7F));

                s = "For example, try typing 'open root' to open the Root node";
                assistant.QueueMessage(new Message(s, 0, 7F));

                s = "If you want to return to the previous node, type 'move up'";
                assistant.QueueMessage(new Message(s, 0, 7F));

                s = "That's it for the new update, I'll let you get back to work now";
                assistant.QueueMessage(new Message(s, 0, 4F));

                s = "Just as a reminder: Use 'open ' followed by a node's name, and 'move up' to navigate the explorer.";
                assistant.QueueMessage(new Message(s, 0, 60F, false, true));

                break;

            case 3:

                break;

            case 4:

                break;

            case 5:

                break;

            default:
                break;
        }
    }

}
