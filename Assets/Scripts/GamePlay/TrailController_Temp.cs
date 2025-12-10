using UnityEngine;

public class TrailController_Temp : MonoBehaviour
{
    [Header("Y Position Ranges")]
    public Vector2 topYRange = new Vector2(10f, 3f);
    public Vector2 midYRange = new Vector2(3f, -8f);
    public Vector2 bottomYRange = new Vector2(-8f, -20f);

    [Header("Color Gradients")]
    public Gradient topGradient = new Gradient();
    public Gradient midGradient = new Gradient();
    public Gradient bottomGradient = new Gradient();

    private ParticleSystem trailParticleSystem;

    void Start()
    {
        trailParticleSystem = GetComponent<ParticleSystem>();
        
        // Initialize default gradients if not set
        InitializeGradients();
    }

    void InitializeGradients()
    {
        if (topGradient.colorKeys.Length == 0)
        {
            var colorKeys = new GradientColorKey[2];
            colorKeys[0].color = Color.red;
            colorKeys[0].time = 0f;
            colorKeys[1].color = new Color(1f, 0.5f, 0f); // Orange
            colorKeys[1].time = 1f;
            topGradient.SetKeys(colorKeys, topGradient.alphaKeys);
        }

        if (midGradient.colorKeys.Length == 0)
        {
            var colorKeys = new GradientColorKey[2];
            colorKeys[0].color = Color.green;
            colorKeys[0].time = 0f;
            colorKeys[1].color = Color.cyan;
            colorKeys[1].time = 1f;
            midGradient.SetKeys(colorKeys, midGradient.alphaKeys);
        }

        if (bottomGradient.colorKeys.Length == 0)
        {
            var colorKeys = new GradientColorKey[2];
            colorKeys[0].color = new Color(0.5f, 0f, 1f); // Purple
            colorKeys[0].time = 0f;
            colorKeys[1].color = Color.blue;
            colorKeys[1].time = 1f;
            bottomGradient.SetKeys(colorKeys, bottomGradient.alphaKeys);
        }
    }

    void Update()
    {
        // Update particle color based on Y position
        UpdateParticleColor();
    }

    void UpdateParticleColor()
    {
        if (trailParticleSystem == null) return;

        float yPos = transform.position.y;
        float t = 0f;
        Color newColor;

        if (yPos >= midYRange.x)
        {
            // Top range
            // InverseLerp maps yPos from the range (topYRange.x to topYRange.y) to a 0-1 value (t)
            t = Mathf.InverseLerp(topYRange.x, topYRange.y, yPos);
            newColor = topGradient.Evaluate(t);
        }
        else if (yPos >= bottomYRange.x)
        {
            // Mid range
            t = Mathf.InverseLerp(midYRange.x, midYRange.y, yPos);
            newColor = midGradient.Evaluate(t);
        }
        else
        {
            // Bottom range
            t = Mathf.InverseLerp(bottomYRange.x, bottomYRange.y, yPos);
            newColor = bottomGradient.Evaluate(t);
        }

        var main = trailParticleSystem.main;
        main.startColor = newColor;
    }
}