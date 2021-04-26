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

    public bool attacking;
    public int playerDamage;
    public bool lockRotation;
    public float playerSpeed = 6f;
    public float returnSpeed; //holds current speed to return to after attack
    public int dashCount; //number of dash charges available
    public int maxDashCount;
    public float dashCooldown;

    public float[] dashChargeCooldowns;
    public float dashChargeCooldown1;
    public float dashChargeCooldown2;
    public float dashChargeCooldown3;

    //pick-up prompt
    public GameObject promptPickup;
    public GameObject pausePanel;
    public GameObject augmentPanel;

    public Ability ability1;
    public Ability ability2;

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
    }

    public void StartAttack()
    {
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
        Debug.Log("Returning movement.");
        lockRotation = false;
        attacking = false;
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
        pausePanel.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        this.paused = true;
        virtualCam.SetActive(false);
    }

    //set pause menu visiblity to false
    public void ClosePause()
    {
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
        augmentPanel.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        this.paused = true;
        virtualCam.SetActive(false);
    }

    public void CloseAugmentScreen()
    {
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
        /*dashCount = 0;
        for (int i = 0; i < dashChargeCooldowns.Length; i++)
        {
            if (dashChargeCooldowns[i] >= dashCooldown) dashCount++;
        }*/
        dashCount = Mathf.Min(dashCount + 1, maxDashCount);
        Debug.Log($"Dash count set to {dashCount}");
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
}
