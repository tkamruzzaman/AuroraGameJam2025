using System;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private float bulletForce = 10f;
    [SerializeField] private float planeZ = 0f;
    private Vector3 mousePosition;
    [SerializeField] GameObject aimIndicator;
    [SerializeField] SpriteRenderer aimIndicatorSprite;

    private void Awake()
    {
        player = this.gameObject.transform;
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Optional: lock the cursor to the game window
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        float dist = Mathf.Abs(Camera.main.transform.position.z - planeZ);

        // Correct world position of mouse
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist)
        );

        // Force Z plane
        mouseWorld.z = planeZ;


        Vector3 direction = mouseWorld - aimIndicator.transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Smooth rotation
        float rotationSpeed = 5f;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation =
            Quaternion.Lerp(aimIndicator.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        Debug.Log(transform.eulerAngles.z);

        if (AngleCheck(transform.eulerAngles.z) && Input.GetMouseButtonDown(0)  )
        {
            // Distance from camera to your plane


            // Spawn bullet at player's Z plane
            Vector3 spawnPos = new Vector3(player.position.x, player.position.y, planeZ);
            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

            // Direction to mouse
            Vector3 dir = mouseWorld - spawnPos;
            MoveBullet(dir, bullet);
        }
    }

    
    
    // void MoveBullet(Vector3 dir, GameObject bullet)
    // {
    //     dir.z = 0;
    //
    //     Rigidbody rb = bullet.GetComponent<Rigidbody>();
    //     rb.AddForce(dir.normalized * bulletForce, ForceMode.Impulse);
    //
    //     // Lock 2.5D plane
    //     rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    // }

    void MoveBullet(Vector3 dir, GameObject bullet)
    {
        dir.z = 0;

        EchoBallMovement mb = bullet.GetComponent<EchoBallMovement>();
        mb.Launch(dir);

        // Lock 2.5D
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    bool AngleCheck(float angle)
    {
        if ((angle >= 100 && angle <= 170)||(angle >= 20 && angle <= 70))
        {
            aimIndicatorSprite.color = Color.green;
            return true ;
        }
        else
        {
            aimIndicatorSprite.color = Color.red;
             return false;
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