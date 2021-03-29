using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyCondition : MonoBehaviour
{
    private float maxHealth;
    public float currentHealth;

    //"dead"
    public bool vanquished;
    public bool isRagdoll;

    private Animator animator;
    private CapsuleCollider enemyCollider;
    private Rigidbody[] rigidbodies;
    private Rigidbody myRigidbody;
    

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = (float)PlayerPrefs.GetInt("enemyHealth");
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<CapsuleCollider>();
        isRagdoll = false;
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        myRigidbody = GetComponent<Rigidbody>();
        ToggleRagdoll(false);
    }

    public void TakeDamage(float damage)
    {
        this.currentHealth -= damage;
        Debug.Log(gameObject.ToString() + " took " + damage + " damage.");
        FoeVanquished();
    }

    public void FoeVanquished()
    {
        if (currentHealth <= 0 && !vanquished)
        {
            vanquished = true;
            animator.SetTrigger("Vanquished");
            ToggleRagdoll(vanquished);
            Destroy(gameObject, 7f);
        }
    }

    public void ToggleRagdoll(bool vanquished)
    {
        if (!vanquished)
        {
            foreach (Rigidbody ragdollBone in rigidbodies)
            {
                ragdollBone.isKinematic = true;
            }
        }
        else
        {
            foreach (Rigidbody ragdollBone in rigidbodies)
            {
                ragdollBone.isKinematic = false;
                enemyCollider.enabled = false;
                animator.enabled = false;
            }
        }
    }
}
