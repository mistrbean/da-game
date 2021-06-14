using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : Ability
{
    public Transform cam;
    private float tickRate; //rate that spherecast occurs
    private float tickDamage;
    private float totalDamage;
    public float defaultUseTime;

    public float moveSpeed;
    public float verticalSpeed;

    private RaycastHit[] targets;
    private int layerMask;

    public override void Start()
    {
        base.Start();
        cam = GameObject.Find("Camera").transform;

        this.abilityName = "Laser Beam";
        this.moveSpeed = 12f;
        this.verticalSpeed = 35f;
        this.cooldown = 10.0f;
        this.energyCost = 25;
        this.useTime = 5.0f; //5 seconds
        this.defaultUseTime = 5.0f;
        this.tickRate = 0.5f; //tick damage every half second
        this.totalDamage = 1500.0f;
        this.tickDamage = totalDamage / (useTime / tickRate);
        this.targets = new RaycastHit[10];
        this.layerMask = 1 << 11; //only collide with objects in "Enemy" layer 11
        this.takeControl = true;
    }

    public override bool UseAbility()
    {
        if (base.UseAbility())
        {
            InvokeRepeating(nameof(this.IncrementUseTimer), 0.0f, 0.1f);
            InvokeRepeating(nameof(CheckTargets), 0.0f, tickRate);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void IncrementUseTimer()
    {
        this.useTimer += 0.1f;
        if (this.useTimer >= this.useTime)
        {
            CancelInvoke(nameof(IncrementUseTimer));
            CancelInvoke(nameof(CheckTargets));
            this.useTimer = 0.0f;
            StartCooldown();
        }
    }

    public void CheckTargets()
    {
        Physics.SphereCastNonAlloc(cam.position, 1.0f, cam.TransformDirection(Vector3.forward), targets, 20.0f, layerMask);
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].collider != null) targets[i].collider.gameObject.SendMessage("TakeDamage", tickDamage);
        }
    }

    public void UpdateTickDamage()
    {
        this.tickDamage = totalDamage / (useTime / tickRate);
    }

    public void SetUseTimeToTick()
    {
        this.useTime = this.tickRate;
    }

    public void RevertUseTime()
    {
        this.useTime = this.defaultUseTime;
    }

}
