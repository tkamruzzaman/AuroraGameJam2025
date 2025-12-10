using UnityEngine;

public class FoxController : MonoBehaviour
{
    public float cameraDistance = 10f;
    public bool smooth = true;
    public float smoothSpeed = 15f;

    void Update()
    {
        
        if (Input.GetMouseButton(0))
        {
            var cam = Camera.main;
            if (cam == null) return;

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = cameraDistance;
            Vector3 targetPosition = cam.ScreenToWorldPoint(mousePos);
            
            // LOCK Z POSITION: Overwrite the calculated Z with the current GameObject's Z
            targetPosition.z = transform.position.z; 

            if (smooth)
            {
                transform.position = Vector3.Lerp(
                    transform.position, 
                    targetPosition, 
                    Time.deltaTime * smoothSpeed
                );
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }
}
