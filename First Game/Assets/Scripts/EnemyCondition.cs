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

    private Animator animator;
    

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = (float)PlayerPrefs.GetInt("enemyHealth");
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
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
            transform.Rotate(new Vector3(-90, 0, 0));
        }
    }
}
