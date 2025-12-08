using UnityEngine;

public class MagnetForce : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bullet"))
        {
            BulletMagnetizable bm = other.GetComponent<BulletMagnetizable>();
            if (bm != null)
            {
                bm.StopOldMovement();      // ⛔ STOP original AddForce movement
                bm.isBeingPulled = true;   // ✅ Start magnet pulling
                bm.magnet = transform;
            }
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