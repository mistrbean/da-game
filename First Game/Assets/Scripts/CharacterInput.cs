using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    public bool jump;
    public bool targeting;
    public bool dash;
    public bool sprint;
    public bool forwardPressed; //"w"
    public bool strafePressed; //"a" or "d" or "s"
    public bool strafeLeft;
    public bool strafeRight;
    public bool backPressed;
    //public bool backwardsPressed //"s" unused until we need a backstep animation/feature
    public Vector3 direction;
    public bool lockRotation;

    //game objects and components
    public Animator animator;
    public Transform cam;
    public GameObject virtualCam;
    private PlayerState playerState;
    private CharacterMovement characterMovement;


    void Awake()
    {
        //lock cursor
        Cursor.lockState = CursorLockMode.Locked;

        //set references for game objects and components
        animator = GetComponent<Animator>();
        virtualCam = GameObject.Find("VirtualPlayerCam");
        playerState = GetComponent<PlayerState>();
        characterMovement = GetComponent<CharacterMovement>();
    }

    void Update()
    {
        /* If opening/closing pause menu */
        if (Input.GetKeyDown("escape"))
        {
            playerState.SendMessage("CheckPause");
        }

        if (Input.GetKeyDown("tab"))
        {
            playerState.SendMessage("CheckAugmentScreen");
        }

        if (playerState.paused) return; //stop getting input if game is paused

        bool abilityOne = Input.GetKeyDown("e");
        bool abilityTwo = Input.GetKeyDown("r");

        /* Get ability input, prioritizing ability1 */
        if (abilityOne && playerState.ability1.useable && !playerState.ability2.inUse) playerState.ability1.UseAbility();
        if (abilityTwo && playerState.ability2.useable && !playerState.ability1.inUse) playerState.ability2.UseAbility();

        /* If looking at equippable item */
        if (LookingAtEquippable(out GameObject item))
        {
            playerState.SendMessage("CheckPrompt", true); //check prompt visibility, enabling if not already enabled
            if (Input.GetKeyDown("f")) //if picking up item
            {
                playerState.SendMessage("PickupItem", item);
            }
        }
        else
        {
            playerState.SendMessage("CheckPrompt", false); //check prompt visibility, disabling if already enabled
        }

        /* --------------------------------- */
        
        /* If trying to swap weapons */
        if (Input.GetKeyDown("q"))
        {
            playerState.SendMessage("SwapWeapons");
        }
        /* --------------------------- */

        /* If trying to zoom in/out */
        Vector2 scroll = Input.mouseScrollDelta;
        if (scroll.y > 0f)
        {
            virtualCam.SendMessage("ZoomIn");
        }
        else if (scroll.y < 0f)
        {
            virtualCam.SendMessage("ZoomOut");
        }
        /* -------------------------- */

        /*    Get movement input     */

        //attack input
        if (!playerState.attacking)
        {
            if (Input.GetMouseButtonDown(0)) animator.SetTrigger("Attack");
        }
        targeting = Input.GetMouseButton(1); //targeting input
        jump = Input.GetKeyDown("space"); //jump input
        if (targeting && jump) //turn jump into dash if targeting
        {
            dash = true;
            jump = false;
        }
        else if (jump)
        {
            dash = false;
            //animator.SetTrigger("Jump");
            animator.SetBool("Grounded", false);
        }
        else
        {
            dash = false;
        }

        if (!targeting && Input.GetKey("left shift")) //sprint input if not targeting
        {
            sprint = true;
        }
        else
        {
            sprint = false;
        }

        //movement input
        forwardPressed = Input.GetKey("w");
        strafeLeft = Input.GetKey("a"); strafeRight = Input.GetKey("d");
        backPressed = Input.GetKey("s");
        strafePressed = strafeLeft || strafeRight || backPressed;
        

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizontal, 0.0f, vertical).normalized;

        if (targeting || playerState.lockRotation) this.lockRotation = true;
        else this.lockRotation = false;


        Ability ability = null;
        if (playerState.ability1 != null && playerState.ability1.inUse) ability = playerState.ability1;
        else if (playerState.ability2 != null && playerState.ability2.inUse) ability = playerState.ability2;

        characterMovement.UpdateMovement(this, ability);

        /* ------------------------- */
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
            if (item == playerState.equippedWeapon) return false;
            return true;
        }
        else
        {
            item = null;
            return false;
        }
    }
}
