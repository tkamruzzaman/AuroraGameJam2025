using System;
using UnityEngine;

public class BounceBehaviour : MonoBehaviour
{
    public bool usePerfectBounce = true;
   

    Rigidbody rb;
    Vector3 lastVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        lastVelocity = rb.linearVelocity;
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (!usePerfectBounce) return;
    //     if (collision.gameObject.CompareTag("Obstacle"))
    //     {
    //         var speed = lastVelocity.magnitude;
    //         var direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
    //         rb.linearVelocity = direction * speed * collision.gameObject.GetComponent<Bounciness>().BounceSpeedMultiplier;
    //         
    //     }
    // }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!usePerfectBounce) return;
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
            return;
        }
        if (!collision.gameObject.CompareTag("Obstacle")) return;

        var speed = lastVelocity.magnitude;
        var normal = collision.contacts[0].normal;
        var direction = Vector3.Reflect(lastVelocity.normalized, normal);
        Vector3 hitPosition = collision.contacts[0].point;
        //direction = direction + new Vector3(0f,collision.gameObject.GetComponent<Bounciness>().BounceAngle,0f);
    // Compare hit position with tree's position
        bool hitFromRight = hitPosition.x > transform.position.x;
        var bounce = collision.gameObject.GetComponent<Bounciness>().BounceSpeedMultiplier;
        collision.gameObject.GetComponent<Bounciness>().BounceTree(hitFromRight);
        // Tell mellow system to use the bounced direction
        GetComponent<EchoBallMovement>().Bounce(direction, bounce);
    }

}