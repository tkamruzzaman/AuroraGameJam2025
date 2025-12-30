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
    [SerializeField] Transform aimStartPoint;
    [SerializeField] private GameObject playerArt;
    private Vector3 mouseWorld;

    public  bool isShooting;
    [Header("Projection")]
    [SerializeField] private Projection projection;
    public LineRenderer line;
    public int maxBounces = 3;
    public float rayLength = 50f;
    
    public LayerMask obstacleMask;

    private int currentBounce;

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
        if (!isShooting&&  !Player.iswalkingAnimationTrue)
        {
            Aim();
            
        }
        
        
        if (!Player.iswalkingAnimationTrue)
        {
            FlipSpriteBasedOnTarget(playerArt.transform,aimIndicator.transform);
        }
        else
        {
            ClearPreview();
        }
       
        if (bullet == null && AngleCheck(transform.eulerAngles.z) && Input.GetMouseButtonDown(0) &&  !Player.iswalkingAnimationTrue )
        {
            // Distance from camera to your plane
            
            player.GetComponent<Player>().CallAction();
            isShooting = true;
            

        }

        if (bullet)
        {
            aimIndicator.SetActive(false);
        }
        else
        {
            aimIndicator.SetActive(true);
        }
        
    }

    public void ShootBullet()
    {
        ClearPreview();
        // Spawn bullet at player's Z plane
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, planeZ);
        bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        
        OnBulletSpawned?.Invoke();
        print("Bullet Spawned Event Invoked");
        // Direction to mouse
        Vector3 dir = mouseWorld - spawnPos;
        // cinemachineCamera.gameObject.SetActive(true);
        // cinemachineCamera.Follow = bullet.transform;
            
        
        bullet.GetComponent<EchoBallMovement>().MoveBullet(dir);
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
        currentBounce = 0;

        Vector3 dir =
            aimIndicator.transform.position -
            aimIndicatorHolder.transform.position;
        if (AngleCheck(transform.eulerAngles.z) && !bullet)
        {
            PreviewShoot(aimStartPoint.position, dir);
        }
        else
        {
            ClearPreview();
        }
       

       // Debug.Log(transform.eulerAngles.z);
     //  projection.SimulateTrajectory(bulletPrefab,new Vector3(transform.position.x, transform.position.y, planeZ),mouseWorld -aimIndicator.transform.position );
    }
    
    public void ClearPreview()
    {
        currentBounce = 0;
        line.positionCount = 0;
    }
   void PreviewShoot( Vector3 startPosition, Vector3 direction)
   {
       
       if (currentBounce >= maxBounces)
           return;
       // Reset only on first call
       if (currentBounce == 0)
       {
           line.positionCount = 0;
           AddPoint(startPosition);
       }

       

       startPosition.z = planeZ;
       direction.Normalize();

       if (Physics.Raycast(startPosition, direction,
               out RaycastHit hit, rayLength, obstacleMask))
       {
           Debug.Log("Found Hit: " + hit.collider.name);
           if (hit.collider.CompareTag("Obstacle"))
           {
               Vector3 hitPoint = hit.point;
               hitPoint.z = planeZ;

               AddPoint(hitPoint);

               // Reflect 
               Vector3 reflectedDir =
                   Vector3.Reflect(direction, hit.normal);

               // Optional: obstacle-specific logic
               Bounciness b = hit.collider.GetComponent<Bounciness>();
               if (b != null)
               {
                   reflectedDir =
                       Quaternion.Euler(0, 0, b.BounceAngle) * reflectedDir;
               }

               currentBounce++;

               // Small offset to avoid re-hitting same collider
               Vector3 newStart =
                   hitPoint + reflectedDir * 0.01f;

               PreviewShoot(newStart, reflectedDir);
               return;
           }
           else if ( hit.collider.CompareTag("Sky"))
           {
               Vector3 hitPoint = hit.point;
               hitPoint.z = planeZ;
               AddPoint(hitPoint);
           }
           else
           {
               Vector3 endPoint =
                   startPosition + direction * rayLength;
               endPoint.z = planeZ;
               Debug.Log("End Point: " + endPoint);
               AddPoint(endPoint);
           }
       }
       else
       {
           // No hit → draw ray forward and stop
           Vector3 endPoint =
               startPosition + direction * rayLength;
           endPoint.z = planeZ;
    Debug.Log("End Point: " + endPoint);
           AddPoint(endPoint);
       }
   }

   private void AddPoint(Vector3 point)
   {
       int index = line.positionCount;
       line.positionCount = index + 1;
       line.SetPosition(index, point);
   }
}