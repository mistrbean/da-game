using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController : MonoBehaviour
{
    public Vector3 PrefPosition;
    public Vector3 PrefRotation;

    public int weaponDamage;

    [SerializeField] private PlayerState playerState;
    public int energyReturn = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.SendMessage("TakeDamage", weaponDamage);
            playerState.AccumulateEnergy(energyReturn);
        }
    }

}
