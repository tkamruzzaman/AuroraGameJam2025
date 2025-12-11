using UnityEngine;

public class BulletMagnetizable : MonoBehaviour
{
    public bool isBeingPulled = false;
    public Transform magnet;
    public float pullForce = 15f;
    public float maxSpeed = 20f;
    EchoBallMovement movement;
    [SerializeField] float bulletDuration = 20f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<EchoBallMovement>();
        Destroy(gameObject,20f);
    }
    

    void Update()
    {
        
    }

    // Call this to stop the bullet's old movement
    public void StopOldMovement()
    {
        Vector3 dir = (magnet.position - transform.position).normalized;
        //rb.AddForce(dir * pullForce, ForceMode.Acceleration);
        movement.Launch(dir* rb.linearVelocity.magnitude);
       //rb.linearVelocity = dir* rb.linearVelocity.magnitude ;
        
        
        // rb.linearVelocity = Vector3.zero;       // Remove all motion
        // rb.angularVelocity = Vector3.zero; // Stop any spin
    }
}