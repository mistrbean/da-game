using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    //reference game objects and components
    public Animator animator;
    public Transform cam;
    public ControllerColliderHit lastWall;
    public CharacterController controller;

    //horizontal movement (running)
    private Vector3 moveDir;
    public float velocity = 0.0f;
    public float acceleration = 0.1f;
    public float deceleration = 0.5f;
    private int VelocityHash;
    public float playerSpeed = 6f;

    //aerial movement (jumping/falling)
    Vector3 jumpVelocity;
    public float defGravity;
    public float gravity = -9.81f;
    public bool groundedPlayer;
    public int jumps;
    public int maxJumps;
    public float jumpHeight = 3.0f;
    public bool onWall;
    public bool canAttach;
    public bool canKick; //off of an enemy

    //smooth character turning
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    //dashing
    public bool dashing;
    public float dashTimer = 0.0f;
    public float maxDashTime;

    // Start is called before the first frame update
    void Start()
    {
        //get component references
        animator = GetComponent<Animator>();

        //get velocity parameter id
        VelocityHash = Animator.StringToHash("Velocity");

        dashing = false;
    }

    public void UpdateMovement(CharacterInput input)
    {
        groundedPlayer = controller.isGrounded;
        if (dashing || dashTimer > 0.0f) CheckDash();
        UpdateJumpVelocity();
        if (input.jump) Jump();
        if (input.dash) Dash();

        if (!dashing)
        {
            jumpVelocity.y += gravity * Time.deltaTime;
        }

        if (input.targeting) SetMoveDir(input.direction, input.targeting);
        else SetMoveDir(input.direction);

        bool anythingPressed = input.strafePressed || input.forwardPressed;
        MoveCharacter(input.jump, anythingPressed);
        UpdateAnimVelocity(anythingPressed);
        UpdateAnimSpeedMultiplier(anythingPressed, input.sprint, input.targeting);
        UpdateSidestepAnim(input.forwardPressed, input.strafePressed, input.targeting);

    }

    public void UpdateJumpVelocity()
    {
        if (groundedPlayer && jumpVelocity.y < 0)
        {
            jumpVelocity.y = -2.0f;
            TouchedGround();
        }
        else if (dashing || onWall)
        {
            jumpVelocity.y = 0.0f;
        }
    }

    public void TouchedGround()
    {
        jumps = 0;
        canAttach = true;
        canKick = true;
    }

    public void Jump()
    {
        if (jumps < maxJumps)
        {
            jumps++;
            gravity = defGravity;
            jumpVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void Dash()
    {
        if (dashTimer == 0.0f)
        {
            dashing = true;
            playerSpeed = 16.0f;
            if (!groundedPlayer)
            {
                gravity /= 3;
            }
        }
    }

    public void CheckDash()
    {
        dashTimer += Time.deltaTime;
        //dash lasts [maxDashTime] seconds (ex. 1)
        if ((dashTimer % 60) >= maxDashTime)
        {
            dashing = false;
            playerSpeed = 4.0f;
        }

        //keep gravity lowered for 0.2f, then reset dash timer
        if ((dashTimer % 60) >= maxDashTime + 0.2f)
        {
            dashTimer = 0.0f;
            gravity = defGravity;
        }
    }

    public void UpdateAnimVelocity(bool anythingPressed)
    {
        //animator blend tree (setting velocity)
        //if (forwardPressed && velocity < 1.0f)
        if (anythingPressed && velocity < 1.0f)
        {
            velocity += Time.deltaTime * acceleration;
        }

        //if (!forwardPressed && velocity > 0.0f)
        if (!anythingPressed && velocity > 0.0f)
        {
            velocity -= Time.deltaTime * deceleration;
        }

        //if (!forwardPressed && velocity < 0.0f)
        if (!anythingPressed && velocity < 0.0f)
        {
            velocity = 0.0f;
        }

        //update velocity variable in animator blend tree
        animator.SetFloat(VelocityHash, velocity);
    }

    public void UpdateAnimSpeedMultiplier(bool anythingPressed, bool sprint, bool targeting)
    {
        //set animator speed multiplier for sprinting
        if (anythingPressed && sprint && !targeting)
        {
            playerSpeed = 8.0f;
            animator.SetFloat("AnimSpeedMultiplier", 1.5f);
        }
        else if (!dashing)
        {
            playerSpeed = 4.0f;
            animator.SetFloat("AnimSpeedMultiplier", 1.0f);
        }
    }

    public void UpdateSidestepAnim(bool forwardPressed, bool strafePressed, bool targeting)
    {
        if (!forwardPressed)
        {
            if (targeting && strafePressed)
            {
                animator.SetBool("sideStep", true);
            }
            else
            {
                animator.SetBool("sideStep", false);
            }
        }
        else
        {
            animator.SetBool("sideStep", false);
        }
    }

    public void SetMoveDir(Vector3 direction)
    {
        moveDir = Vector3.zero;

        if (direction.magnitude >= 0.1f)
        {
            //rotate character with camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //smooth turning
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //set rotation
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            //set moveDir
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        

    }

    public void SetMoveDir(Vector3 direction, bool targeting)
    {
        moveDir = Vector3.zero;

        if (targeting)
        {
            //rotate character with camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //smooth turning
            float angle = Mathf.SmoothDampAngle(cam.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //set rotation
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            //set moveDir
            if (direction.magnitude >= 0.1f)
            {
                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            }
        }
    }

    public void MoveCharacter(bool jump, bool anythingPressed)
    {
        if (!onWall)
        {
            controller.Move(((moveDir.normalized * playerSpeed) + jumpVelocity) * Time.deltaTime);
        }
        else if (anythingPressed && !jump)
        {
            moveDir = Vector3.zero;
            controller.Move(((moveDir.normalized * playerSpeed) + jumpVelocity) * Time.deltaTime);
        }
        else if (jump)
        {
            controller.Move(((moveDir.normalized * playerSpeed) + jumpVelocity) * Time.deltaTime);
            onWall = false;
        }
        else
        {
            onWall = false;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //if hitting wall (should somehow change to if jumping into wall)
        if (hit.gameObject.layer == 9)
        {
            if (canAttach || lastWall.gameObject != hit.gameObject)
            {
                onWall = true;
                jumps = 0;
                canAttach = false;
                lastWall = hit;
            }
        }
    }
}
