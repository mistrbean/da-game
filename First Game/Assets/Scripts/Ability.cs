using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public PlayerState playerState;
    public string abilityName;
    public float moveSpeed;
    public float verticalSpeed;
    public float cooldown;
    public float cooldownTimer;
    public bool useable;
    public float useTime;
    public float useTimer;
    public bool takeControl; //whether this ability should take control over character movement

    public virtual void Start()
    {
        playerState = GetComponent<PlayerState>();
    }

    public virtual void UseAbility()
    {
        playerState.LockRotation(true);
        this.useable = false;
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
    }

    public virtual void StartCooldown()
    {
        this.cooldownTimer = 0.0f;
        playerState.LockRotation(false);
        Debug.Log("On cooldown");
    }

    public virtual void StopCooldown()
    {
        this.useable = true;
        CancelInvoke();
        Debug.Log("Off cooldown");
    }
}
