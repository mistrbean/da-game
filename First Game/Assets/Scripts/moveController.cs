using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class moveController : MonoBehaviour
{
    Animator animator;
    [SerializeField]
    private InputActionReference movementControl;
    [SerializeField]
    private InputActionReference jumpControl;
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    private float rotationSpeed = 4f;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraMainTransform;
    int isRunningHash;
    int isIdleHash;

    PlayerControls input;

    Vector2 currentMovement;
    bool movementPressed;

    void Awake()
    {
        input = new PlayerControls();

        input.Player.Movement.performed += ctx =>
        {
            currentMovement = ctx.ReadValue<Vector2>();
            movementPressed = currentMovement.x != 0 || currentMovement.y != 0;
        };

    }
    void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();

        input.Player.Enable();
    }
    void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Enable();
    }

    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        cameraMainTransform = Camera.main.transform;
        animator = GetComponent<Animator>();

        isRunningHash = Animator.StringToHash("Armature|runAnim");
        isIdleHash = Animator.StringToHash("Armature|idleAnim");
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movement = movementControl.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        move = cameraMainTransform.forward * move.z + cameraMainTransform.right * move.x;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Changes the height position of the player..
        if (jumpControl.action.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }

        handleMovement();

    }

    void handleMovement()
    {
        bool isRunning = animator.GetBool(isRunningHash);
        bool isIdle = animator.GetBool(isIdleHash);

        if (movementPressed && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }
        if (movementPressed && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }
    }

    void FixedUpdate()
    {
        
    }
}
