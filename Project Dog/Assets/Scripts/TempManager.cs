using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TempManager : MonoBehaviour
{
    private Assistant assistant;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        assistant = transform.GetComponent<Assistant>();

        DialogueShortcut(1);
    }

    void Update()
    {
        
    }

    public void LaunchFoxStartup()
    {
        DialogueShortcut(2);
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

                m = new Message("Type 'explore unit' to attempt to establish a wireless link between SITWatch and the unit", 0F, 2F, false, true);
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
