using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserCamera : MonoBehaviour
{
    public bool isFree = false;
    public Transform Camera; // Drag main camera here.

    public float lookRateSpeed = 420f;
    private Vector2 lookIn, screenCentre, mouseDistance;

    private float rollIn;
    public float rollSpeed = 90f;
    public float rollAccel = 90f;


    public float speed = 0.042f; // Movement speed of the camera/user
    public float sens = 1000f; // Rotation sensitivity

    void Start()
    {
        //1. Initiat if the camera is free to move around
        //2. Get the screen centre
        //3. Print somthing to inform that the camera has started
        isFree = false;
        screenCentre.x = Screen.width * 0.5f;
        screenCentre.y = Screen.height * 0.5f;
        Debug.Log("UserCamera started");
        //Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //1. Don't do anything if isFree = false  -> the camera should not move
        //2. Get user input from mouse 
        //      "Horizontal" == x-axis
        //      "Vertical" == z-axis  (usually y-axis but in Unity the default is different
        //      "Jump" == y-axis
        //3. Update the camera position / rotation
        if (!isFree) return;

        //Update POSITION
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * sens;
        //float y = Input.GetAxis("Mouse Y") * sens;
        float y = Input.GetAxis("Jump") * Time.deltaTime * sens;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * sens;

        //Vector3 move = new(x, y, z);
        transform.position += speed * z * transform.forward;
        transform.position += speed * x * transform.right;
        transform.position += speed * y * transform.up;


        //Update ROTATION
        lookIn.x = Input.mousePosition.x;
        lookIn.y = Input.mousePosition.y;

        mouseDistance.x = (lookIn.x - screenCentre.x) / screenCentre.y;
        mouseDistance.y = (lookIn.y - screenCentre.y) / screenCentre.y;

        mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);

        rollIn = Mathf.Lerp(rollIn, Input.GetAxis("Roll"), rollAccel * Time.deltaTime);

        float xRot = mouseDistance.x * lookRateSpeed * Time.deltaTime;
        float yRot = mouseDistance.y * lookRateSpeed * Time.deltaTime;
        float zRot = rollIn *rollSpeed *Time.deltaTime;
        Vector3 rotate = new(-yRot, xRot, zRot);
        transform.Rotate(rotate, Space.Self);    

    }

}
