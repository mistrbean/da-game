using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Vector3 PrefPosition;
    public Vector3 PrefRotation;

    public int weaponDamage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyCondition>().currentHealth -= weaponDamage;
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            PlayerState playerState = player.GetComponent<PlayerState>();

            if (playerState.equippedWeapon != this.gameObject)
            {
                transform.parent = playerState.hand.transform;
                transform.localPosition = this.PrefPosition;
                transform.localEulerAngles = this.PrefRotation;
                playerState.equippedWeapon = this.gameObject;
                BoxCollider weaponCollider = this.gameObject.GetComponent<BoxCollider>();
                weaponCollider.enabled = false;
                playerState.weaponCollider = weaponCollider;
            }
        }

    }

}
