using System;
using UnityEngine;

public class ShootChecking : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Shoot.HasClearView = false; 
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Shoot.HasClearView = true; 
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Shoot.HasClearView = false; 
        }
    }
}
