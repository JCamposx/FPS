using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 rotation;
    
    public void RotateUpDown(float angle)
    {
        Vector3 newRotation = new Vector3(
            transform.rotation.eulerAngles.x + angle,
            transform.rotation.eulerAngles.y,
            transform.rotation.eulerAngles.z
        );

        transform.rotation = Quaternion.Euler(newRotation);

        rotation = transform.rotation.eulerAngles;
    }
}
