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
}
