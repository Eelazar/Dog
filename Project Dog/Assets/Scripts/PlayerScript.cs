using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerScript : MonoBehaviour
{

    public float movementSpeed;
    public float rotationSpeed;
    public InputField input;
    public float slowedTime;

    private Vector3 movementInput;
    private Vector3 rotationInput;
    private Rigidbody rb;
    private GameObject head;
    private Quaternion lookRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        head = transform.GetChild(0).gameObject;

        input.gameObject.SetActive(false);
    }

    
    void Update()
    {
        GetInput();
    }

    void FixedUpdate()
    {
        TranslateMovementInput();
    }

    void GetInput()
    {
        movementInput.x = Input.GetAxis("Horizontal") * movementSpeed;
        movementInput.z = Input.GetAxis("Vertical") * movementSpeed;

        if (Input.GetKeyDown(KeyCode.Return) && input.gameObject.activeSelf == false)
        {
            ReceiveCommand();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && input.gameObject.activeSelf == true)
        {
            input.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }

    void TranslateMovementInput()
    {
        //Add Velocity relative to head rotation
        Vector3 temp = head.transform.TransformDirection(movementInput);
        temp.y = 0;
        rb.velocity = temp;

        //Rotate head according to camera
        head.transform.rotation = Quaternion.Slerp(head.transform.rotation, Camera.main.transform.rotation, rotationSpeed);
    }

    void ReceiveCommand()
    {
        input.gameObject.SetActive(true);
        input.Select();
        input.ActivateInputField();

        Time.timeScale = slowedTime;
    }
}
