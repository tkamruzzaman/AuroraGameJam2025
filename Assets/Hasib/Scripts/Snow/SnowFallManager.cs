using System;
using System.Collections.Generic;
using UnityEngine;

public class SnowFallManager : MonoBehaviour
{
    public static SnowFallManager Instance;
    
    [SerializeField] List<ParticleSystem> fallingSnowParticles;
    [SerializeField] GameObject snowParticlesParent;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayFallingSnowParticles(Vector3 position)
    {
        snowParticlesParent.transform.position = position;
        foreach (ParticleSystem fallingSnowParticle in fallingSnowParticles)
        {
            fallingSnowParticle.Play();
        }
    }
}
