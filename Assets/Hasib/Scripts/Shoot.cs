using System;
using UnityEngine;
using UnityEngine.Serialization;
using Unity.Cinemachine;


public class Shoot : MonoBehaviour
{
    public static bool HasClearView = true;
    [SerializeField] private GameObject bulletPrefab;
  
    [SerializeField] private Transform player;
    [SerializeField] private float bulletForce = 10f;
    [SerializeField] private float planeZ = 0f;
    private Vector3 mousePosition;
    [FormerlySerializedAs("aimIndicator")] [SerializeField] GameObject aimIndicatorHolder;
    [SerializeField] SpriteRenderer aimIndicatorSprite;
    [SerializeField] GameObject aimIndicator;
    [SerializeField] private GameObject playerArt;
    private Vector3 mouseWorld;

    public static bool isShooting;
    [Header("Shoot Angles")]
    [SerializeField] Vector2 aimAngleRight;
    [SerializeField] Vector2 aimAngleLeft;
    // public CinemachineCamera cinemachineCamera;
    public static event Action OnBulletSpawned;
    public static Shoot Instance;
    public GameObject bullet;
    private void Awake()
    {
        Instance = this;
        
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Optional: lock the cursor to the game window
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        if (!isShooting)
        {
            Aim();
        }
        
        
        if (!Player.iswalkingAnimationTrue)
        {
            FlipSpriteBasedOnTarget(playerArt.transform,aimIndicator.transform);
        }
       
        if (bullet == null && AngleCheck(transform.eulerAngles.z) && Input.GetMouseButtonDown(0) &&  !Player.iswalkingAnimationTrue )
        {
            // Distance from camera to your plane
            
            player.GetComponent<Player>().CallAction();
            isShooting = true;

        }
        
    }

    public void ShootBullet()
    {
        // Spawn bullet at player's Z plane
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, planeZ);
        bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        
        OnBulletSpawned?.Invoke();
        print("Bullet Spawned Event Invoked");
        // Direction to mouse
        Vector3 dir = mouseWorld - spawnPos;
        // cinemachineCamera.gameObject.SetActive(true);
        // cinemachineCamera.Follow = bullet.transform;
            
        MoveBullet(dir, bullet);
        isShooting = false;
    }

    void FlipSpriteBasedOnTarget(Transform spriteTransform, Transform targetTransform)
    {
        // Compare world positions
        if (targetTransform.position.x > spriteTransform.position.x)
        {
            // Target is on the right
            Vector3 scale = spriteTransform.localScale;
            scale.x = Mathf.Abs(scale.x); // face right
            spriteTransform.localScale = scale;
        }
        else
        {
            // Target is on the left
            Vector3 scale = spriteTransform.localScale;
            scale.x = -Mathf.Abs(scale.x); // face left
            spriteTransform.localScale = scale;
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
        if ((angle >= aimAngleLeft.x && angle <= aimAngleLeft.y)||(angle >= aimAngleRight.x && angle <= aimAngleRight.y) )
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
        float dist = Mathf.Abs(Camera.main.transform.position.z - planeZ);

        // Correct world position of mouse
         mouseWorld = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist)
        );

        // Force Z plane
        mouseWorld.z = planeZ;

        
        
        
        Vector3 direction = mouseWorld - aimIndicatorHolder.transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Smooth rotation
        float rotationSpeed = 5f;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation =
            Quaternion.Lerp(aimIndicatorHolder.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
       // Debug.Log(transform.eulerAngles.z);
    }
    
    
   

   
}