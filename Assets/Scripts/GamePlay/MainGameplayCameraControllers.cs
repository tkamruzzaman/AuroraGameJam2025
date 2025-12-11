using UnityEngine;
using System;
using System.Collections;
using Unity.Cinemachine;


public class MainGameplayCameraControllers : MonoBehaviour
{
    public Transform player;
    public Transform CurrentEcho;
public CinemachineCamera WalkingCamera;
public CinemachineCamera BulletShootCamera;
    private Transform Echo;
    //CurrentEcho == bullet
    //cinemachine..f
    void OnEnable()
    {
        Shoot.OnBulletSpawned += BulletShoot;
        Player.OnPlayerWalking += WalkingIdleCamera;
        //BulletMagnetizable.OnBulletDestroyed += WalkingIdleCamera;
        //MagnetForce.OnMagneticStartActivation += BulletShoot;
    }
    void OnDisable()
    {
        Shoot.OnBulletSpawned -= BulletShoot;
        Player.OnPlayerWalking -= WalkingIdleCamera;
        //BulletMagnetizable.OnBulletDestroyed -= BulletShoot;
        //MagnetForce.OnMagneticStartActivation -= BulletShoot;

    }
    void WalkingIdleCamera()
    {
        WalkingCamera.gameObject.SetActive(true);
        BulletShootCamera.gameObject.SetActive(false);
        WalkingCamera.Follow = player;
        print("Walking Camera Activated");

    }
    void BulletShoot()
    {
        print("camera goes back");
        BulletShootCamera.gameObject.SetActive(true);
        WalkingCamera.gameObject.SetActive(false);
        //BulletShootCamera.Follow = player;
    }
   
}
