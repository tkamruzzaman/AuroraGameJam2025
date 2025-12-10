using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AuroraPointsConnector : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public LayerMask targetLayerRenderer;
    
    [Header("Target Configuration")]
    public string targetTag = "ConnectionPoint"; 
    
    private Camera mainCamera;
    private bool _isDrawing;
    private int requiredTargetCount; 
    
    public List<GameObject> connectedObjects = new List<GameObject>();
    public List<Vector3> drawPositions = new List<Vector3>();

    private void Start()
    {
        mainCamera = Camera.main;
        lineRenderer.gameObject.SetActive(false);
        lineRenderer.positionCount = 0;

        // Determine the total number of required targets ONCE at startup
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag(targetTag);
        requiredTargetCount = allTargets.Length;
        
        Debug.Log($"Total required connection targets found: {requiredTargetCount}.");
    }

    private void Update()
    {
        // === MOUSE DOWN (Start Drawing) ===
        if (Input.GetMouseButtonDown(0))
        {
            if (!_isDrawing && lineRenderer.gameObject.activeSelf)
            {
                ResetConnection(shouldClearLine: true); 
            }
            
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var raycastHit, maxDistance: 100f, (int)targetLayerRenderer))
            {
                var hitObject = raycastHit.transform.gameObject;

                if (hitObject.CompareTag(targetTag))
                {
                    _isDrawing = true;
                    connectedObjects.Clear(); 
                    connectedObjects.Add(hitObject);
                    lineRenderer.gameObject.SetActive(true);
                }
            }
        }

        // === MOUSE HELD (Continue Connecting Points - Allows Reconnection) ===
        if (Input.GetMouseButton(0) && _isDrawing)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var raycastHit , maxDistance: 100f, (int)targetLayerRenderer))
            {
                var hitObject = raycastHit.collider.gameObject;

                if (hitObject.CompareTag(targetTag))
                {
                    // If the hit object is different from the LAST connected object, add it.
                    // This allows any re-connection (including back to the start) but prevents spamming the list.
                    if (connectedObjects.Count == 0 || hitObject != connectedObjects.Last())
                    {
                        connectedObjects.Add(hitObject);
                        Debug.Log($"Connected to sphere: {hitObject.name}. List size: {connectedObjects.Count}");
                    }
                }
            }
        }

        DrawLine(); 

        // === MOUSE UP (Stop Drawing/Validation Check) ===
        if (Input.GetMouseButtonUp(0) && _isDrawing)
        {
            _isDrawing = false;
            
            if (CheckForCompleteConnection())
            {
                Debug.Log("🎉 Successful Aurora Connection! All unique spheres connected.");
                HandleSuccessfulConnection();
            }
            else
            {
                Debug.Log($"Connection failed. Visited {connectedObjects.Distinct().Count()} out of {requiredTargetCount} unique targets.");
                ResetConnection(shouldClearLine: true); 
            }
        }
    }
    
    // --- Utility Methods ---

    private bool CheckForCompleteConnection()
    {
        // Success: The number of unique objects visited equals the total required count.
        return connectedObjects.Distinct().Count() == requiredTargetCount;
    }

    private void HandleSuccessfulConnection()
    {
        _isDrawing = false; 

        // Finalize the line (Remove the segment that was leading to the cursor)
        drawPositions.Clear();
        foreach (var obj in connectedObjects)
        {
            drawPositions.Add(obj.transform.position);
        }
        lineRenderer.positionCount = drawPositions.Count;
        lineRenderer.SetPositions(drawPositions.ToArray());
        
        Debug.Log("PERFORMING ACTION: Line kept active, now triggering game event/handle.");
    }

    private void ResetConnection(bool shouldClearLine)
    {
        _isDrawing = false;
        connectedObjects.Clear();
        drawPositions.Clear();
        
        if (shouldClearLine)
        {
            lineRenderer.positionCount = 0;
            lineRenderer.gameObject.SetActive(false);
        }
    }

    // --- Drawing Methods ---
    
    public void DrawLine()
    {
        // Stop drawing if the mouse is up or we are not in the drawing state
        if (!Input.GetMouseButton(0) || !_isDrawing || connectedObjects.Count == 0)
        {
            return;
        }

        drawPositions.Clear();
        
        foreach (var obj in connectedObjects)
        {
            drawPositions.Add(obj.transform.position);
        }

        // Add the current cursor position
        Vector3 inputDrawPosition = GetMouseWorldPosition();
        drawPositions.Add(inputDrawPosition);

        lineRenderer.positionCount = drawPositions.Count;
        lineRenderer.SetPositions(drawPositions.ToArray());
    }

    public Vector3 GetMouseWorldPosition()
    {
        var targetInputPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        return targetInputPos;
    }
}