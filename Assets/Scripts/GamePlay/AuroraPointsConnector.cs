using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections; 
using Unity.Cinemachine;
using DG.Tweening;

public class AuroraPointsConnector : MonoBehaviour
{
    public static AuroraPointsConnector Instance;
    public LineRenderer lineRenderer;
    public LayerMask targetLayerRenderer;
    [SerializeField] private Shoot shoot;
    public static event Action OnAuroraConnectionComplete;

    [SerializeField] private GameObject player;
    [SerializeField]
    bool IsAllPointActive;
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
    public CinemachineCamera cinemachineCamera;
    public GameObject AuroraShader;

    private void Awake()
    {
        Instance = this;
    }

    public void CheckIfAllActive()
    {
        foreach (GameObject go in connectedObjects)
        {
            if (!go.GetComponent<MagnetForce>().isAlreadyActive)
            {
                return;
            }
        }

        IsAllPointActive = true;
    }
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
        cinemachineCamera.Follow = player.transform;
    }

    private void Update()
    {
   

        if (IsAllPointActive)
        {
        //     player.SetActive(false);
         shoot.enabled = false;
            Cursor.visible = true;
        if (Input.GetMouseButtonDown(0))
        {
            if (!_isDrawing && lineRenderer.gameObject.activeSelf)
            {
                ResetConnection(shouldClearLine: true);
            }

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var raycastHit, Mathf.Infinity, (int)targetLayerRenderer))
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
            if (Physics.Raycast(ray, out var raycastHit, Mathf.Infinity, (int)targetLayerRenderer))
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
                Debug.Log(
                    $"Connection failed. Visited {connectedObjects.Distinct().Count()} out of {requiredTargetCount} unique targets.");
                ResetConnection(shouldClearLine: true);
            }
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
OnAuroraConnectionComplete?.Invoke();
        drawPositions.Clear();
        foreach (var obj in connectedObjects)
        {
            drawPositions.Add(obj.transform.position);
        }
        lineRenderer.positionCount = drawPositions.Count;
        lineRenderer.SetPositions(drawPositions.ToArray());
        
        if (foxtail != null && drawPositions.Count > 1)
        {
            OnAuroraConnectionComplete?.Invoke();//turn on the cinemachine camera
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
        
        // --- Calculate the movement direction vector ---
        Vector3 movementDirection = (endPoint - startPoint).normalized;
        
        // --- Calculate the target rotation ---
        // This creates a rotation that looks along the movementDirection.
        // If your model's forward axis is NOT the Z-axis in local space, 
        // you might need to adjust the rotation, but try this standard approach first.
        if (movementDirection != Vector3.zero)
        {
            foxtail.transform.rotation = Quaternion.LookRotation(movementDirection);
        }
        // If the foxtail's forward axis is the LOCAL X-axis, and not the local Z-axis (default for LookRotation),
        // you might need to use:
        // foxtail.transform.rotation = Quaternion.LookRotation(movementDirection) * Quaternion.Euler(0, -90, 0); 
        // (or some other adjustment like +90 or 180 on a different axis)
        // Try the simple one first and see which way the foxtail points!
        
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
    FadeIn(AuroraShader, 7);
}
    void EndThePointsConnection()
    {
        connectedObjects.Clear();
        lineRenderer.positionCount = 0;
        lineRenderer.gameObject.SetActive(false);
        foxtail.SetActive(false);
    }
public void FadeIn(GameObject plane, float duration)
{
    EndThePointsConnection();
    AuroraShader.SetActive(true);
    Material material = plane.GetComponent<Renderer>().material;
    
    // Set material to transparent mode
    material.SetFloat("_Mode", 3);
    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
    material.SetInt("_ZWrite", 0);
    material.DisableKeyword("_ALPHATEST_ON");
    material.EnableKeyword("_ALPHABLEND_ON");
    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
    material.renderQueue = 3000;
    
    // Start invisible
    Color color = material.color;
    color.a = 0f;
    material.color = color;
    
    // Fade in
    material.DOFade(1f, duration);
}
}