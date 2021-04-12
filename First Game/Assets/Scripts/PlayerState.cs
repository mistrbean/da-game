using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public Animator animator;
    public GameObject virtualCam;

    public GameObject equippedWeapon;
    public GameObject secondWeapon;
    public GameObject hand;
    public BoxCollider weaponCollider;

    public bool attacking;
    public int playerDamage;
    public bool lockRotation;
    public float playerSpeed = 6f;
    public float returnSpeed; //holds current speed to return to after attack

    //pick-up prompt
    public GameObject promptPickup;
    public GameObject pausePanel;

    public Ability ability1;
    public Ability ability2;

    //pause menu
    public bool paused;

    private void Start()
    {
        animator = GetComponent<Animator>();
        weaponCollider = hand.GetComponent<BoxCollider>();
        weaponCollider.enabled = false;
        equippedWeapon = hand;
        attacking = false;
        paused = false;
        ability1 = gameObject.AddComponent<LaserBeam>();
        ability2 = gameObject.AddComponent<LegSpin>();
    }

    public void StartAttack()
    {
        attacking = true;
        weaponCollider.enabled = true;
        lockRotation = true;
        animator.SetBool("Attacking", true);
        returnSpeed = playerSpeed;
        playerSpeed = 1.5f;
        Debug.Log("Slowing movement to " + playerSpeed);
    }

    public void StopAttack()
    {
        weaponCollider.enabled = false;
        playerSpeed = returnSpeed;
        Debug.Log("Returning movement.");
        lockRotation = false;
        attacking = false;
        animator.SetBool("Attacking", false);
    }

    public void SetIdle()
    {
        animator.SetBool("isIdle", true);
    }

    public void UnsetIdle()
    {
        animator.SetBool("isIdle", false);
    }

    public void SetPlayerSpeed(float playerSpeed)
    {
        if (!attacking)
        {
            this.playerSpeed = playerSpeed;
        }
    }

    public void ReturnPlayerSpeed()
    {
        this.playerSpeed = returnSpeed;
    }

    public void PickupItem(GameObject item)
    {
        if (item.CompareTag("Weapon")) EquipWeapon(item);
    }

    public void EquipWeapon(GameObject weapon)
    {
        if (this.equippedWeapon != weapon)
        {
            //if picking up second item
            if (this.secondWeapon == null && this.equippedWeapon != null && this.equippedWeapon != hand)
            {
                this.equippedWeapon.SetActive(false);
                this.secondWeapon = equippedWeapon;
            }
            WeaponController weaponController = weapon.GetComponent<WeaponController>();
            weapon.transform.parent = this.hand.transform;
            weapon.transform.localPosition = weaponController.PrefPosition;
            weapon.transform.localEulerAngles = weaponController.PrefRotation;
            this.equippedWeapon = weapon;
            this.weaponCollider = weapon.GetComponent<BoxCollider>();
            weapon.GetComponent<SphereCollider>().enabled = false;

            Debug.Log("Equipped weapon " + this.equippedWeapon.ToString());
        }
    }

    public void SwapWeapons()
    {
        if (this.secondWeapon != null && !attacking)
        {
            this.equippedWeapon.SetActive(false);
            this.secondWeapon.SetActive(true);
            GameObject swapWeapon = this.equippedWeapon;
            this.equippedWeapon = this.secondWeapon;
            this.secondWeapon = swapWeapon;
            this.weaponCollider.enabled = false;
            this.weaponCollider = this.equippedWeapon.GetComponent<BoxCollider>();
        }
    }

    //set pick-up prompt visibility to check
    public void CheckPrompt(bool check)
    {
        promptPickup.SetActive(check);
    }

    //set pause menu visibility to opposite of current visibility
    public void CheckPause()
    {
        if (pausePanel.activeSelf)
        {
            ClosePause();
        }
        else
        {
            OpenPause();
        }

    }

    //set pause menu visibility to true
    public void OpenPause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        this.paused = true;
        virtualCam.SetActive(false);
    }

    //set pause menu visiblity to false
    public void ClosePause()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        this.paused = false;
        virtualCam.SetActive(true);
    }

    //lock/unlock character rotation so that they are always facing forward
    public void LockRotation(bool check)
    {
        this.lockRotation = check;
    }
}
