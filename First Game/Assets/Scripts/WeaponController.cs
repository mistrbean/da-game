using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Vector3 PrefPosition;
    public Vector3 PrefRotation;

    public int weaponDamage;

    public int iWeaponType; // 0: hammer

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.SendMessage("TakeDamage", weaponDamage);
        }
    }

}
