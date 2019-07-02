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

    private TMP_InputField login_Input;

    private bool login;


    void Start()
    {
        login_Input = login_Panel.GetComponentInChildren<TMP_InputField>();

        assistant = transform.GetComponent<Assistant>();
        console = transform.GetComponent<BootConsole>();

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

        //Login Loading Logo Animation:
        while (t < 1)
        {
            t = (Time.time - start) / loginAnimDuration;

            //Get the appropriate rotation from the curve and rotate the logo
            float rotation = (360 * loginAnimTurnCount) * loginAnimCurve.Evaluate(t);
            Vector3 rotationVector = new Vector3(0, 0, rotation);
            loginAnimLogo.transform.eulerAngles = rotationVector;

            yield return null;
        }

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

        console.Launch();

        string s = "Welcome " + PlayerPrefs.GetString("Username", "UNKNOWN") + ", how are you today?";
        StartCoroutine(assistant.DisplayMessage(s, 1F));
        StartCoroutine(assistant.HideMessage(4F));

        s = "I'm Jarvis, your personal assistant, here to help whenever you need me";
        StartCoroutine(assistant.DisplayMessage(s, 5F));
        StartCoroutine(assistant.HideMessage(9F));

        s = "Try typing 'help' in the console to see a list of available commands";
        StartCoroutine(assistant.DisplayMessage(s, 10F));
        StartCoroutine(assistant.HideMessage(13F));


        yield return null;
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

        //Loading Screen Animation
        //Repeats as many times as there are spins 
        for (int i = 0; i < loadingAnimTurnCount; i++)
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

            yield return new WaitForSeconds(spinAnimPause);
        }

        //Load the scene
        SceneManager.LoadSceneAsync(sceneToLoad);

        yield return null;
    }
}
