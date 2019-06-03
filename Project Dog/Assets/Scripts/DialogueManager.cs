using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    //Debug
    public InputField v_moveIF;
    public InputField v_followIF;
    public InputField s_playerIF;

    public GameObject dog;
    public GameObject player;
    public InputField inputField;
    public Text textField;
    public float slowedTime = 0.2F;

    private bool commanding;
    private bool dialogueAnalyzed;
    private DogScript dogS;
    private PlayerScript playerS;

    //0 = fresh; 1 = missing move location; 2 = missing follow target
    private int trainOfThoughts;

    void Start()
    {
        inputField.gameObject.SetActive(false);
        textField.gameObject.SetActive(false);

        dogS = dog.GetComponent<DogScript>();
        playerS = player.GetComponent<PlayerScript>();
    }

    void Update()
    {
        ListenForInput();


        //DEBUG
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayerPrefs.SetString("v_move", v_moveIF.text);
            PlayerPrefs.SetString("v_follow", v_followIF.text);
            PlayerPrefs.SetString("s_player", s_playerIF.text);
        }
    }

    void ListenForInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) && commanding == false)
        {
            OpenDialogue();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && commanding == true && dialogueAnalyzed == false)
        {
            AnalyzeInput();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && commanding == true && dialogueAnalyzed == true)
        {
            CloseDialogue();
        }
    }

    void OpenDialogue()
    {
        commanding = true;
        dialogueAnalyzed = false;
        trainOfThoughts = 0;

        inputField.gameObject.SetActive(true);
        inputField.Select();
        inputField.ActivateInputField();
        textField.gameObject.SetActive(true);

        //Time.timeScale = slowedTime;
        //Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }

    void CloseDialogue()
    {
        commanding = false;

        inputField.text = "";
        inputField.gameObject.SetActive(false);
        inputField.DeactivateInputField();
        textField.gameObject.SetActive(false);
        textField.text = "";

        //Time.timeScale = 1;
        //Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }

    void AnalyzeInput()
    {
        string input = inputField.text;

        switch (trainOfThoughts)
        {
            case 0:
                //Fresh thought
                if (input.Contains("stop"))
                {
                    if (input.Contains(PlayerPrefs.GetString("v_follow")))
                    {
                        dogS.CMDStopFollow();
                        RespondFlat(6);
                        return;
                    }

                    dogS.CMDStop();
                    RespondFlat(5);
                    return;
                }
                else if (input.Contains("go"))
                {
                    dogS.CMDGo();
                    RespondFlat(2);
                    return;
                }
                else if (input.Contains(PlayerPrefs.GetString("v_move")))
                {
                    //IF LOCATION ETC
                    RespondFlat(1);
                    return;
                }
                else if (input.Contains(PlayerPrefs.GetString("v_follow")))
                {
                    if (input.Contains(PlayerPrefs.GetString("s_player")))
                    {
                        dogS.CMDFollow(player);

                        RespondFlat(4);
                        return;
                    }
                    else
                    {
                        trainOfThoughts = 2;
                        RespondAsk(1);
                        return;
                    }
                }
                else
                {
                    //If nothing is found answer with "not understood" sample
                    RespondFlat(0);
                    return;
                }

            case 1:
                //else
                //{
                //    //If nothing is found answer with "not understood" sample
                //    RespondFlat(0);
                //    return;
                //}

            case 2:
                //Missing follow target
                if (input.Contains(PlayerPrefs.GetString("s_player")))
                {
                    dogS.CMDFollow(player);

                    RespondFlat(4);
                    return;
                }
                else
                {
                    //If nothing is found answer with "not understood" sample
                    RespondFlat(0);
                    return;
                }

            default:
                break;
        }

    }

    void RespondAsk(int index)
    {
        string response = "";

        switch (index)
        {
            case 0:
                //Missing Location
                response = "Where should I " + PlayerPrefs.GetString("v_move") + "?";
                break;
            case 1:
                //Missing Target
                response = "Who should I " + PlayerPrefs.GetString("v_follow") + "?";
                break;
            default:
                break;
        }

        textField.text = response;
        inputField.text = "";
        inputField.Select();
        inputField.ActivateInputField();
    }

    void RespondFlat(int index)
    {
        string response = "";

        switch (index)
        {
            case 0:
                //Not understood
                response = "Sorry, I couldn't understand that.";
                break;
            case 1:
                //Bad Excuse
                response = "I don't think this feature was programmed into my brain yet...";
                break;
            case 2:
                //Acknowledgement 
                response = "Sure!";
                break;
            case 3:
                //Acknowledgement Move
                response = "Okay, I'll " + PlayerPrefs.GetString("v_move") + " there.";
                break;
            case 4:
                //Acknowledgement Follow
                response = "I will " + PlayerPrefs.GetString("v_follow") + " " + PlayerPrefs.GetString("s_player") + " then!";
                break;
            case 5:
                //Acknowledgement Stop
                response = "I'll stop.";
                break;
            case 6:
                //Acknowledgement Stop Follow
                response = "Ok I'll stop " + PlayerPrefs.GetString("v_follow") + " " + PlayerPrefs.GetString("s_player") + ".";
                break;
            default:
                break;
        }

        textField.text = response;

        dialogueAnalyzed = true;
    }

    
}
