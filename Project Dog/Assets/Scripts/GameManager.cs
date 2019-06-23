using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    public BaseObject player;

    public BaseObject door1;

    public BaseObject door2;

    private void Awake()
    {
        current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ObjectManager.AddObject("Door1", door1);
        ObjectManager.AddObject("Door2", door2);

        CommandFeedback feedback = CommandManager.ExecuteCommand("Open", "Door1");

        if (!feedback.valid)
            Debug.Log(feedback.feedback);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
