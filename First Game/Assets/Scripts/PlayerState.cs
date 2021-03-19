﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    private Animator animator;

    public GameObject equippedWeapon;
    public GameObject secondWeapon;
    public GameObject hand;
    public BoxCollider weaponCollider;

    public bool attacking;
    public int playerDamage;

    //pick-up prompt
    public GameObject promptPickup;
    private bool promptVisible;

    private void Start()
    {
        animator = GetComponent<Animator>();
        weaponCollider = hand.GetComponent<BoxCollider>();
        weaponCollider.enabled = false;
        equippedWeapon = hand;
        attacking = false;
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

    //public pick-up prompt functions

    //check if prompt is visible, if not let it be so
    public void CheckPrompt(bool check)
    {
        promptPickup.SetActive(check);
        promptVisible = check;

        /*if (!promptVisible && check)
        {
            promptPickup.SetActive(true);
            promptVisible = true;
        }
        else if (promptVisible && !check)
        {
            promptPickup.SetActive(false)
        }*/
    }
}
