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
    [SerializeField] private bool sprinting;

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
    public float landingDamage = 0;
    public float maxLandingDamage = 0;

    //smooth character turning
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    //dashing
    public bool dashing;
    public float dashTimer = 0.0f;
    public float maxDashTime;

    //attacking
    public bool attackControl = true;

    //animator paramter hashes
    private int velocityHash;
    private int onWallHash;
    private int groundedHash;
    private int jumpHash;
    private int verticalVelocityHash;
    private int sideStepLeftHash;
    private int sideStepRightHash;
    private int backStepHash;

    // Start is called before the first frame update
    void Start()
    {
        //get component references
        animator = GetComponent<Animator>();
        playerState = GetComponent<PlayerState>();

        //get animator parameter hashes
        velocityHash = Animator.StringToHash("Velocity");
        onWallHash = Animator.StringToHash("OnWall");
        groundedHash = Animator.StringToHash("Grounded");
        jumpHash = Animator.StringToHash("Jump");
        verticalVelocityHash = Animator.StringToHash("VerticalVelocity");
        sideStepLeftHash = Animator.StringToHash("SideStepLeft");
        sideStepRightHash = Animator.StringToHash("SideStepRight");
        backStepHash = Animator.StringToHash("BackStep");

    dashing = false;
    }

    public void UpdateMovement(CharacterInput input, Ability ability)
    {
        this.sprinting = input.sprint;
        groundedPlayer = controller.isGrounded;
        if (!groundedPlayer) animator.SetBool(groundedHash, false);
        if (dashing || dashTimer > 0.0f) CheckDash();
        if (ability != null && ability.inUse && ability.takeControl && !input.targeting)
        {
            //Debug.Log("Ability taking control");
            MoveCharacter(ability);
        }
        else if (playerState.attacking && playerState.target != null && attackControl)
        {
            Vector3 offset = playerState.target.transform.position - transform.position;
            if (offset.magnitude <= 0.75f) attackControl = false;
            else MoveCharacter(true, playerState.target);
        }
        else
        {
            UpdateJumpVelocity();
            if (input.jump) Jump();
            if (input.dash) Dash();
            if (!dashing)
            {
                jumpVelocity.y += gravity * Time.deltaTime;
            }
            //if (input.lockRotation) SetMoveDir(input.direction, input.lockRotation);
            //else SetMoveDir(input.direction);
            SetMoveDir(input.direction, playerState.lockRotation || input.lockRotation);

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
            animator.SetTrigger(jumpHash);
        }
    }

    public void Dash()
    {
        if (dashTimer == 0.0f && playerState.dashCount > 0)
        {
            dashing = true;
            playerState.SetPlayerSpeed(playerState.dashSpeed);
            if (!groundedPlayer)
            {
                gravity /= 3;
            }
            playerState.DecrementDashCount();
        }
    }

    public void CheckDash()
    {
        dashTimer += Time.deltaTime;
        //dash lasts [maxDashTime] seconds (ex. 1)
        if ((dashTimer % 60) >= maxDashTime)
        {
            dashing = false;
            playerState.SetPlayerSpeed(playerState.runSpeed);
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
        //if (anythingPressed && velocity < 1.0f)
        if (anythingPressed)
        {
            if (this.sprinting && velocity < 1.0f) velocity += Time.deltaTime * acceleration;
            else
            {
                if (velocity < 0.5f) velocity += Time.deltaTime * acceleration;
                else if (velocity > 0.51f) velocity -= Time.deltaTime * deceleration;
            }
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
        animator.SetFloat(velocityHash, velocity);
    }

    public void UpdateAnimVerticalVelocity()
    {
        if (groundedPlayer)
        {
            verticalVelocity = 0f;
            animator.SetBool(groundedHash, true);
            return;
        }
        if (jumpVelocity.y > 0) verticalVelocity += Time.deltaTime * verticalAcceleration;
        if (jumpVelocity.y < 0)
        {
            if (verticalVelocity > 0) verticalVelocity = 0;
            else verticalVelocity -= Time.deltaTime * verticalDeceleration;
        }

        animator.SetFloat(verticalVelocityHash, verticalVelocity);
    }

    public void UpdateAnimSpeedMultiplier(bool anythingPressed, bool sprint, bool targeting)
    {
        //set animator speed multiplier for sprinting
        if (anythingPressed && sprint && !targeting)
        {
            if (playerState.playerSpeed != playerState.sprintSpeed) playerState.SetPlayerSpeed(playerState.sprintSpeed);
            this.sprinting = true;
            //animator.SetFloat("AnimSpeedMultiplier", 1.5f);
        }
        else if (!dashing)
        {
            if (playerState.playerSpeed != playerState.runSpeed) playerState.SetPlayerSpeed(playerState.runSpeed);
            this.sprinting = false;
            //animator.SetFloat("AnimSpeedMultiplier", 1.0f);

        }
        else
        {
            this.sprinting = false;
        }
        /*else
        {
            animator.SetFloat("AnimSpeedMultiplier", 1.0f);
        }*/
    }

    public void UpdateAnimator(bool forwardPressed, bool backPressed, bool strafeLeft, bool strafeRight, bool targeting)
    {
        if (!forwardPressed && targeting)
        {
            if (strafeLeft && !strafeRight)
            {
                animator.SetBool(sideStepLeftHash, true);
                animator.SetBool(sideStepRightHash, false);
            } 
            else if (!strafeLeft && strafeRight)
            {
                animator.SetBool(sideStepLeftHash, false);
                animator.SetBool(sideStepRightHash, true);
            }
            else
            {
                animator.SetBool(sideStepLeftHash, false);
                animator.SetBool(sideStepRightHash, false);
            }
            if (backPressed)
            {
                animator.SetBool(backStepHash, true);
            }
            else
            {
                animator.SetBool(backStepHash, false);
            }
        }
        else
        {
            animator.SetBool(sideStepLeftHash, false);
            animator.SetBool(sideStepRightHash, false);
            animator.SetBool(backStepHash, false);
        }
    }

    /*public void SetMoveDir(Vector3 direction)
    {
        moveDir = Vector3.zero;

        if (direction.magnitude >= 0.1f)
        {
            //rotate character with camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //smooth turning
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //set rotation
            if (!onWall) transform.rotation = Quaternion.Euler(0f, angle, 0f);
            //set moveDir
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        

    }*/

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
            if (!onWall && !playerState.dontRotate) transform.rotation = Quaternion.Euler(0f, angle, 0f);
            //set moveDir
            if (direction.magnitude >= 0.1f)
            {
                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            }
        }
        else if (direction.magnitude >= 0.1f)
        {
            //rotate character with camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //smooth turning
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //set rotation
            if (!onWall && !playerState.dontRotate) transform.rotation = Quaternion.Euler(0f, angle, 0f);
            //set moveDir
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
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
        if (angle < 0) playerState.SetPlayerSpeed(((playerSpeed / 50f) * angle) + playerSpeed);
        else playerState.SetPlayerSpeed((((playerSpeed * -1) / 90.0f) * angle ) + playerSpeed);
    }

    public void MoveCharacter(bool jump, bool anythingPressed)
    {
        if (!onWall)
        {
            controller.Move(((moveDir.normalized * playerState.playerSpeed) + jumpVelocity) * Time.deltaTime);
        }
        else if (anythingPressed && !jump)
        {
            moveDir = Vector3.zero;
            controller.Move(((moveDir.normalized * playerState.playerSpeed) + jumpVelocity) * Time.deltaTime);
        }
        else if (jump)
        {
            controller.Move(((moveDir.normalized * playerState.playerSpeed) + jumpVelocity) * Time.deltaTime);
            onWall = false;
            animator.SetBool(onWallHash, false);
        }
        else
        {
            onWall = false;
            animator.SetBool(onWallHash, false);
        }
    }

    public void MoveCharacter(bool attacking, GameObject target)
    {
        if (attacking && target != null)
        {
            Vector3 offset = target.transform.position - transform.position;
            if (offset.magnitude > .75f)
            {
                Vector3 targetPos = target.transform.position;
                targetPos.y = transform.position.y;
                transform.LookAt(targetPos);
                controller.Move(offset.normalized * 10.0f * Time.deltaTime);
                attackControl = true;
            }
            else
            {
                attackControl = false;
            }
            
        }
        else
        {
            attackControl = true;
        }
    }

    public void MoveCharacter(Ability ability)
    {
        if (ability is LaserBeam laser)
        {
            SetPlayerSpeed(laser.moveSpeed);
            //if (playerState.lockRotation) SetMoveDir(Vector3.back, true);
            //else SetMoveDir(Vector3.back);
            SetMoveDir(Vector3.back, playerState.lockRotation);

            jumpVelocity.y = cam.rotation.x * laser.verticalSpeed;
            controller.Move(((moveDir.normalized * playerState.playerSpeed) + jumpVelocity) * Time.deltaTime);
            onWall = false;
            animator.SetBool(onWallHash, false);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //if hitting wall (should somehow change to if jumping into wall)
        if (hit.gameObject.layer == 9 && verticalVelocity != 0)
        {
            if (canAttach || lastWall.gameObject != hit.gameObject)
            {
                onWall = true;
                animator.SetBool(onWallHash, true);
                jumps = 0;
                canAttach = false;
                lastWall = hit;
                //Debug.Log(hit.normal);
                transform.rotation = Quaternion.LookRotation(hit.normal);
                transform.RotateAround(transform.position, transform.up, 180f);
            }
        }
        else if (hit.gameObject.layer == 13 && jumpVelocity.y < -12f && landingDamage > 0)
        {
            //Debug.Log(jumpVelocity.y);
            if (jumpVelocity.y > -16f) landingDamage = Mathf.Min(jumpVelocity.y * -1 * 10, maxLandingDamage);
            else if (jumpVelocity.y > -20) landingDamage = Mathf.Min(jumpVelocity.y * -1 * 20, maxLandingDamage);
            else landingDamage = Mathf.Min(jumpVelocity.y * -1 * 30, maxLandingDamage);

            RaycastHit[] targets = Physics.SphereCastAll(transform.position, 3.0f, cam.TransformDirection(Vector3.up), 0.0f, playerState.enemyLayerMask);
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].collider != null) targets[i].collider.gameObject.SendMessage("TakeDamage", landingDamage);
            }
            Debug.DrawRay(transform.position, transform.forward.normalized * 3.0f, Color.yellow, 5f);
        }
        else if (hit.gameObject.layer == 11 && verticalVelocity != 0 && playerState.kickDamage > 0)
        {
            if (Input.GetKeyDown("space"))
            {
                hit.gameObject.GetComponent<EnemyCondition>().TakeDamage(playerState.kickDamage);
                jumpVelocity.y += 6;
                //controller.Move((Vector3.back * playerState.playerSpeed) + jumpVelocity);
            }
        }
    }
}
