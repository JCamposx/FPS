using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float turnSpeed;

    private Rigidbody mRb;
    private Vector2 mDirection;
    private Vector2 mDeltaLook;
    private Transform cameraMain;

    private void Start()
    {
        mRb = GetComponent<Rigidbody>();
        cameraMain = transform.Find("Main Camera");
    }

    private void Update()
    {
        mRb.velocity = mDirection.y * speed * transform.forward;

        Debug.DrawRay(
            transform.position,
            transform.forward,
            Color.red
        );
        
        transform.Rotate(
            Vector3.up,
            turnSpeed * Time.deltaTime * mDeltaLook.x
        );
        cameraMain.GetComponent<CameraMovement>().RotateUpDown(
            -turnSpeed * Time.deltaTime * mDeltaLook.y
        );

    }

    private void OnMove(InputValue value)
    {
        mDirection = value.Get<Vector2>();
    }

    private void OnLook(InputValue value)
    {
        mDeltaLook = value.Get<Vector2>();
    }
}
