﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyCondition : MonoBehaviour
{
    private int maxHealth;
    public int currentHealth;

    //"dead"
    public bool vanquished;

    Animator animator;
    

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = PlayerPrefs.GetInt("enemyHealth");
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0 && !vanquished)
        {
            vanquished = true;
            animator.SetBool("vanquished", true);
            transform.Rotate(new Vector3(-90, 0, 0));
        }
    }
}