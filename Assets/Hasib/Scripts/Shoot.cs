using System;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private float bulletForce = 10f;
    [SerializeField] private float planeZ = 0f;
    private Vector3 mousePosition;
    private void Awake()
    {
        player = this.gameObject.transform;
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;

    }

    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            // Distance from camera to your plane
            float dist = Mathf.Abs(Camera.main.transform.position.z - planeZ);

            // Correct world position of mouse
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist)
            );

            // Force Z plane
            mouseWorld.z = planeZ;

            // Spawn bullet at player's Z plane
            Vector3 spawnPos = new Vector3(player.position.x, player.position.y, planeZ);
            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

            // Direction to mouse
            Vector3 dir = mouseWorld - spawnPos;
            dir.z = 0;

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(dir.normalized * bulletForce, ForceMode.Impulse);

            // Lock 2.5D plane
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
    }

    void Aim()
    {
        // Bullet is shot on planeZ, so aim on the same plane
        float dist = Mathf.Abs(Camera.main.transform.position.z - planeZ);

        Vector3 mouse = Input.mousePosition;
        mouse.z = dist;

        // Convert to world
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouse);
        mouseWorld.z = planeZ;

        // Use the player position AT planeZ
        Vector3 aimPos = new Vector3(transform.position.x, transform.position.y, planeZ);

        Vector3 dir = mouseWorld - aimPos;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void shoot()
    {
        Vector3 spawnPos = new Vector3(player.position.x, player.position.y, planeZ);
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        // Direction to mouse
        Vector3 dir = this.transform.position - spawnPos;
        dir.z = 0;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(dir.normalized * bulletForce, ForceMode.Impulse);

        // Lock 2.5D plane
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }



}
