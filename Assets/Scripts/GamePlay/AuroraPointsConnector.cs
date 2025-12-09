using System.Collections.Generic;
using System; 
using UnityEngine;

public class AuroraPointsConnector : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public LayerMask targetLayerRenderer;
    private Camera mainCamera;
    private bool _isDrawing;
    public List<GameObject> connectedObjects = new List<GameObject>();
    public List<Vector3> drawPositions = new List<Vector3>();
    private void Start()
    {
        mainCamera = Camera.main;
        //lineRenderer.positionCount = 0;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var raycastHit,maxDistance: 100f, (int)targetLayerRenderer))
            {
                _isDrawing = true;
                connectedObjects.Add(raycastHit.transform.gameObject);
                lineRenderer.gameObject.SetActive(true);
            }
        }

        if (Input.GetMouseButton(0) && _isDrawing)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var raycastHit , maxDistance: 100f, (int)targetLayerRenderer))
            {
                var targetObject = raycastHit.transform.gameObject;
                if (!connectedObjects.Contains(raycastHit.collider.gameObject))
                {
                    connectedObjects.Add(raycastHit.collider.gameObject);
                }
            }
        }
        DrawLine();

        if (Input.GetMouseButtonUp(0) && _isDrawing)
        {
            _isDrawing = false;
            connectedObjects.Clear();
            // Here you can add logic to check if the drawn path is valid
        }
    }
    public void DrawLine()
    {
        drawPositions.Clear();
        if (connectedObjects.Count > 0)
        {
            foreach (var obj in connectedObjects)
            {
                drawPositions.Add(obj.transform.position);
            }
            Vector3 inputDrawPosition =  GetMouseWorldPosition();
            drawPositions.Add(inputDrawPosition);
            lineRenderer.positionCount = drawPositions.Count;
            lineRenderer.SetPositions(drawPositions.ToArray());       
        }
        
    }
    public Vector3 GetMouseWorldPosition()
    {
        var targetInputPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        return targetInputPos;}
}
