using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : Ability
{
    public Transform cam;
    private float tickRate; //rate that spherecast occurs
    private float tickDamage;
    private float totalDamage;

    private int layerMask;

    public override void Start()
    {
        base.Start();
        cam = GameObject.Find("Main Camera").transform;

        this.abilityName = "Laser Beam";
        this.moveSpeed = 12f;
        this.cooldown = 3.0f;
        this.useable = true;
        this.cooldownTimer = 0.0f;
        this.useTime = 5.0f; //5 seconds
        this.useTimer = 0.0f;
        this.tickRate = 0.5f; //tick damage every half second
        this.totalDamage = 1500.0f;
        this.tickDamage = totalDamage / (useTime / tickRate);
        this.layerMask = 1 << 11; //only collide with objects in "Enemy" layer 11
        this.takeControl = true;
    }

    public override void UseAbility()
    {
        base.UseAbility();

        InvokeRepeating(nameof(IncrementUseTimer), 0.0f, 0.25f);
        InvokeRepeating(nameof(CheckTargets), 0.0f, tickRate);
    }

    public void CheckTargets()
    {
        if (useTimer >= useTime)
        {
            CancelInvoke();
            this.useTimer = 0.0f;
            StartCooldown();
            InvokeRepeating(nameof(IncrementCooldownTimer), 0.0f, 0.5f);
        }
        else
        {
            RaycastHit[] targets = Physics.SphereCastAll(cam.position, 1.0f, cam.TransformDirection(Vector3.forward), 20.0f, layerMask);
            Debug.DrawRay(cam.position, cam.TransformDirection(Vector3.forward) * 20.0f, Color.yellow, 1, true);
            if (targets.Length != 0)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    targets[i].collider.gameObject.SendMessage("TakeDamage", tickDamage);
                }
            }
        }
    }

}
