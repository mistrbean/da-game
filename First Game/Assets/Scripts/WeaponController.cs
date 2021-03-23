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
            other.gameObject.SendMessage("TakeDamage", weaponDamage);
        }
    }

}
