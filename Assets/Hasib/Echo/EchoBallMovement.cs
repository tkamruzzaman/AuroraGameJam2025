using UnityEngine;

public class EchoBallMovement : MonoBehaviour
{

    public Rigidbody rb;

    [Header("Movement Settings")]
    public float maxSpeed = 15f;     // Peak speed

    public AnimationCurve mellowCurve; // Controls mellow acceleration/deceleration
    public float curveDuration = 1.5f;

    private Vector3 launchDir;
    private float timer = 0f;

    public void Launch(Vector3 dir)
    {
        launchDir = dir.normalized;
        //timer = 0f;  // reset mellow curve
    }

    public void Bounce(Vector3 newDir, float newSpeedMultiplier)
    {
        launchDir = newDir.normalized;
        maxSpeed *= newSpeedMultiplier;  // optional, keeps mellow feel
        timer = 0f;                      // restart mellow curve from new direction
    }


    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(timer / curveDuration);

        // Curve defines "how fast should the ball be right now"
        float curveSpeed = mellowCurve.Evaluate(t);

        // Final speed = maxSpeed multiplied by curve value
        float currentSpeed = curveSpeed * maxSpeed;

        // Apply mellow velocity
        Vector3 vel = launchDir * currentSpeed;

        // Lock 2.5D plane
        vel.z = 0;

        rb.linearVelocity = vel;
        //Rotate();
    }

    void Rotate()
    {
        transform.Rotate(Vector3.up * 2f* Time.deltaTime);
    }
}


