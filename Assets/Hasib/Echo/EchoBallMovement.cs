using UnityEngine;

public class EchoBallMovement : MonoBehaviour
{

    public Rigidbody rb;

    [Header("Movement Settings")]
    public float maxSpeed = 15f;     // Peak speed
    public AnimationCurve speedCurve; // Controls mellow acceleration/deceleration
    public float curveDuration = 1.5f;

    private Vector3 launchDir;
    private float timer = 0f;

    public void Launch(Vector3 dir)
    {
        launchDir = dir.normalized;
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(timer / curveDuration);

        // Curve defines "how fast should the ball be right now"
        float curveSpeed = speedCurve.Evaluate(t);

        // Final speed = maxSpeed multiplied by curve value
        float currentSpeed = curveSpeed * maxSpeed;

        // Apply mellow velocity
        Vector3 vel = launchDir * currentSpeed;

        // Lock 2.5D plane
        vel.z = 0;

        rb.linearVelocity = vel;
    }
}


