using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public GameObject equippedWeapon;
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
            WeaponController weaponController = weapon.GetComponent<WeaponController>();
            weapon.transform.parent = this.hand.transform;
            weapon.transform.localPosition = weaponController.PrefPosition;
            weapon.transform.localEulerAngles = weaponController.PrefRotation;
            this.equippedWeapon = weapon;
            BoxCollider weaponCollider = weapon.GetComponent<BoxCollider>();
            weaponCollider.enabled = false;
            this.weaponCollider = weaponCollider;

            Debug.Log("Equipped weapon " + this.equippedWeapon.ToString());
        }
    }
}
