using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public PlayerState playerState;
    public string abilityName;
    public bool useable;
    public bool inUse;
    public float cooldownTimer;
    public float useTimer;

    public float useTime;
    public float cooldown;
    public bool takeControl; //whether this ability should take control over character movement

    public virtual void Start()
    {
        playerState = GetComponent<PlayerState>();
        this.useable = true;
        this.inUse = false;
        this.cooldownTimer = 0.0f;
        this.useTimer = 0.0f;
    }

    public virtual void UseAbility()
    {
        playerState.StopAttack();
        //playerState.DoneAttacking();
        playerState.LockRotation(true);
        this.useable = false;
        this.inUse = true;
        this.useTimer = 0.0f;
    }

    public virtual void IncrementCooldownTimer()
    {
        cooldownTimer += 0.5f;
        if (cooldownTimer >= cooldown) StopCooldown();
    }

    public virtual void IncrementUseTimer()
    {
        this.useTimer += 0.25f;
        if (this.useTimer >= this.useTime)
        {
            CancelInvoke();
            this.useTimer = 0.0f;
            StartCooldown();
        }
    }

    public virtual void StartCooldown()
    {
        this.inUse = false;
        this.cooldownTimer = 0.0f;
        playerState.LockRotation(false);
        InvokeRepeating(nameof(IncrementCooldownTimer), 0.0f, 0.5f);
        Debug.Log("On cooldown");
    }

    public virtual void StopCooldown()
    {
        this.useable = true;
        CancelInvoke();
        Debug.Log("Off cooldown");
    }
}
