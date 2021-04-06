using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegSpin : Ability
{
    private float tickRate;
    private float tickDamage;
    private float totalDamage;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        this.abilityName = "Leg-o-Copter";
        this.cooldown = 5.0f; //5 second cooldown
        this.useTime = 5.0f; //spin for 5 seconds
        this.takeControl = false;
        this.totalDamage = 2500f;
        
    }


}
