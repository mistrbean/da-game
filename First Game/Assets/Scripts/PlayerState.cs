using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    private Animator animator;
    public GameObject virtualCam;

    public GameObject equippedWeapon;
    public GameObject secondWeapon;
    public GameObject hand;
    public BoxCollider weaponCollider;

    public bool attacking;
    public int playerDamage;
    public bool lockRotation;

    //pick-up prompt
    public GameObject promptPickup;
    public GameObject pausePanel;

    public Ability ability1;

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
    }

    public void StartAttack()
    {
        weaponCollider.enabled = true;
    }

    public void StopAttack()
    {
        weaponCollider.enabled = false;
    }

    public void Attacking()
    {
        attacking = true;
        animator.SetBool("Attacking", true);
    }

    public void DoneAttacking()
    {
        attacking = false;
        animator.SetBool("Attacking", false);
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
