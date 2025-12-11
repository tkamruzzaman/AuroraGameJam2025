using UnityEngine;
using System;
using System.Collections;
using Unity.Cinemachine;


public class MainGameplayCameraControllers : MonoBehaviour
{
    public Transform Player;
    public Transform CurrentEcho;
public CinemachineCamera cinemachineCamera;
    private Transform Echo;
    //CurrentEcho == bullet
    //cinemachine..f
    void OnEnable()
    {
        Shoot.OnBulletSpawned += FollowBullet;
        BulletMagnetizable.OnBulletDestroyed += FollowPlayer;
    }
    void OnDisable()
    {
        Shoot.OnBulletSpawned -= FollowBullet;
        BulletMagnetizable.OnBulletDestroyed -= FollowPlayer;
    }
    void FollowPlayer()
    {
        print("Following Player");
        cinemachineCamera.gameObject.SetActive(true);
        cinemachineCamera.Follow = Player;
    }
    void FollowBullet()
    {
        print("Following Bullet");
        cinemachineCamera.gameObject.SetActive(true);
        cinemachineCamera.Follow = Shoot.Instance.bullet.transform;
    }
}
