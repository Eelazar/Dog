using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TempManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The decryption window")]
    private GameObject decryptionWindow;

    public float decryptWindowAnimDuration;


    private const string foxXMLFileName = "PlayerFoxExplorerFile.xml";

    private Assistant assistant;
    private Explorer explorer;
    private Console console;
    private DecryptionSoftware decryptor;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        assistant = transform.GetComponent<Assistant>();
        explorer = transform.GetComponent<Explorer>();
        console = transform.GetComponent<Console>();
        decryptor = transform.GetComponent<DecryptionSoftware>();

        DialogueShortcut(1);
    }

    void Update()
    {
        
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
            t = (Time.time - start) / decryptWindowAnimDuration;

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
            t = (Time.time - start) / decryptWindowAnimDuration;

            decryptionWindow.GetComponent<RectTransform>().anchorMin = Vector2.Lerp(oldMin, new Vector2(0.15F, 0.05F), t);
            decryptionWindow.GetComponent<RectTransform>().anchorMax = Vector2.Lerp(oldMax, new Vector2(0.15F, 0.05F), t);

            decryptionWindow.GetComponent<Image>().color = Vector4.Lerp(full, clear, t);

            yield return null;
        }
    }

    public void LaunchFoxStartup()
    {
        DialogueShortcut(2);
    }

    public void ExploreFox()
    {
        explorer.SwitchXML(foxXMLFileName);
    }

    public void DialogueShortcut(int i)
    {
        Message m;

        switch (i)
        {
            case 1:
                m = new Message("Hi there! I hope you don't mind that I've taken the liberty to integrate myself into the program.", 2F, 2F, true);
                assistant.QueueMessage(m);

                m = new Message("As you can see I have also started to log my messages, so you can review anything you missed.", 0F, 1.5F);
                assistant.QueueMessage(m);

                m = new Message("This is the main security terminal.", 0F, 1F);
                assistant.QueueMessage(m);

                m = new Message("The top right corner of the screen displays the factory's surveillance system.", 0F, 1.5F);
                assistant.QueueMessage(m);

                m = new Message("Try typing 'view cam1' to launch the camera.", 0F, 2, false, true);
                assistant.QueueMessage(m);

                break;

            case 2:
                m = new Message("Hmm, strange...", 0F, 0.5F, true, false, true);
                assistant.QueueMessage(m);

                m = new Message("I'm picking up an unknown entity in this room.", 0F, 1F);
                assistant.QueueMessage(m);

                m = new Message("It looks like an early iteration of our fox units... The serial nummer is 001.", 0F, 1.5F);
                assistant.QueueMessage(m);

                m = new Message("I think it's broken. You should probably check just to be sure though.", 0F, 1F);
                assistant.QueueMessage(m);

                m = new Message("Type 'explore fox' to attempt to establish a wireless link between SITWatch and the unit", 0F, 2F, false, true);
                assistant.QueueMessage(m);

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
