﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegSpin : Ability
{
    [SerializeField] private GameObject legL;
    [SerializeField] private GameObject legR;
    private BoxCollider legL_collider;
    private BoxCollider legR_collider;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        legL = GameObject.Find("Foot.L");
        legR = GameObject.Find("Foot.R");
        legL.GetComponent<WeaponController>().weaponDamage = 300;
        legR.GetComponent<WeaponController>().weaponDamage = 300;
        legL_collider = legL.GetComponent<BoxCollider>();
        legR_collider = legR.GetComponent<BoxCollider>();
        legL_collider.enabled = false;
        legR_collider.enabled = false;
        this.abilityName = "Leg-o-Copter";
        this.cooldown = 5.0f; //5 second cooldown
        this.useTime = 5.0f; //spin for 5 seconds
        this.takeControl = false;
        this.hitDamage = 300f;
        
    }

    public override void UseAbility()
    {
        base.UseAbility();

        legL_collider.enabled = true;
        legR_collider.enabled = true;

        playerState.animator.SetTrigger("LegSpin");
        InvokeRepeating(nameof(IncrementUseTimer), 0.0f, 0.25f);
    }

    public override void StartCooldown()
    {
        this.inUse = false;
        this.cooldownTimer = 0.0f;
        playerState.LockRotation(false);
        legL_collider.enabled = false;
        legR_collider.enabled = false;
        playerState.animator.SetBool("SpinningLegs", false);
        InvokeRepeating(nameof(IncrementCooldownTimer), 0.0f, 0.5f);
        Debug.Log("On cooldown");
    }

    public void LegSpinning()
    {
        playerState.animator.SetBool("SpinningLegs", true);
    }
}
