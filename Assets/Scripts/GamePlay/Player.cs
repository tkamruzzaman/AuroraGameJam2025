using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static bool iswalkingAnimationTrue;
    const string IS_WALKING = "IsWalking";
    const string IDLE_STATE = "IdleBlendTree";
    const string IS_CALLING = "IsCalling";

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private Rigidbody2D playerRigidbody;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private GameObject playerArt;

    [SerializeField] private InputActionAsset InputActions;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction interactAction;

    bool isJumping;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        interactAction = InputSystem.actions.FindAction("Interact");

        playerRigidbody ??= GetComponent<Rigidbody2D>();
        playerCollider ??= GetComponent<Collider2D>();
        playerAnimator ??= GetComponent<Animator>();
    }
    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }
    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }
    private void Start()
    {

    }

    private void Update()
    {
        Movement();

        if (jumpAction.WasPressedThisFrame())
        {
            Jump();
        }

        if (interactAction.WasPressedThisFrame())
        {
            CallAction();
        }

        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(IDLE_STATE))
        {
            Debug.Log("idle state. change the idle animation here maybe?");
        }
    }

    private void Movement()
    {

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        float moveX = moveValue.x;
        float moveY = moveValue.y;

        Vector3 movement = new Vector3(moveX, 0, 0) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        if (moveX != 0)
        {
            playerAnimator.SetBool(IS_WALKING, true);
            //flip on move direction
            Vector3 scale = playerArt.transform.localScale;
            scale.x = Mathf.Sign(moveX) * Mathf.Abs(scale.x);
            playerArt.transform.localScale = scale;
            //transform.localScale = scale;
            iswalkingAnimationTrue = true;
        }
        else
        {
            playerAnimator.SetBool(IS_WALKING, false);
            iswalkingAnimationTrue = false;
        }
    }

    private void Jump()
    {
        if (jumpAction.triggered && GroundCheck() && !isJumping)
        {
            isJumping = true;
            print("Jump!");
            playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        }
    }

    private bool GroundCheck()
    {
        //TODO: fix this ground check
        // RaycastHit2D hit = Physics2D.Raycast(playerCollider.bounds.center, Vector2.down, playerCollider.bounds.extents.y + 0.1f);
        // if (hit.collider != null)
        // {
        //     return false;
        // }   
        return true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isJumping = false;
        }
    }

    public void CallAction()
    {
        playerAnimator.SetBool(IS_CALLING, true);
        //play calling sound
    }
}