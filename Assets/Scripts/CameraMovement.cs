using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 rotation;
    
    public void RotateUpDown(float angle)
    {
        //Vector3 rotationPre = transform.rotation.eulerAngles;

        transform.Rotate(
            Vector3.right,
            angle
        );

        /*rotationPre.x = Mathf.Clamp(
            transform.rotation.eulerAngles.x,
            -90f,
            90f
        );

        transform.rotation = Quaternion.Euler(rotationPre);*/

        /*if (transform.rotation.eulerAngles.x > 80f &&
            transform.rotation.eulerAngles.x < 280f )
        {
            Debug.Log("cp1");
            transform.rotation = Quaternion.Euler(rotationPre);
        }*/

        rotation = transform.rotation.eulerAngles;
    }
}
