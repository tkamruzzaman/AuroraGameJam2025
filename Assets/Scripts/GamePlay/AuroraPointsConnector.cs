using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections; 
using Unity.Cinemachine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class AuroraPointsConnector : MonoBehaviour
{
    public static AuroraPointsConnector Instance;
    public LineRenderer lineRenderer;
    public LayerMask targetLayerRenderer;
    [SerializeField] private Shoot shoot;
    public static event Action OnAuroraConnectionComplete;

    [SerializeField] private GameObject player;
    [SerializeField] public bool IsAllPointActive;
    [Header("Target Configuration")] public Vector3 foxtailOffset = new Vector3(0f, 0.5f, 0f);
    public string targetTag = "PathSphere";

    [Header("Animation")] public GameObject foxtail;
    public float animationSpeed = 5f;

    private Camera mainCamera;
    private bool _isDrawing;
    private int requiredTargetCount;

    public List<GameObject> connectedObjects = new List<GameObject>();
    public List<Vector3> drawPositions = new List<Vector3>();
    public CinemachineCamera cinemachineCamera;
    public GameObject AuroraShader;
    public GameObject AuroraShader2;
    public GameObject AuroraShader3;
    private float currentIntensityAuroraShader = 0f;
    private Color auroraBaseColor;
    [SerializeField] private GameObject lastScene;
    private bool isDrawingDone;

    private void Awake()
    {
        Instance = this;
    }

    public void CheckIfAllActive()
    {
        int activatedCount = 0;
        foreach (GameObject go in connectedObjects)
        {
            if (go.GetComponent<MagnetForce>().isAlreadyActive)
            {
                activatedCount++;
            }
            else
            {
                // Don't notify DialogueManager here - OnPointActivated() already handles the count
                // Just sync the count to ensure it matches (only if higher)
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.SetActivatedCount(activatedCount);
                }

                return;
            }
        }

        IsAllPointActive = true;

        // Notify DialogueManager about level completion
        // Count is already tracked by OnPointActivated(), just sync to be sure
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.SetActivatedCount(activatedCount);
            DialogueManager.Instance.OnLevelComplete();
        }
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
        auroraBaseColor = AuroraShader.GetComponent<Renderer>().material.color;
        Renderer r = AuroraShader.GetComponent<Renderer>();
        Material m = r.material;

        // Start with zero intensity (black)
        Color start = m.color;
        Color zero = start * 0f;
        m.color = zero;
    }

    private void Update()
    {
        if (IsAllPointActive)
        {
            //     player.SetActive(false);
            shoot.enabled = false;
            Cursor.visible = true;
            if (isDrawingDone)
            {
                return;
            }
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
                    isDrawingDone = true;
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
            OnAuroraConnectionComplete?.Invoke(); //turn on the cinemachine camera
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
        var targetInputPos =
            mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
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
            //foxtail.SetActive(false);
        }

        Debug.Log("--- FOXTAL ANIMATION COMPLETE. PROCEED TO NEXT STEP ---");
        currentIntensityAuroraShader = 2.5f;
        FadeColorToTarget(AuroraShader, 1);
        FadeColorIntensity(AuroraShader2, 2);
        // FadeColorIntensity(AuroraShader3, 3);


        Invoke(nameof(LoadEnding), 5f);
        lastScene.SetActive(true);
    }

    void LoadEnding()
    {
        //SceneManager.LoadScene("10_Ending");
        Scenes nextScene;
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            nextScene = Scenes.Game_Level_02;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            nextScene = Scenes.Game_Level_03;
        }
        else
        {
            nextScene = Scenes.End;
        }

        GameServices.Instance.sceneNavigation.LoadScene(nextScene);
    }

    void EndThePointsConnection()
    {
        connectedObjects.Clear();
        lineRenderer.positionCount = 0;
        lineRenderer.gameObject.SetActive(false);
        foxtail.SetActive(false);
        foreach (GameObject obj in connectedObjects)
        {
            obj.gameObject.SetActive(false);
        }
    }

    public void FadeColorIntensity(GameObject plane, float duration)
    {
        if (!plane.activeSelf)
            plane.SetActive(true);

        Renderer r = plane.GetComponent<Renderer>();
        Material m = r.material;

        // Start with zero intensity (black)
        Color start = m.color;
        Color zero = start * 0f;
        m.color = zero;

        // Target intensity = 1.5x brighter
        Color target = start * 1.5f;

        // Tween from zero → target
        m.DOColor(target, duration)
            .SetEase(Ease.InOutQuad);
    }

    public void FadeColorToTarget(GameObject plane, float duration)
    {
        if (!plane.activeSelf)
            plane.SetActive(true);

        Renderer r = plane.GetComponent<Renderer>();
        Material m = r.material;

        // Increase intensity every call
        currentIntensityAuroraShader += 0.3f; // you can change this step size

        // We always base intensity on the original color (not current)
        Color target = auroraBaseColor * currentIntensityAuroraShader;

        // Tween from current color → target color
        m.DOColor(target, duration)
            .SetEase(Ease.InOutQuad);
    }


    public void AuroraFadeIner()
    {
        FadeColorToTarget(AuroraShader, 3);
    }
}