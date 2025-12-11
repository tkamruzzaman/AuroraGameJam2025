using UnityEngine;

public class TempTimelineCharacterController : MonoBehaviour
{
    // The list of GameObjects defining the path points.
    public Transform[] pathPoints; 
    
    // The GameObject that will be moving (the Tiger).
    public Transform tiger; 
    
    // Movement speed multiplier.
    public float speed = 2.0f; 

    private int currentPointIndex = 0; 
    private float lerpProgress = 0f; 

    private void Start()
    {
        // --- Initialization and Safety Checks ---

        if (pathPoints == null || pathPoints.Length < 2)
        {
            Debug.LogError("The pathPoints array must contain at least two points. Disabling script.");
            enabled = false;
            return;
        }

        if (tiger == null)
        {
            Debug.LogError("The 'tiger' Transform reference is not set. Disabling script.");
            enabled = false;
            return;
        }
        
        // Start the Tiger at the first point's position.
        tiger.position = pathPoints[0].position;
    }

    private void Update()
    {
        // Check if the script is properly initialized before proceeding
        if (pathPoints.Length < 2 || tiger == null)
        {
            return;
        }

        // 1. Define the start and end positions of the current segment
        Vector3 startPosition = pathPoints[currentPointIndex].position;
        
        // Calculate the index of the next point, looping back to 0 at the end.
        int nextPointIndex = (currentPointIndex + 1) % pathPoints.Length;
        Vector3 targetPosition = pathPoints[nextPointIndex].position;
        
        // 2. Calculate Distance and Normalize Speed
        float distance = Vector3.Distance(startPosition, targetPosition);
        
        // Safety check to prevent division by zero or large spikes
        if (distance < 0.001f)
        {
            distance = 0.001f; 
        }
        
        // 3. Increment the Lerp progress
        // Time.deltaTime * speed / distance ensures the speed is constant regardless of segment length.
        lerpProgress += Time.deltaTime * speed / distance;
        lerpProgress = Mathf.Clamp01(lerpProgress);

        // 4. Move the Tiger using Lerp
        tiger.position = Vector3.Lerp(startPosition, targetPosition, lerpProgress);

        // 5. Check if the segment is finished
        if (lerpProgress >= 1f)
        {
            // Move to the next target point
            currentPointIndex = nextPointIndex;
            
            // Reset progress for the start of the new segment
            lerpProgress = 0f; 
        }
    }
}