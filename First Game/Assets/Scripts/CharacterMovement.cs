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
    private PlayerState playerState;

    //horizontal movement (running)
    float velocity = 0.0f;
    public float acceleration = 0.1f;
    public float deceleration = 0.5f;
    int VelocityHash;
    public float playerSpeed = 6f;
    public int playerDamage;

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
    }

}
