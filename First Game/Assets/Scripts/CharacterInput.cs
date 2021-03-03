using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    Animator animator;
    float velocity = 0.0f;
    public float acceleration = 0.1f;
    public float deceleration = 0.5f;
    int VelocityHash;
    public float playerSpeed = 6f;

    //jump
    Vector3 jumpVelocity;
    public float gravity = -9.81f;
    public bool groundedPlayer;
    public float jumpHeight = 3.0f;

    //smooth character turning
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    //character face toward camera
    public Transform cam;

    public CharacterController controller;

    void Start()
    {
        //lock cursor
        Cursor.lockState = CursorLockMode.Locked;

        //set reference for animator
        animator = GetComponent<Animator>();

        //get velocity parameter id
        VelocityHash = Animator.StringToHash("Velocity");
    }

    void Update()
    {

        groundedPlayer = controller.isGrounded;

        if(groundedPlayer && jumpVelocity.y < 0)
        {
            jumpVelocity.y = -2.0f;
        }

        //get key/axis input from player
        bool forwardPressed = Input.GetKey("w") || Input.GetKey("s") || Input.GetKey("a") || Input.GetKey("d");
        bool sprintPressed = Input.GetKey("left shift");
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0.0f, vertical).normalized;

        //update jumpVelocity using physics equation
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            jumpVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            //animator jump code below?

            //
        }
        jumpVelocity.y += gravity * Time.deltaTime;

        //declare movement Vector3 (x, y, z)
        Vector3 moveDir = Vector3.zero;

        //move character
        if (direction.magnitude >= 0.1f)
        {
            //rotate character with camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            //smooth turning
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //physical movement
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        }

        controller.Move(((moveDir.normalized * playerSpeed) + jumpVelocity) * Time.deltaTime);

        //animator blend tree (setting velocity)
        if (forwardPressed && velocity < 1.0f)
        {
            velocity += Time.deltaTime * acceleration;
        }

        if (!forwardPressed && velocity > 0.0f)
        {
            velocity -= Time.deltaTime * deceleration;
        }

        if (!forwardPressed && velocity < 0.0f)
        {
            velocity = 0.0f;
        }

        //set animator speed multiplier for sprinting
        if (forwardPressed && sprintPressed)
        {
            playerSpeed = 8.0f;
            animator.SetFloat("AnimSpeedMultiplier", 1.5f);
        }
        else
        {
            playerSpeed = 4.0f;
            animator.SetFloat("AnimSpeedMultiplier", 1.0f);
        }

        //update velocity variable in animator blend tree
        animator.SetFloat(VelocityHash, velocity);
    }

}
