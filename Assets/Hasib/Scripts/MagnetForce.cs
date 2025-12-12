using System;
using DG.Tweening;
using UnityEngine;
using Random = System.Random;

public class MagnetForce : MonoBehaviour
{
    public bool isAlreadyActive; // setting this parameter to public - Uswah
    [SerializeField] Renderer materialRenderer;
    [SerializeField] private float newIntensity;
    [SerializeField] private float intensityMultipler;
    public static MagnetForce instance;
    public static event Action OnMagneticStartActivation;
    void OnEnable()
    {
        instance = this;
    }
    private void Start ()
    {
        materialRenderer = GetComponent<Renderer>();
        StartFloatyRandom(transform, 0.3f, 0.8f, 3f);

       
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
            AuroraPointsConnector.Instance.CheckIfAllActive();
            AuroraPointsConnector.Instance.AuroraFadeIner();
            DisableColliders();
            ChangeColor();
            //OnMagneticStartActivation?.Invoke();
            other.gameObject.GetComponent<MeshRenderer>().enabled = false; 
           // transform.GetChild(0).GetComponent<Animator>().SetBool("IsLit", true);
            Destroy(other.gameObject);
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
            col.isTrigger = true;
        }
    }
    
    public void StartFloatyRandom(Transform target, float minAmount, float maxAmount, float duration = 1f)
    {
        // Random starting direction: up or down
        float dir = UnityEngine.Random.value > 0.5f ? 1f : -1f;

        // Random float amount between min and max
        float amount = UnityEngine.Random.Range(minAmount, maxAmount) * dir;

        target.DOMoveY(target.position.y + amount, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}