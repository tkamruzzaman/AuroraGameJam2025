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
    public Shoot shootScript;
    
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

       MagnetForce[] allMagnetTargets = FindObjectsByType<MagnetForce>(FindObjectsSortMode.None);
        requiredTargetCount = allMagnetTargets.Length;
        
       if (foxtail != null)
        {
            if (requiredTargetCount > 0)
            {
                // We assume the first found target is acceptable for initial positioning
                foxtail.transform.position = allMagnetTargets[0].transform.position + foxtailOffset; 
            }
            foxtail.SetActive(false);
        }
        
        Debug.Log($"Total required connection targets found: {requiredTargetCount}.");
    }

    private void Update()
    {
        bool isDrawConditionMet = AllPointsConnectedAction(); 

        if (isDrawConditionMet)
        {
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true;
            shootScript.gameObject.SetActive(false); //deactivate script only 

            if (Input.GetMouseButtonDown(0))
            {
                lineRenderer.gameObject.SetActive(true); 

                print("0");
                if (!_isDrawing && lineRenderer.gameObject.activeSelf)
                {
                    print("1");
                    ResetConnection(shouldClearLine: true); 
                }
                
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                
                if (Physics.Raycast(ray, out var raycastHit, maxDistance: 300, (int)targetLayerRenderer))
                {
                    print("2");
                    var hitObject = raycastHit.transform.gameObject;

                    if (hitObject.CompareTag(targetTag))
                    {
                        print("3");
                        _isDrawing = true;
                        connectedObjects.Clear(); 
                        connectedObjects.Add(hitObject); 
                        // lineRenderer.gameObject.SetActive(true); 
                    }
                }
            }

            if (Input.GetMouseButton(0) && _isDrawing)
            {
                print("4");
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                
                if (Physics.Raycast(ray, out var raycastHit , maxDistance: 300, (int)targetLayerRenderer))
                {
                    print("5");
                    var hitObject = raycastHit.collider.gameObject;

                    if (hitObject.CompareTag(targetTag))
                    {
                        print("6");
                        if (connectedObjects.Count == 0 || hitObject != connectedObjects.Last())
                        {
                            print("7");
                            connectedObjects.Add(hitObject);
                            Debug.Log($"Connected to sphere: {hitObject.name}. List size: {connectedObjects.Count}");
                        }
                    }
                }
            }
            
            DrawLine(); 

            if (Input.GetMouseButtonUp(0) && _isDrawing)
            {
                print("8");
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
        else
        {
            if (_isDrawing || lineRenderer.gameObject.activeSelf)
            {
                print("9");
                ResetConnection(shouldClearLine: true);
            }
        }
    }
    
   public bool AllPointsConnectedAction()
    {
        MagnetForce[] allTargets = FindObjectsByType<MagnetForce>(FindObjectsSortMode.None);
        
        // Check if the current required count matches the number of objects we just found.
        // This is a safety check, normally they should match unless objects are created/destroyed dynamically.
        if (allTargets.Length != requiredTargetCount)
        {
             // If this happens, you should re-evaluate requiredTargetCount
             requiredTargetCount = allTargets.Length;
        }
        
        foreach (MagnetForce mf in allTargets)
        {
            // Note: We don't need a null check here because FindObjectsOfType only returns 
            // active components, but we check the property.
            
            if (!mf.isAlreadyActive)
            {
                // Debug.Log($"AllPointsConnectedAction: Condition failed because {mf.gameObject.name} has isAlreadyActive = FALSE. Drawing disabled.");
                return false;
            }
        }
        
        // If the array is empty (requiredTargetCount=0) this will return true, but input is already disabled.
        return true;
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
