using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private bool paused;
    private float timeBackup;

    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && paused == false)
        {
            timeBackup = Time.timeScale;
            Time.timeScale = 0;
            paused = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && paused == true)
        {
            Time.timeScale = timeBackup;
            paused = false;
        }
    }
}
