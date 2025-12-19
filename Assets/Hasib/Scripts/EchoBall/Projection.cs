using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projection : MonoBehaviour
{
    private Scene _scene;
    private PhysicsScene _physicsScene;
    [SerializeField] private Transform bouncableParent;


    private void Start()
    {
        CreatePhysicsScene();
    }

    void CreatePhysicsScene()
    {
       _scene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
       _physicsScene = _scene.GetPhysicsScene();
       foreach (Transform bouncable in bouncableParent)
       {
           var ghostObj = Instantiate(bouncable.gameObject,bouncable.position, bouncable.rotation);
           ghostObj.GetComponent<Renderer>().enabled = false;
           SceneManager.MoveGameObjectToScene(ghostObj, _scene);
       }
    }


    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int maxSimulationSteps = 100;
    public void SimulateTrajectory(GameObject ball, Vector3 spawnPosition)
    {
        GameObject bulletObj = Instantiate(ball, spawnPosition, Quaternion.identity);
        bulletObj.GetComponent<Renderer>().enabled = false;
        SceneManager.MoveGameObjectToScene(bulletObj, _scene);
        
        float dist = Mathf.Abs(Camera.main.transform.position.z - (-26f));

        // Correct world position of mouse
         Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist)
        );
        
        // Force Z plane
        mouseWorld.z = -26f;
        Vector3 dir = mouseWorld - spawnPosition;
        bulletObj.GetComponent<EchoBallMovement>().MoveBullet(dir);
        
        lineRenderer.positionCount = maxSimulationSteps;
        for(int i =0; i < maxSimulationSteps; i++)
        {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            lineRenderer.SetPosition(i, bulletObj.transform.position);
        }
        Destroy(bulletObj);
    }
}
