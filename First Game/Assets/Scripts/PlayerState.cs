using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public Animator animator;
    public GameObject virtualCam;

    public GameObject equippedWeapon;
    public GameObject secondWeapon;
    public GameObject hand;
    public BoxCollider weaponCollider;

    public PlayerHUD playerHUD;
    public CharacterMovement characterMovement;

    public bool attacking;
    public int playerDamage;
    public bool lockRotation;
    public float playerSpeed;
    public float defaultSpeed = 4f;
    public float runSpeed = 4f;
    public float sprintSpeed = 8f;
    public float returnSpeed; //holds current speed to return to after attack
    public int dashCount; //number of dash charges available
    public int maxDashCount;
    public float dashCooldown;
    public int enemyLayerMask = 1 << 11; //only collide with objects in "Enemy" layer 11
    public int kickDamage = 0; //jump kick damage (for aerial stomp augment)

    public float dashSpeed = 16f;
    public float defaultDashSpeed = 16f;
    public float[] dashChargeCooldowns;
    public float dashChargeCooldown1;
    public float dashChargeCooldown2;
    public float dashChargeCooldown3;

    //targets for melee assist
    [SerializeField] private int potentialTargetCount = 3;
    [SerializeField] private RaycastHit[] meleeTargets;
    [SerializeField] public GameObject target;
    [SerializeField] private float targetDistance;
    [SerializeField] private float potentialTargetDistance;

    //pick-up prompt
    public GameObject promptPickup;
    public GameObject pausePanel;
    public GameObject augmentPanel;

    public Ability ability1;
    public Ability ability2;

    public Augment headAugment;
    public Augment rightArmAugment;
    public Augment leftArmAugment;
    public Augment chestAugment;
    public Augment waistAugment;
    public Augment rightLegAugment;
    public Augment leftLegAugment;

    public Augment[] collectedAugments;

    //energy
    public int energy;
    public int maxEnergy;

    //pause menu
    public bool paused;

    private void Start()
    {
        animator = GetComponent<Animator>();
        weaponCollider = hand.GetComponent<BoxCollider>();
        weaponCollider.enabled = false;
        equippedWeapon = hand;
        attacking = false;
        paused = false;
        ability1 = gameObject.AddComponent<LaserBeam>();
        ability2 = gameObject.AddComponent<LegSpin>();
        maxEnergy = 100;
        energy = maxEnergy;
        InvokeRepeating(nameof(AccumulateEnergy), 0f, 0.25f);
        maxDashCount = 3;
        dashCount = maxDashCount;
        dashCooldown = 5.0f;
        dashChargeCooldowns = new float[maxDashCount];
        for (int i = 0; i < dashChargeCooldowns.Length; i++) dashChargeCooldowns[i] = dashCooldown;
        meleeTargets = new RaycastHit[potentialTargetCount];
    }

    public void StartAttack()
    {
        target = null;
        targetDistance = -1.0f;
        Vector3 origin = transform.position + transform.forward * 1f;
        if (Physics.SphereCastNonAlloc(origin, 1.0f, transform.forward, meleeTargets, 1.5f, enemyLayerMask) > 0)
        {
            for (int i = 0; i < meleeTargets.Length; i++)
            {
                if (meleeTargets[i].collider != null)
                {
                    if (target == null)
                    {
                        target = meleeTargets[i].collider.gameObject;
                        targetDistance = Vector3.Distance(transform.position, meleeTargets[i].transform.position);
                    }
                    else
                    {
                        potentialTargetDistance = Vector3.Distance(transform.position, meleeTargets[i].transform.position);
                        if (potentialTargetDistance < targetDistance)
                        {
                            target = meleeTargets[i].collider.gameObject;
                            targetDistance = potentialTargetDistance;
                        }
                    }
                    //Debug.Log($"Potential Target: {meleeTargets[i]} | Distance: {Vector3.Distance(transform.position, meleeTargets[i].transform.position)}");
                }
            }
        }
        //Debug.Log($"Chose target {target} | {targetDistance} units away.");
        //if (target != null) target.GetComponent<EnemyCondition>().TakeDamage(5000);
        characterMovement.attackControl = true;
        attacking = true;
        weaponCollider.enabled = true;
        lockRotation = true;
        animator.SetBool("Attacking", true);
        returnSpeed = playerSpeed;
        playerSpeed = 1.5f;
        DrainEnergy(10);
    }

    public void StopAttack()
    {
        weaponCollider.enabled = false;
        playerSpeed = returnSpeed;
        lockRotation = false;
        attacking = false;
        characterMovement.attackControl = true;
        animator.SetBool("Attacking", false);
    }

    public void SetIdle()
    {
        animator.SetBool("isIdle", true);
    }

    public void UnsetIdle()
    {
        animator.SetBool("isIdle", false);
    }

    public void SetPlayerSpeed(float playerSpeed)
    {
        if (!attacking)
        {
            this.playerSpeed = playerSpeed;
        }
    }

    public void ReturnPlayerSpeed()
    {
        this.playerSpeed = returnSpeed;
    }

    public void PickupItem(GameObject item)
    {
        if (item.CompareTag("Weapon")) EquipWeapon(item);
    }

    public void EquipWeapon(GameObject weapon)
    {
        if (this.equippedWeapon != weapon)
        {
            //if picking up second item
            if (this.secondWeapon == null && this.equippedWeapon != null && this.equippedWeapon != hand)
            {
                this.equippedWeapon.SetActive(false);
                this.secondWeapon = equippedWeapon;
            }
            WeaponController weaponController = weapon.GetComponent<WeaponController>();
            weapon.transform.parent = this.hand.transform;
            weapon.transform.localPosition = weaponController.PrefPosition;
            weapon.transform.localEulerAngles = weaponController.PrefRotation;
            this.equippedWeapon = weapon;
            this.weaponCollider = weapon.GetComponent<BoxCollider>();
            weapon.GetComponent<SphereCollider>().enabled = false;

            Debug.Log("Equipped weapon " + this.equippedWeapon.ToString());
        }
    }

    public void SwapWeapons()
    {
        if (this.secondWeapon != null && !attacking)
        {
            this.equippedWeapon.SetActive(false);
            this.secondWeapon.SetActive(true);
            GameObject swapWeapon = this.equippedWeapon;
            this.equippedWeapon = this.secondWeapon;
            this.secondWeapon = swapWeapon;
            this.weaponCollider.enabled = false;
            this.weaponCollider = this.equippedWeapon.GetComponent<BoxCollider>();
        }
    }

    //set pick-up prompt visibility to check
    public void CheckPrompt(bool check)
    {
        promptPickup.SetActive(check);
    }

    //set pause menu visibility to opposite of current visibility
    public void CheckPause()
    {
        if (pausePanel.activeSelf)
        {
            ClosePause();
        }
        else
        {
            OpenPause();
        }

    }

    //set pause menu visibility to true
    public void OpenPause()
    {
        playerHUD.gameObject.SetActive(false);
        pausePanel.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        this.paused = true;
        virtualCam.SetActive(false);
    }

    //set pause menu visiblity to false
    public void ClosePause()
    {
        playerHUD.gameObject.SetActive(true);
        pausePanel.SetActive(false);
        if (!augmentPanel.activeSelf)
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            this.paused = false;
            virtualCam.SetActive(true);
        }
    }

    //lock/unlock character rotation so that they are always facing forward
    public void LockRotation(bool check)
    {
        this.lockRotation = check;
    }

    public void CheckAugmentScreen()
    {
        if (pausePanel.activeSelf) return;
        if (augmentPanel.activeSelf) CloseAugmentScreen();
        else OpenAugmentScreen();
    }

    public void OpenAugmentScreen()
    {
        playerHUD.gameObject.SetActive(false);
        augmentPanel.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        this.paused = true;
        virtualCam.SetActive(false);
    }

    public void CloseAugmentScreen()
    {
        playerHUD.gameObject.SetActive(true);
        augmentPanel.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        this.paused = false;
        virtualCam.SetActive(true);
    }

    public void AccumulateEnergy()
    {
        if (this.energy == maxEnergy) return;
        if (this.energy + 2 > maxEnergy) this.energy = maxEnergy;
        else this.energy += 2;
    }

    public void AccumulateEnergy(int energy)
    {
        if (this.energy == maxEnergy) return;
        if (this.energy + energy > maxEnergy) this.energy = maxEnergy;
        else this.energy += energy;
    }

    public bool DrainEnergy(int energy)
    {
        if (this.energy - energy < 0) return false;
        else
        {
            this.energy -= energy;
            return true;
        }
    }

    public void IncrementDashCount()
    {
        dashCount = Mathf.Min(dashCount + 1, maxDashCount);
        //Debug.Log($"Dash count set to {dashCount}");
        playerHUD.AddDashCharge();
    }

    public void DecrementDashCount()
    {
        dashCount = Mathf.Max(dashCount - 1, 0);
        playerHUD.RemoveDashCharge();
        DashCooldown();
    }

    public void DashCooldown()
    {
        for (int i = dashChargeCooldowns.Length - 1; i >= 0; i--)
        {
            if (dashChargeCooldowns[i] >= dashCooldown)
            {
                dashChargeCooldowns[i] = 0;
                if (i == 2)
                {
                    InvokeRepeating(nameof(IncrementDashCooldown3), 0.1f, 0.1f);
                    return;
                }
                else if (i == 1)
                {
                    InvokeRepeating(nameof(IncrementDashCooldown2), 0.1f, 0.1f);
                    return;
                }
                else if (i == 0)
                {
                    InvokeRepeating(nameof(IncrementDashCooldown1), 0.1f, 0.1f);
                    return;
                }
            }
            
        }

    }

    public void IncrementDashCooldown1()
    {
        dashChargeCooldowns[0] += .1f;
        if (dashChargeCooldowns[0] >= dashCooldown)
        {
            CancelInvoke(nameof(IncrementDashCooldown1));
            IncrementDashCount();
            dashChargeCooldowns[0] = dashCooldown;
        }
    }

    public void IncrementDashCooldown2()
    {
        dashChargeCooldowns[1] += .1f;
        if (dashChargeCooldowns[1] >= dashCooldown)
        {
            CancelInvoke(nameof(IncrementDashCooldown2));
            IncrementDashCount();
            dashChargeCooldowns[1] = dashCooldown;
        }
    }

    public void IncrementDashCooldown3()
    {
        dashChargeCooldowns[2] += .1f;
        if (dashChargeCooldowns[2] >= dashCooldown)
        {
            CancelInvoke(nameof(IncrementDashCooldown3));
            IncrementDashCount();
            dashChargeCooldowns[2] = dashCooldown;
        }
    }

    public bool HasAugment(string augmentName)
    {
        if (headAugment.name == augmentName) return true;
        if (rightArmAugment.name == augmentName) return true;
        if (leftArmAugment.name == augmentName) return true;
        if (chestAugment.name == augmentName) return true;
        if (waistAugment.name == augmentName) return true;
        if (rightLegAugment.name == augmentName) return true;
        if (leftLegAugment.name == augmentName) return true;
        return false;
    }

    public bool HasAugment(string augmentName, int slot)
    {
        switch (slot)
        {
            case 0:
                if (headAugment.name == augmentName) return true;
                else return false;
            case 1:
                if (rightArmAugment.name == augmentName || leftArmAugment.name == augmentName) return true;
                else return false;
            case 2:
                if (rightArmAugment.name == augmentName || leftArmAugment.name == augmentName) return true;
                else return false;
            case 3:
                if (chestAugment.name == augmentName) return true;
                else return false;
            case 4:
                if (waistAugment.name == augmentName) return true;
                else return false;
            case 5:
                if (rightLegAugment.name == augmentName || leftLegAugment.name == augmentName) return true;
                else return false;
            case 6:
                if (rightLegAugment.name == augmentName || leftLegAugment.name == augmentName) return true;
                else return false;
        }
        return false;
    }

    public void EquipAugment(Augment augment, int slot)
    {
        switch (slot)
        {
            case 0:
                UnequipAugment(slot);
                headAugment = augment;
                return;
            case 1:
                UnequipAugment(slot);
                rightArmAugment = augment;
                return;
            case 2:
                UnequipAugment(slot);
                leftArmAugment = augment;
                return;
            case 3:
                UnequipAugment(slot);
                chestAugment = augment;
                if (augment.name == "Volatile Discharge")
                {
                    if (ability1 is LaserBeam laserBeam)
                    {
                        laserBeam.energyCost += 25;
                        laserBeam.SetUseTimeToTick();
                        laserBeam.UpdateTickDamage();
                    }
                    else if (ability2 is LaserBeam laserBeam2)
                    {
                        laserBeam2.energyCost += 25;
                        laserBeam2.SetUseTimeToTick();
                        laserBeam2.UpdateTickDamage();
                    }
                    return;
                }
                return;
            case 4:
                UnequipAugment(slot);
                waistAugment = augment;
                return;
            case 5:
                UnequipAugment(slot);
                rightLegAugment = augment;
                if (augment.name == "Impact Energy") 
                {
                    if (ability1 is LegSpin legSpin) legSpin.UpdateReturnEnergy(5);
                    else if (ability2 is LegSpin legSpin2) legSpin2.UpdateReturnEnergy(5);
                    return;
                }
                if (augment.name == "Increased Potential")
                {
                    characterMovement.maxJumps++;
                    return;
                }
                if (augment.name == "Fresh Oil")
                {
                    this.runSpeed *= 1.25f;
                    this.sprintSpeed *= 1.25f;
                    return;
                }
                if (augment.name == "Explosive Landing")
                {
                    characterMovement.landingDamage = 1;
                    characterMovement.maxLandingDamage = 1000;
                    return;
                }
                if (augment.name == "Overclock")
                {
                    this.dashSpeed *= 2;
                    return;
                }
                if (augment.name == "Aerial Stomp")
                {
                    this.kickDamage = 500;
                    return;
                }
                return;
            case 6:
                UnequipAugment(slot);
                leftLegAugment = augment;
                if (augment.name == "Impact Energy")
                {
                    if (ability1 is LegSpin legSpin) legSpin.UpdateReturnEnergy(5);
                    else if (ability2 is LegSpin legSpin2) legSpin2.UpdateReturnEnergy(5);
                    return;
                }
                if (augment.name == "Increased Potential")
                {
                    characterMovement.maxJumps++;
                    return;
                }
                if (augment.name == "Fresh Oil")
                {
                    this.runSpeed *= 1.25f;
                    this.sprintSpeed *= 1.25f;
                    return;
                }
                if (augment.name == "Explosive Landing")
                {
                    characterMovement.landingDamage = 1;
                    characterMovement.maxLandingDamage = 1000;
                    return;
                }
                if (augment.name == "Overclock")
                {
                    this.dashSpeed *= 2;
                    return;
                }
                if (augment.name == "Aerial Stomp")
                {
                    this.kickDamage = 500;
                    return;
                }
                return;
        }
    }

    public void UnequipAugment(int slot)
    {
        switch (slot)
        {
            case 0:
                headAugment = null;
                return;
            case 1:
                rightArmAugment = null;
                return;
            case 2:
                leftArmAugment = null;
                return;
            case 3:
                if (chestAugment != null)
                {
                    if (chestAugment.name == "Volatile Discharge")
                    {
                        if (ability1 is LaserBeam laserBeam)
                        {
                            laserBeam.energyCost -= 25;
                            laserBeam.RevertUseTime();
                            laserBeam.UpdateTickDamage();
                        }
                        else if (ability2 is LaserBeam laserBeam2)
                        {
                            laserBeam2.energyCost -= 25;
                            laserBeam2.RevertUseTime();
                            laserBeam2.UpdateTickDamage();
                        }
                        chestAugment = null;
                        return;
                    }
                    chestAugment = null;
                }
                return;
            case 4:
                waistAugment = null;
                return;
            case 5:
                if (rightLegAugment != null)
                {
                    if (rightLegAugment.name == "Impact Energy")
                    {
                        if (ability1 is LegSpin legSpin) legSpin.UpdateReturnEnergy(0);
                        else if (ability2 is LegSpin legSpin2) legSpin2.UpdateReturnEnergy(0);
                        rightLegAugment = null;
                        return;
                    }
                    if (rightLegAugment.name == "Increased Potential")
                    {
                        characterMovement.maxJumps--;
                        rightLegAugment = null;
                        return;
                    }
                    if (rightLegAugment.name == "Fresh Oil")
                    {
                        this.runSpeed = defaultSpeed;
                        this.sprintSpeed = runSpeed * 2;
                        rightLegAugment = null;
                        return;
                    }
                    if (rightLegAugment.name == "Explosive Landing")
                    {
                        characterMovement.landingDamage = 0;
                        characterMovement.maxLandingDamage = 0;
                        rightLegAugment = null;
                        return;
                    }
                    if (rightLegAugment.name == "Overclock")
                    {
                        this.dashSpeed /= 2;
                        rightLegAugment = null;
                        return;
                    }
                    if (rightLegAugment.name == "Aerial Stomp")
                    {
                        this.kickDamage = 0;
                        rightLegAugment = null;
                        return;
                    }
                }
                return;
            case 6:
                if (leftLegAugment != null)
                {
                    if (leftLegAugment.name == "Impact Energy")
                    {
                        if (ability1 is LegSpin legSpin) legSpin.UpdateReturnEnergy(0);
                        else if (ability2 is LegSpin legSpin2) legSpin2.UpdateReturnEnergy(0);
                        leftLegAugment = null;
                        return;
                    }
                    if (leftLegAugment.name == "Increased Potential")
                    {
                        characterMovement.maxJumps--;
                        leftLegAugment = null;
                        return;
                    }
                    if (leftLegAugment.name == "Fresh Oil")
                    {
                        this.runSpeed = defaultSpeed;
                        this.sprintSpeed = runSpeed * 2;
                        leftLegAugment = null;
                        return;
                    }
                    if (leftLegAugment.name == "Explosive Landing")
                    {
                        characterMovement.landingDamage = 0;
                        characterMovement.maxLandingDamage = 0;
                        leftLegAugment = null;
                        return;
                    }
                    if (leftLegAugment.name == "Overclock")
                    {
                        this.dashSpeed /= 2;
                        leftLegAugment = null;
                        return;
                    }
                    if (leftLegAugment.name == "Aerial Stomp")
                    {
                        this.kickDamage = 0;
                        leftLegAugment = null;
                        return;
                    }
                }
                return;
        }
    }
}
