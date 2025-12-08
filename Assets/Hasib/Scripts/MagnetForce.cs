using System;
using UnityEngine;

public class MagnetForce : MonoBehaviour
{
    private bool isAlreadyActive;
    [SerializeField] Renderer materialRenderer;
    [SerializeField] private float newIntensity;
    [SerializeField] private float intensityMultipler;
    private void Start ()
    {
        materialRenderer = GetComponent<Renderer>();
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bullet") && !isAlreadyActive)
        {
            BulletMagnetizable bm = other.GetComponent<BulletMagnetizable>();
            if (bm != null)
            {
                     // ⛔ STOP original AddForce movement
                bm.isBeingPulled = true;   // ✅ Start magnet pulling
                bm.magnet = transform;
                bm.StopOldMovement(); 
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("bullet")&& !isAlreadyActive)
        {
            
            isAlreadyActive = true;
            DisableColliders();
            ChangeColor();
            other.gameObject.GetComponent<MeshRenderer>().enabled = false;  
            Destroy(other.gameObject,5f);
        }
        
    }

    void ChangeColor()
    {
        materialRenderer.material.EnableKeyword("_EMISSION");
        Color currentEmission = materialRenderer.material.GetColor("_EmissionColor");

        // Convert to color + intensity
        float currentIntensity = Mathf.Max(currentEmission.r, currentEmission.g, currentEmission.b);
        Color baseColor = currentEmission / currentIntensity;

        // Apply new intensity while keeping color
        Color newEmission = baseColor * newIntensity*intensityMultipler;
        materialRenderer.material.SetColor("_EmissionColor", newEmission);
    }

    void DisableColliders()
    {
        SphereCollider[] colliders = GetComponents<SphereCollider>();


        foreach (SphereCollider col in colliders)
        {
            col.enabled = false;
        }
    }
}