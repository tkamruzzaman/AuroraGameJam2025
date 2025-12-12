using UnityEngine;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.Playables;

public class MainGameplayCameraControllers : MonoBehaviour
{
    public Transform player;
    public Transform Fox;
    public CinemachineCamera WalkingCamera;
    public CinemachineCamera BulletShootCamera;
    public CinemachineCamera FoxFollowCamera;
    public GameObject FoxTimeline;
    private Transform Echo;

    [SerializeField] private AuroraPointsConnector _auroraPointsConnector;
    //CurrentEcho == bullet
    //cinemachine..f
    void OnEnable()
    {
        Shoot.OnBulletSpawned += BulletShoot;
        Player.OnPlayerWalking += WalkingIdleCamera;
        //BulletMagnetizable.OnBulletDestroyed += WalkingIdleCamera;
        //MagnetForce.OnMagneticStartActivation += BulletShoot;
        AuroraPointsConnector.OnAuroraConnectionComplete += BulletShoot;
    }
    void OnDisable()
    {
        Shoot.OnBulletSpawned -= BulletShoot;
        Player.OnPlayerWalking -= WalkingIdleCamera;
        //BulletMagnetizable.OnBulletDestroyed -= BulletShoot;
        //MagnetForce.OnMagneticStartActivation -= BulletShoot;
        AuroraPointsConnector.OnAuroraConnectionComplete -= BulletShoot;


    }
    void FoxFollowCamerShot()
    {
        WalkingCamera.gameObject.SetActive(false);
        BulletShootCamera.gameObject.SetActive(false);
        //FoxFollowCamera.gameObject.SetActive(true);
       // FoxTimeline.SetActive(true);
        FoxFollowCamera.Follow = Fox;
        //FoxTimeline.GetComponent<PlayableDirector>().Play();
    }
    void WalkingIdleCamera()
    {
        if (!_auroraPointsConnector.IsAllPointActive)
        {
            WalkingCamera.gameObject.SetActive(true);
            BulletShootCamera.gameObject.SetActive(false);
            WalkingCamera.Follow = player;
            print("Walking Camera Activated");
        }
        

    }
    void BulletShoot()
    {
        print("camera goes back");
        BulletShootCamera.gameObject.SetActive(true);
        WalkingCamera.gameObject.SetActive(false);
        //BulletShootCamera.Follow = player;
    }
   
}
