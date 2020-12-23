using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    public float Speed = 3.0f;

    float actorRotationX;
    float actorRotationY;
   
    void Start()
    {
      

    }

    void Update()
    {
        UpdateRotation();
        UpdateMovement();
    }

    void UpdateMovement()
    {
        float forwardInput = Input.GetAxis("Vertical") * Time.deltaTime * Speed;
        float rightInput = Input.GetAxis("Horizontal") * Time.deltaTime * Speed;

        // z축을 입력한 만큼 추가
        Vector3 left = transform.position;
        left.z += forwardInput;
        left.x += rightInput;
        transform.position = left;
    }

    void UpdateRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        actorRotationX += mouseX;
        actorRotationY += mouseY;

        actorRotationX %= 360.0f;
        actorRotationY = Mathf.Clamp(actorRotationY, -90.0f, 90.0f);

        transform.rotation = Quaternion.Euler(-actorRotationY, actorRotationX, 0);
    }

}