using UnityEngine;

public class CameraController : MonoBehaviour
{

[SerializeField] Camera mainCamera;

    [SerializeField] private Transform target; 
    [SerializeField] private Vector3 offset;   
    [SerializeField] private float smoothSpeed = 0.125f;



    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(mainCamera.transform.position, desiredPosition, smoothSpeed);
        mainCamera.transform.position = smoothedPosition;

        mainCamera.transform.LookAt(target);
    }
}
