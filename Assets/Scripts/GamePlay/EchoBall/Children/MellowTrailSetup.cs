using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class FloatyMellowTrailBullet : MonoBehaviour
{
    [Header("Trail Settings")]
    public float trailTime = 0.25f;        // very short
    public float startWidth = 0.015f;      // thin
    public float maxWidth = 0.05f;         // slightly thicker
    public float minVertexDistance = 0.02f;

    [Header("Emission Settings")]
    public Color trailColor = Color.white;
    public float emissionIntensity = 1.5f;

    private TrailRenderer trail;

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();

        // Duration
        trail.time = trailTime;

        // Width curve
        trail.widthCurve = new AnimationCurve(
            new Keyframe(0f, startWidth),
            new Keyframe(0.15f, maxWidth),
            new Keyframe(0.5f, maxWidth * 0.75f),
            new Keyframe(1f, 0f)
        );

        // Material with emission
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.SetColor("_BaseColor", trailColor);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", trailColor * emissionIntensity);
        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        trail.material = mat;

        // Other soft/floating settings
        trail.minVertexDistance = minVertexDistance;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.receiveShadows = false;
        trail.alignment = LineAlignment.View;  // soft, floaty
    }
}