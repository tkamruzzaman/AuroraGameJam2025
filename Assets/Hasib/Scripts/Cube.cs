using System;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private int _bounceCount;
    [SerializeField] private int bounceLimit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            _bounceCount++;
            if (_bounceCount > bounceLimit)
            {
                Destroy(gameObject);
            }
        }
    }
}
