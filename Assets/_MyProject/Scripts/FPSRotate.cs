using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSRotate : MonoBehaviour
{
    public Camera cam;          //Main camera for rotate in y-axis
    void Update()               //Updated every frame;
    {
        this.LookRotation(transform, cam.transform);    //Call LookRotation() to change the x-rotation of the gameobject and the y-rotation of camera
    }
    public void LookRotation(Transform character, Transform camera)     //Change the x-rotation of the gameobject and the y-rotation of camera
    {
        float yRot = Input.GetAxis("Mouse X") * 1.5f;     //get x and y of mouse in screen
        float xRot = Input.GetAxis("Mouse Y") * 1.5f;
        character.localRotation *= Quaternion.Euler(0f, yRot, 0f);      //To change character's rotation around y-axis
        camera.localRotation *= Quaternion.Euler(-xRot, 0f, 0f);        //To change camera's rotation around x-axis
        camera.localRotation = ClampRotationAroundXAxis(camera.localRotation);  //Clamp camera's rotation
    }                                                                   //The key point is use localRotation,not rotation or Quaternion.Rotate.
    Quaternion ClampRotationAroundXAxis(Quaternion q)       //The method of clamp rotation,I can't understand it;use it carefully.
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;
        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, -90f, 90f);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);
        return q;
    }
}
