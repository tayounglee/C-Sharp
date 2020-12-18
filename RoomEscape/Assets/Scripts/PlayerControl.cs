using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float upDownRange = 90;

    private Vector3 speed;
    private float forwardSpeed;
    private float sideSpeed;

    private float rotateLeftRight;
    private float verticalRotation = 0f;

    private float verticalVelocity = 0f;


    private CharacterController charactercontroller;

    void Start()
    {
        charactercontroller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

    }

    void Update()
    {

        PlayerMove();
        PlayerRotate();
    }


    //Player의 x축, z축 움직임을 담당
    void PlayerMove()
    {
        forwardSpeed = Input.GetAxis("Vertical") * movementSpeed;
        sideSpeed = Input.GetAxis("Horizontal") * movementSpeed;

        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        speed = new Vector3(sideSpeed, verticalVelocity, forwardSpeed);
        speed = transform.rotation * speed;

        charactercontroller.Move(speed * Time.deltaTime);
    }

    //Player의 회전을 담당
    void PlayerRotate()
    {
        //좌우 회전
        rotateLeftRight = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0f, rotateLeftRight, 0f);

        //상하 회전
        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}