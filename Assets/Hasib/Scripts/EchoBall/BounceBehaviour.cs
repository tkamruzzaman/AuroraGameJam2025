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

        var bounce = collision.gameObject.GetComponent<Bounciness>().BounceSpeedMultiplier;

        // Tell mellow system to use the bounced direction
        GetComponent<EchoBallMovement>().Bounce(direction, bounce);
    }

}