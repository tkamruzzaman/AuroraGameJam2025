using UnityEngine;

public class BulletMagnetizable : MonoBehaviour
{
    public bool isBeingPulled = false;
    public Transform magnet;
    public float pullForce = 15f;
    public float maxSpeed = 20f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isBeingPulled) return;

        // Apply pulling
        Vector3 dir = (magnet.position - transform.position).normalized;
        rb.AddForce(dir * pullForce, ForceMode.Acceleration);

        // Cap speed
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    // Call this to stop the bullet's old movement
    public void StopOldMovement()
    {
        rb.linearVelocity = Vector3.zero;       // Remove all motion
        rb.angularVelocity = Vector3.zero; // Stop any spin
    }
}