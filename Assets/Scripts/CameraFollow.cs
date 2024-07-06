using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform target; // The target that the camera will follow
    public float smoothSpeed = 0.125f; // The speed at which the camera will smooth
    public Vector3 offset = new Vector3(0, 0, -10); // The offset of the camera from the target

    void LateUpdate()
    {
        if (target != null)
        {
            // Only update the X and Y positions
            Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y, -10f);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
        else
        {
            Debug.LogWarning("CameraFollow: No target set for the camera to follow.");
        }
    }
}
