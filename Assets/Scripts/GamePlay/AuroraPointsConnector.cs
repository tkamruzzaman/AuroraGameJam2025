using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AuroraPointsConnector : MonoBehaviour
{
    // --- Public Fields ---
    public LineRenderer lineRenderer;
    public List<Transform> pointsToConnect = new List<Transform>();
    public GameObject foxTailObject; 
    public bool activateFoxTail = false; 
    public float animationDuration = 1.0f;
    public float sphereCompletionRadius = 8f; // Value set by user
    public string sphereTag = "PathSphere"; 
    public GameObject AuroraShader;

    // NEW: Highlighting parameters
    public float highlightScale = 1.5f;
    public Color highlightColor = Color.yellow;
    
    private LayerMask sphereLayerMask; 
    private bool isAnimating = false;
    private bool lineInitialized = false;
    private bool lineAnimationFinished = false;
    private int nextSphereIndexToPass = 0; 
    
    // NEW: Reference to the Coroutine to ensure it only runs once
    private Coroutine passageMonitorCoroutine; 

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }
        }
        
        InitializeLine();
        
        for (int i = 0; i < pointsToConnect.Count; i++)
        {
            ResetSphereState(pointsToConnect[i]);
        }
        
        lineRenderer.enabled = false;
        if (foxTailObject != null)
        {
            foxTailObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isAnimating && !lineAnimationFinished && CheckAllPointsActivated())
        {
            StartLineAnimation();
        }
        
        CheckForNextAction();
    }

    private void CheckForNextAction()
    {
        // FIX: Start the Coroutine ONLY if the animation is finished, Fox Tail is active, 
        // AND the Coroutine is not already running.
        if (lineAnimationFinished && activateFoxTail && passageMonitorCoroutine == null)
        {
            if (foxTailObject != null && !foxTailObject.activeSelf)
            {
                foxTailObject.SetActive(true);
            }
            
            // Start the single, long-running Coroutine to monitor ALL spheres sequentially
            passageMonitorCoroutine = StartCoroutine(MonitorFoxTailPassage());
        }
    }

    // THIS COROUTINE REPLACES THE INCORRECTLY USED CheckFoxTailPassageByDistance()
    // It runs the entire sequence loop using yield returns to wait for user input/movement.
    private IEnumerator MonitorFoxTailPassage()
    {
        // Loop runs until all spheres are checked
        while (nextSphereIndexToPass < pointsToConnect.Count)
        {
            Transform targetSphere = pointsToConnect[nextSphereIndexToPass];
            
            if (targetSphere == null)
            {
                Debug.LogError($"Target sphere at index {nextSphereIndexToPass} is null. Skipping.");
                nextSphereIndexToPass++;
                continue; // Skip the rest of the loop and check the next point
            }

            // INNER LOOP: This is where the Coroutine waits, frame by frame, 
            // until the distance condition is met for the CURRENT target sphere.
            while (Vector3.Distance(foxTailObject.transform.position, targetSphere.position) > sphereCompletionRadius)
            {
                yield return null; // Wait for the next frame
            }
            
            // === SUCCESSFUL PASSAGE ===
            
            Debug.Log($"Sequence Step: Fox Tail reached sphere index {nextSphereIndexToPass}!");
            
            ApplyHighlight(targetSphere);

            // Move to the next target index. The main 'while' loop restarts for the next sphere.
            nextSphereIndexToPass++;
        }

        // --- Sequence Complete ---
        
        Debug.Log("FULL SEQUENCE COMPLETE! All spheres highlighted.");
        
        // This handles the final actions and cleans up the coroutine reference
        yield return new WaitForSeconds(1.0f);
        if (AuroraShader != null)
        {
            AuroraShader.SetActive(true);
        }
        DeactivateAllPointsAndLine();
        
        passageMonitorCoroutine = null; // Clear the reference
    }
    
    // --- Rest of the Script ---

    public void DeactivateAllPointsAndLine()
    {
        // Stop the monitoring coroutine if it's running
        if (passageMonitorCoroutine != null)
        {
            StopCoroutine(passageMonitorCoroutine);
            passageMonitorCoroutine = null;
        }

        foreach (Transform point in pointsToConnect)
        {
            if (point != null && point.gameObject.activeSelf)
            {
                point.gameObject.SetActive(false);
            }
        }
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }

        if (foxTailObject != null && foxTailObject.activeSelf)
        {
            foxTailObject.SetActive(false);
        }
    }

    private void ApplyHighlight(Transform sphere)
    {
        sphere.localScale = new Vector3(highlightScale, highlightScale, highlightScale);
        
        Renderer renderer = sphere.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (renderer.material.HasProperty("_Color"))
            {
                renderer.material.color = highlightColor;
            }
            else
            {
                if (renderer.material.HasProperty("_BaseColor"))
                {
                    renderer.material.SetColor("_BaseColor", highlightColor);
                }
                else
                {
                    Debug.LogError($"Sphere {sphere.name} material does not have a recognized color property (_Color or _BaseColor). Highlighting failed.");
                }
            }
        }
    }
    
    private void ResetSphereState(Transform sphere)
    {
        if (sphere == null) return;
        
        sphere.localScale = Vector3.one;
        
        Renderer renderer = sphere.GetComponent<Renderer>();
        if (renderer != null && renderer.material.HasProperty("_Color"))
        {
            renderer.material.color = Color.white;
        }
        else if (renderer != null && renderer.material.HasProperty("_BaseColor"))
        {
            renderer.material.SetColor("_BaseColor", Color.white);
        }
    }
    
    public bool CheckAllPointsActivated()
    {
        foreach (Transform pointTransform in pointsToConnect)
        {
            if (pointTransform == null)
            {
                return false; 
            }
            
            MagnetForce magnetForce = pointTransform.GetComponent<MagnetForce>();
            
            if (magnetForce != null)
            {
                if (!magnetForce.isAlreadyActive)
                {
                    return false;
                }
            }
            else
            {
                Debug.LogError($"Point {pointTransform.name} is missing the MagnetForce script!");
                return false;
            }
        }
        return true;
    }

    private void InitializeLine()
    {
        if (pointsToConnect == null || pointsToConnect.Count < 2)
        {
            lineRenderer.positionCount = 0;
            lineInitialized = false;
            return;
        }

        lineRenderer.positionCount = pointsToConnect.Count;
        
        for (int i = 0; i < pointsToConnect.Count; i++)
        {
            if (pointsToConnect[i] != null)
            {
                lineRenderer.SetPosition(i, pointsToConnect[i].position);
            }
            else
            {
                Debug.LogError($"Point at index {i} is null. Line will be incomplete.");
                lineRenderer.positionCount = i; 
                lineInitialized = false;
                return;
            }
        }

        lineInitialized = true;
    }
    public void StartLineAnimation()
    {
        if (!lineInitialized)
        {
            Debug.LogError("Line not initialized. Check 'pointsToConnect' list.");
            return;
        }

        lineRenderer.enabled = true; 
        isAnimating = true;
        StartCoroutine(AnimateLineDrawing());
    }
    private IEnumerator AnimateLineDrawing()
    {
        float timer = 0f;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, pointsToConnect[0].position);
        
        float totalDistance = 0f;
        for(int i = 0; i < pointsToConnect.Count - 1; i++)
        {
            if (pointsToConnect[i] != null && pointsToConnect[i+1] != null)
            {
                totalDistance += Vector3.Distance(pointsToConnect[i].position, pointsToConnect[i + 1].position);
            }
        }

        if (totalDistance == 0f) 
        {
            FinalizeLineState(); 
            yield break;
        }

        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / animationDuration;
            float currentDistance = totalDistance * progress;
            float distanceCovered = 0f;
            for (int currentSegment = 0; currentSegment < pointsToConnect.Count - 1; currentSegment++)
            {
                float segmentLength = Vector3.Distance(pointsToConnect[currentSegment].position, pointsToConnect[currentSegment + 1].position);
                if (distanceCovered + segmentLength > currentDistance)
                {
                    float segmentProgress = (currentDistance - distanceCovered) / segmentLength;
                    lineRenderer.positionCount = currentSegment + 2;
                    Vector3 endPosition = Vector3.Lerp(
                        pointsToConnect[currentSegment].position, 
                        pointsToConnect[currentSegment + 1].position, 
                        segmentProgress
                    );
                    lineRenderer.SetPosition(currentSegment + 1, endPosition);
                    for(int i = 0; i <= currentSegment; i++)
                    {
                        lineRenderer.SetPosition(i, pointsToConnect[i].position);
                    }
                    break;
                }
                distanceCovered += segmentLength;
            }
            yield return null;
        }
        FinalizeLineState();
    }
    
    private void FinalizeLineState()
    {
        lineRenderer.positionCount = pointsToConnect.Count;
        for (int i = 0; i < pointsToConnect.Count; i++)
        {
            lineRenderer.SetPosition(i, pointsToConnect[i].position);
        }
        isAnimating = false; 
        lineAnimationFinished = true;
        nextSphereIndexToPass = 0;
        activateFoxTail = true;
    }
}