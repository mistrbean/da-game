using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    public Animator animator;
    float velocity = 0.0f;
    public float acceleration = 0.1f;
    public float deceleration = 0.5f;
    int VelocityHash;
    public float playerSpeed = 6f;
    public int playerDamage;
    private bool isAttacking;

    //jump
    Vector3 jumpVelocity;
    public float defGravity;
    public float gravity = -9.81f;
    public bool groundedPlayer;
    private bool jump;
    public int jumps;
    public int maxJumps;
    public float jumpHeight = 3.0f;
    public bool onWall;
    public bool canAttach;
    public bool canKick; //off of an enemy

    //smooth character turning
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public bool targeting;
    public bool dashing;
    public float dashTimer = 0.0f;
    public float maxDashTime;

    //character face toward camera
    public Transform cam;
    public ControllerColliderHit lastWall;
    public CharacterController controller;
    public GameObject hand;
    public GameObject virtualCam;
    private PlayerState playerState;
    public GameObject promptPickup;

    private bool promptVisible;

    void Start()
    {
        //lock cursor
        Cursor.lockState = CursorLockMode.Locked;

        //set reference for animator
        animator = GetComponent<Animator>();

        //get CinemachineVirtualCamera reference
        virtualCam = GameObject.Find("VirtualPlayerCam");

        //get velocity parameter id
        VelocityHash = Animator.StringToHash("Velocity");
        dashing = false;

        playerState = GetComponent<PlayerState>();
    }

    void Update()
    {
        if (LookingAtEquippable(out GameObject item))
        {
            if (!promptVisible)
            {
                promptPickup.SetActive(true);
                promptVisible = true;
            }
            if (Input.GetKeyDown("f"))
            {
                if (item.CompareTag("Weapon")) playerState.SendMessage("EquipWeapon", item);
            }
        }
        
        if (Input.GetKeyDown("q"))
        {
            playerState.SendMessage("SwapWeapons");
        }

        Vector2 scroll = Input.mouseScrollDelta;
        if (scroll.y > 0f)
        {
            virtualCam.SendMessage("ZoomIn");
        }
        else if (scroll.y < 0f)
        {
            virtualCam.SendMessage("ZoomOut");
        }

        groundedPlayer = controller.isGrounded;
        targeting = Input.GetMouseButton(1);
        if (!isAttacking)
        {
            isAttacking = Input.GetMouseButtonDown(0);
        }
        if (isAttacking)
        {
            animator.SetTrigger("Attack");
            isAttacking = false;
        }

        if (dashing || dashTimer > 0.0f)
        {
            dashTimer += Time.deltaTime;
            //dash lasts [maxDashTime] seconds (ex. 1)
            if ((dashTimer % 60) >= maxDashTime)
            {
                dashing = false;
                playerSpeed = 4.0f;
            }
        }

        if ((dashTimer % 60) >= maxDashTime + 0.2f)
        {
            dashTimer = 0.0f;
            gravity = defGravity;
        }

        if (groundedPlayer && jumpVelocity.y < 0)
        {
            jumpVelocity.y = -2.0f;
            jumps = 0;
            canAttach = true;
            canKick = true;
        }
        else if (dashing || onWall)
        {
            jumpVelocity.y = 0.0f;
        }

        //get key/axis input from player
        bool anythingPressed = Input.GetKey("w") || Input.GetKey("s") || Input.GetKey("a") || Input.GetKey("d");
        if (!Input.GetKey("w"))
        {
            if (targeting && anythingPressed)
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
        bool sprintPressed = Input.GetKey("left shift");
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0.0f, vertical).normalized;

        //update jumpVelocity using physics equation
        if (Input.GetKeyDown("space"))
        {
            if (targeting && dashTimer == 0.0f)
            {
                dashing = true;
                playerSpeed = 16.0f;
                if (!groundedPlayer)
                {
                    gravity /= 3;
                }
            }
            else if (jumps < maxJumps)
            {
                jump = true;
                jumps++;
                gravity = defGravity;
                jumpVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                //animator jump code below?
                
                //
            }
        }

        if (!dashing)
        {
            jumpVelocity.y += gravity * Time.deltaTime;
        }
        

        //declare movement Vector3 (x, y, z)
        Vector3 moveDir = Vector3.zero;

        //move character
        if (direction.magnitude >= 0.1f || targeting)
        {
            //rotate character with camera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            //smooth turning
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //if (!dashing && targeting)
            if (targeting)
            {
                angle = Mathf.SmoothDampAngle(cam.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            }
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //physical movement
            if (direction.magnitude >= 0.1f)
            {
                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            }

        }

        if (!onWall)
        {
            controller.Move(((moveDir.normalized * playerSpeed) + jumpVelocity) * Time.deltaTime);
            jump = false;
        }
        //else if (forwardPressed && !jump)
        else if (anythingPressed && !jump)
        {
            moveDir = Vector3.zero;
            controller.Move(((moveDir.normalized * playerSpeed) + jumpVelocity) * Time.deltaTime);
        }
        else if (jump)
        {
            controller.Move(((moveDir.normalized * playerSpeed) + jumpVelocity) * Time.deltaTime);
            onWall = false;
            jump = false;
        }
        else
        {
            onWall = false;
        }

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

        //set animator speed multiplier for sprinting
        //if (forwardPressed && sprintPressed && !targeting)
        if (anythingPressed && sprintPressed && !targeting)
        {
            playerSpeed = 8.0f;
            animator.SetFloat("AnimSpeedMultiplier", 1.5f);
        }
        else if (!dashing)
        {
            playerSpeed = 4.0f;
            animator.SetFloat("AnimSpeedMultiplier", 1.0f);
        }


        //update velocity variable in animator blend tree
        animator.SetFloat(VelocityHash, velocity);
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
        else if (hit.gameObject.CompareTag("Enemy")) //if hitting enemy
        {
            if (canKick && jump) //if kicking off enemy
            {
                canKick = false;
                hit.gameObject.GetComponent<EnemyCondition>().currentHealth -= playerDamage;
                if (jumps != 0)
                {
                    jumps--;
                }
            }
        }
    }

    private bool LookingAtEquippable(out GameObject item)
    {
        //only collide with objects in "Item" layer 10
        int layerMask = 1 << 10;

        Ray ray = new Ray(cam.position, cam.TransformDirection(Vector3.forward));

        if (Physics.Raycast(ray, out RaycastHit hit, 5.0f, layerMask))
        {
            //Debug.DrawRay(cam.position, cam.TransformDirection(Vector3.forward) * hit.distance, Color.yellow, 1, true);
            item = hit.collider.gameObject;
            return true;
        }
        else
        {
            item = null;
            if (promptVisible)
            {
                promptVisible = false;
                promptPickup.SetActive(false);
            }
            return false;
        }
    }
}
