using UnityEngine;
using DG.Tweening;
public class Bounciness : MonoBehaviour
{
   [SerializeField] float bounceSpeedMultiplier=1f;
   public float BounceSpeedMultiplier => bounceSpeedMultiplier;
   public float BounceAngle;
   public float maxSwayAngle = 10f; // max rotation in degrees
    public float swingDuration = 1.5f; // duration for each sway
    public int oscillations = 2; // number of left-right swings

    public void BounceTree(bool hitFromRight)
    {
        float direction = hitFromRight ? -1f : 1f;

        // Stop any previous tween
        transform.DOKill();

        // Create a sequence for mellow oscillation
        Sequence seq = DOTween.Sequence();

        // Start with the initial swing
        seq.Append(transform.DORotate(new Vector3(0, 0, maxSwayAngle * direction), swingDuration)
                          .SetEase(Ease.OutSine));

        // Add oscillations decreasing in amplitude
        for (int i = 0; i < oscillations; i++)
        {
            float sway = maxSwayAngle * direction * Mathf.Pow(0.5f, i + 1); // smaller each time
            seq.Append(transform.DORotate(new Vector3(0, 0, -sway), swingDuration)
                              .SetEase(Ease.InOutSine));
            seq.Append(transform.DORotate(new Vector3(0, 0, sway), swingDuration)
                              .SetEase(Ease.InOutSine));
        }

        // End at original rotation
        seq.Append(transform.DORotate(Vector3.zero, swingDuration).SetEase(Ease.OutSine));
    }

}
