using System;
using Unity.VisualScripting;
using UnityEngine;

public class MagnetForce : MonoBehaviour
{
    [SerializeField] float pullForce = 15f;     // Strength of the attraction
    [SerializeField] float maxSpeed = 20f;      // Prevents bullets from flying too fast
    bool canAttract = true;
    private Collider otherGameObject;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("SS"+other.gameObject.name);
        if (other.gameObject.CompareTag("bullet"))
        {
            canAttract = true;
            otherGameObject = other;
        }
    }

    private void Update()
    {
        if (canAttract)
        {
            attractMagnet(otherGameObject);
        }
    }

    void attractMagnet(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return; // Only affect objects with Rigidbody

        // Direction towards the magnet
        Vector3 direction = (transform.position - rb.position).normalized;

        // Apply force
        rb.AddForce(direction * pullForce, ForceMode.Acceleration);

        // Optional: clamp maximum speed so it doesn't overshoot
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("bullet"))
        {
            Destroy(other.gameObject);
        }
    }
}