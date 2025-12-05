using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        // Get horizontal input (A/D or Left/Right arrows)
        float horizontal = Input.GetAxisRaw("Horizontal");

        // Move the player along the X-axis
        transform.Translate(Vector3.right * horizontal * moveSpeed * Time.deltaTime);

        // Optional: flip sprite depending on direction
        if (horizontal != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(horizontal) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
}