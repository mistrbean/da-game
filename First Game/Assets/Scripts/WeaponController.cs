using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WeaponController : MonoBehaviour
{
    public GameObject swordImpact;
    private GameObject clone;
    private Vector3 impactPos;

    public Vector3 PrefPosition;
    public Vector3 PrefRotation;

    public int weaponDamage;

    public int iWeaponType; // 0: hammer

    private void Awake()
    {
        swordImpact = GameObject.Find("SwordImpact");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.SendMessage("TakeDamage", weaponDamage);
            impactPos = other.transform.position;
            impactPos.y += 1;
            clone = Instantiate(swordImpact, impactPos, Quaternion.identity);
            Destroy(clone, 0.5f);
        }
    }


}
