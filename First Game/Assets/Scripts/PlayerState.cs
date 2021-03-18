using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public GameObject equippedWeapon;
    public GameObject secondWeapon;
    public GameObject hand;
    public BoxCollider weaponCollider;

    private void Start()
    {
        weaponCollider = hand.GetComponent<BoxCollider>();
        weaponCollider.enabled = false;
        equippedWeapon = hand;
    }

    public void StartAttack()
    {
        weaponCollider.enabled = true;
    }

    public void StopAttack()
    {
        weaponCollider.enabled = false;
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
            BoxCollider weaponCollider = weapon.GetComponent<BoxCollider>();
            SphereCollider pickupCollider = weapon.GetComponent<SphereCollider>();
            pickupCollider.enabled = false;
            this.weaponCollider = weaponCollider;

            Debug.Log("Equipped weapon " + this.equippedWeapon.ToString());
        }
    }

    public void SwapWeapons()
    {
        if (this.secondWeapon != null)
        {
            this.equippedWeapon.SetActive(false);
            this.secondWeapon.SetActive(true);
            GameObject swapWeapon = this.equippedWeapon;
            this.equippedWeapon = this.secondWeapon;
            this.secondWeapon = swapWeapon;
            this.weaponCollider = this.equippedWeapon.GetComponent<BoxCollider>();
        }
    }
}
