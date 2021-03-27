﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    //reference game objects and components
    public Animator animator;
    public Transform cam;
    public ControllerColliderHit lastWall;
    public CharacterController controller;
    public PlayerState playerState;

    //horizontal movement (running)
    [SerializeField] private Vector3 moveDir;
    public float velocity = 0.0f;
    public float verticalVelocity = 0.0f;
    public float verticalAcceleration;
    public float verticalDeceleration;
    public float acceleration = 0.1f;
    public float deceleration = 0.5f;
    private int VelocityHash;
    public float playerSpeed = 6f;

    //aerial movement (jumping/falling)
    public Vector3 jumpVelocity;
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
        playerState = GetComponent<PlayerState>();

        //get velocity parameter id
        VelocityHash = Animator.StringToHash("Velocity");

        dashing = false;
    }

    public void UpdateMovement(CharacterInput input, bool abilityControlled)
    {
        groundedPlayer = controller.isGrounded;
        if (!groundedPlayer) animator.SetBool("Grounded", false);
        if (dashing || dashTimer > 0.0f) CheckDash();
        UpdateJumpVelocity();
        if (abilityControlled)
        {
            Debug.Log("Ability taking control");
            SetPlayerSpeed(playerState.ability1.moveSpeed);
            if (input.lockRotation) SetMoveDir(input.direction, input.lockRotation);
            else SetMoveDir(input.direction);

            MoveCharacter(playerState.ability1);
        }
        else
        {
            if (input.jump) Jump();
            if (input.dash) Dash();
            if (!dashing)
            {
                jumpVelocity.y += gravity * Time.deltaTime;
            }
            if (input.lockRotation) SetMoveDir(input.direction, input.lockRotation);
            else SetMoveDir(input.direction);

            bool anythingPressed = input.strafePressed || input.forwardPressed;
            MoveCharacter(input.jump, anythingPressed);
            UpdateAnimVelocity(anythingPressed);
            UpdateAnimVerticalVelocity();
            UpdateAnimSpeedMultiplier(anythingPressed, input.sprint, input.targeting);
            UpdateAnimator(input.forwardPressed, input.backPressed, input.strafeLeft, input.strafeRight, input.targeting);
        }

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
        //animator.SetBool("Grounded", true);
    }

    public void Jump()
    {
        if (jumps < maxJumps)
        {
            jumps++;
            gravity = defGravity;
            jumpVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (groundedPlayer)
            {
                animator.SetTrigger("Jump");
                Debug.Log("Triggered jump");
            }
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
        if (anythingPressed && velocity < 1.0f)
        {
            velocity += Time.deltaTime * acceleration;
        }

        if (!anythingPressed && velocity > 0.0f)
        {
            velocity -= Time.deltaTime * deceleration;
        }

        if (!anythingPressed && velocity < 0.0f)
        {
            velocity = 0.0f;
        }

        //update velocity variable in animator blend tree
        animator.SetFloat(VelocityHash, velocity);
    }

    public void UpdateAnimVerticalVelocity()
    {
        if (groundedPlayer)
        {
            verticalVelocity = 0f;
            animator.SetBool("Grounded", true);
            return;
        }
        if (jumpVelocity.y > 0) verticalVelocity += Time.deltaTime * verticalAcceleration;
        if (jumpVelocity.y < 0)
        {
            if (verticalVelocity > 0) verticalVelocity = 0;
            else verticalVelocity -= Time.deltaTime * verticalDeceleration;
        }

        animator.SetFloat("VerticalVelocity", verticalVelocity);
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

    public void UpdateAnimator(bool forwardPressed, bool backPressed, bool strafeLeft, bool strafeRight, bool targeting)
    {
        if (!forwardPressed && targeting)
        {
            if (strafeLeft && !strafeRight)
            {
                animator.SetBool("SideStepLeft", true);
                animator.SetBool("SideStepRight", false);
                animator.SetBool("BackStep", false);
            } 
            else if (!strafeLeft && strafeRight)
            {
                animator.SetBool("SideStepLeft", false);
                animator.SetBool("SideStepRight", true);
                animator.SetBool("BackStep", false);
            }
            else if (backPressed)
            {
                animator.SetBool("SideStepLeft", false);
                animator.SetBool("SideStepRight", false);
                animator.SetBool("BackStep", true);
            }
            else
            {
                animator.SetBool("SideStepLeft", false);
                animator.SetBool("SideStepRight", false);
                animator.SetBool("BackStep", false);
            }
        }
        else
        {
            animator.SetBool("SideStepLeft", false);
            animator.SetBool("SideStepRight", false);
            animator.SetBool("BackStep", false);
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

    public void SetMoveDir(Vector3 direction, bool lockRotation)
    {
        moveDir = Vector3.zero;

        if (lockRotation)
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

    public void SetMoveDir(Vector3 direction, bool lockRotation, bool abilityControlled)
    {
        moveDir = Vector3.zero;

        if (lockRotation)
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
                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.back;
            }
        }
    }

    public void SetPlayerSpeed(float playerSpeed)
    {
        float angle = cam.eulerAngles.x;
        if (angle > 180) angle -= 360;
        if (angle < 0) this.playerSpeed = ((playerSpeed / 50f) * angle) + playerSpeed;
        else this.playerSpeed = (((playerSpeed * -1) / 90.0f) * angle ) + playerSpeed;
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

    public void MoveCharacter(Ability ability)
    {
        jumpVelocity.y = cam.rotation.x * 20f;
        if (ability.abilityName == "Laser Beam")
        {
            controller.Move(((moveDir.normalized * playerSpeed) + jumpVelocity) * Time.deltaTime);
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
