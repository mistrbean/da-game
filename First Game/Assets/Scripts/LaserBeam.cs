using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : Ability
{
    public Transform cam;
    private float tickRate; //rate that spherecast occurs
    private float tickDamage;
    private float totalDamage;

    public float moveSpeed;
    public float verticalSpeed;

    private RaycastHit[] targets;
    private int layerMask;

    public override void Start()
    {
        base.Start();
        cam = GameObject.Find("Main Camera").transform;

        this.abilityName = "Laser Beam";
        this.moveSpeed = 12f;
        this.verticalSpeed = 35f;
        this.cooldown = 10.0f;
        this.energyCost = 25;
        this.useTime = 5.0f; //5 seconds
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
            InvokeRepeating(nameof(IncrementUseTimer), 0.0f, 0.25f);
            InvokeRepeating(nameof(CheckTargets), 0.0f, tickRate);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckTargets()
    {
        if (useTimer >= useTime)
        {
            CancelInvoke();
            this.useTimer = 0.0f;
            StartCooldown();
        }
        else
        {
            /*RaycastHit[] targets = Physics.SphereCastAll(cam.position, 1.0f, cam.TransformDirection(Vector3.forward), 20.0f, layerMask);
            Debug.DrawRay(cam.position, cam.TransformDirection(Vector3.forward) * 20.0f, Color.yellow, 1, true);
            if (targets.Length != 0)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    targets[i].collider.gameObject.SendMessage("TakeDamage", tickDamage);
                }
            }*/

            Physics.SphereCastNonAlloc(cam.position, 1.0f, cam.TransformDirection(Vector3.forward), targets, 20.0f, layerMask);
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].collider != null)
                targets[i].collider.gameObject.SendMessage("TakeDamage", tickDamage);
            }
        }
    }

}
