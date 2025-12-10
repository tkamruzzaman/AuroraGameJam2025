using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections; 

public class AuroraPointsConnector : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public LayerMask targetLayerRenderer;
    
    [Header("Target Configuration")]
    public Vector3 foxtailOffset = new Vector3(0f, 0.5f, 0f); 
    public string targetTag = "PathSphere"; 
    
    [Header("Animation")]
    public GameObject foxtail; 
    public float animationSpeed = 5f; 
    
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

        GameObject[] allTargets = GameObject.FindGameObjectsWithTag(targetTag);
        requiredTargetCount = allTargets.Length;
        
        if (foxtail != null)
        {
            if (allTargets.Length > 0)
            {
                foxtail.transform.position = allTargets[0].transform.position + foxtailOffset; 
            }
            foxtail.SetActive(false);
        }
        
        Debug.Log($"Total required connection targets found: {requiredTargetCount}.");
    }

    private void Update()
    {
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

        if (Input.GetMouseButton(0) && _isDrawing)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var raycastHit , maxDistance: 100f, (int)targetLayerRenderer))
            {
                var hitObject = raycastHit.collider.gameObject;

                if (hitObject.CompareTag(targetTag))
                {
                    if (connectedObjects.Count == 0 || hitObject != connectedObjects.Last())
                    {
                        connectedObjects.Add(hitObject);
                        Debug.Log($"Connected to sphere: {hitObject.name}. List size: {connectedObjects.Count}");
                    }
                }
            }
        }

        DrawLine(); 

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
    
    private bool CheckForCompleteConnection()
    {
        return connectedObjects.Distinct().Count() == requiredTargetCount;
    }

    private void HandleSuccessfulConnection()
    {
        _isDrawing = false; 

        drawPositions.Clear();
        foreach (var obj in connectedObjects)
        {
            drawPositions.Add(obj.transform.position);
        }
        lineRenderer.positionCount = drawPositions.Count;
        lineRenderer.SetPositions(drawPositions.ToArray());
        
        if (foxtail != null && drawPositions.Count > 1)
        {
            StartCoroutine(AnimateFoxtailAlongPath(drawPositions));
        }
        else
        {
             Debug.Log("PERFORMING ACTION: Line kept active, no foxtail to animate.");
        }
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
        
        if (foxtail != null)
        {
            StopAllCoroutines(); 
            foxtail.SetActive(false);
        }
    }

    public void DrawLine()
    {
        if (!Input.GetMouseButton(0) || !_isDrawing || connectedObjects.Count == 0)
        {
            return;
        }

        drawPositions.Clear();
        
        foreach (var obj in connectedObjects)
        {
            drawPositions.Add(obj.transform.position);
        }

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
    
    private IEnumerator AnimateFoxtailAlongPath(List<Vector3> path)
    {
        foxtail.SetActive(true);
        foxtail.transform.position = path[0] + foxtailOffset; 

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 startPoint = path[i];
            Vector3 endPoint = path[i + 1];
            
            float segmentDistance = Vector3.Distance(startPoint, endPoint);
            float duration = segmentDistance / animationSpeed; 
            float timeElapsed = 0f;
            
            while (timeElapsed < duration)
            {
                float t = timeElapsed / duration;
                
                Vector3 currentPosition = Vector3.Lerp(startPoint, endPoint, t);
                
                foxtail.transform.position = currentPosition + foxtailOffset;
                
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            foxtail.transform.position = endPoint + foxtailOffset;
        }

        Debug.Log("--- FOXTAL ANIMATION COMPLETE. PROCEED TO NEXT STEP ---");
    }
}