using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Range(0, 1)]
    public float sensitivity;
    [Range(0, 1)]
    public float verticalMinPixel;
    [Range(0, 1)]
    public float verticalMaxPixel;
    [Range(0, 1)]
    public float horizontalMinPixel;
    [Range(0, 1)]
    public float horizontalMaxPixel;


    private Vector3 offset;
    private Vector3 lookPosition;

    private GameObject player;
    private Camera cam;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cam = GetComponent<Camera>();

        offset = player.transform.position - transform.position;
    }

    void Update()
    {
        GetInput();
    }

    void FixedUpdate()
    {
        float desiredangleX = transform.eulerAngles.x;
        float desiredangleY = transform.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(desiredangleX, desiredangleY, 0);
        transform.position = player.transform.position - (rotation * offset);


        float middleWidth = Screen.width / 2;
        float middleHeight = Screen.height / 2;
        float mouseX = Input.mousePosition.x - middleWidth;
        float mouseY = Input.mousePosition.y - middleHeight;
        float minX = horizontalMinPixel * middleWidth;
        float minY = verticalMinPixel * middleHeight;
        float maxX = horizontalMaxPixel * middleWidth;
        float maxY = verticalMaxPixel * middleHeight;
        float xRotation = 0;
        float yRotation = 0;

        if (mouseX > minX)
        {
            mouseX -= minX;
            xRotation = Mathf.Clamp(mouseX, 0, maxX);
        }
        else if(mouseX < -minX)
        {
            mouseX += minX;
            xRotation = Mathf.Clamp(mouseX, -maxX, 0);
        }
        if (mouseY > minY)
        {
            mouseY -= minY;
            yRotation = Mathf.Clamp(mouseY, 0, maxY);
        }
        else if (mouseY < -minY)
        {
            mouseY += minY;
            yRotation = Mathf.Clamp(mouseY, -maxY, 0);
        }

        transform.Rotate(new Vector3(-yRotation * sensitivity, xRotation * sensitivity, 0));
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0));
    }

    void GetInput()
    {

    }

}
