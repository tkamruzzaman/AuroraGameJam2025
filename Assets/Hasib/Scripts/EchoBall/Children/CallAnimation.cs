using System;
using UnityEngine;


public class CallAnimation : MonoBehaviour
{
   [SerializeField] private Shoot _shoot;
   public void CallingAnimation()
   {
      _shoot.ShootBullet();
   }
}
