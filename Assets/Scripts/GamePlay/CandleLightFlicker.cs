using UnityEngine;

public class CandleLightFlicker : MonoBehaviour
{
    Light candleLight;

    [Header("Flicker Settings")]
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    public float flickerSpeed = 0.1f;

    float targetIntensity;

    void Start()
    {
        candleLight = GetComponent<Light>();
        targetIntensity = candleLight.intensity;
    }

    void Update()
    {
        if (Mathf.Abs(candleLight.intensity - targetIntensity) < 0.02f)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
        }

        candleLight.intensity = Mathf.Lerp(
            candleLight.intensity,
            targetIntensity,
            flickerSpeed * Time.deltaTime
        );
    }
}