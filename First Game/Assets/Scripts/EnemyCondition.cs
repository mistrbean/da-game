using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyCondition : MonoBehaviour
{
    public AudioSource targetAudio;

    private float maxHealth;
    public float currentHealth;
    public float enemySpeed = 4;

    //"dead"
    public bool vanquished;
    public bool isRagdoll;

    private Animator animator;
    private CapsuleCollider enemyCollider;
    private Rigidbody[] rigidbodies;
    private Rigidbody myRigidbody;

    private GameObject player;
    private PlayerState playerState;
    private Vector3 lookAt;
    private Vector3 rotationMask;

    private GameObject target;
    

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
        player = GameObject.FindGameObjectWithTag("Player");
        playerState = player.GetComponent<PlayerState>();
        rotationMask = new Vector3(0, 1, 0);
        targetAudio = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        maxHealth = (float)PlayerPrefs.GetInt("enemyHealth");
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<CapsuleCollider>();
        isRagdoll = false;
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        myRigidbody = GetComponent<Rigidbody>();
        ToggleRagdoll(false);
        player = GameObject.FindGameObjectWithTag("Player");
        playerState = player.GetComponent<PlayerState>();
    }

    private void Update()
    {
        if (!vanquished)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < 5)
            {
                lookAt = Quaternion.LookRotation(player.transform.position - transform.position).eulerAngles;
                transform.rotation = Quaternion.Euler(Vector3.Scale(lookAt, rotationMask));
                if (target == null)
                {
                    targetAudio.Play();
                    target = player;
                }
                
            }
        }
    }


    public void TakeDamage(float damage)
    {
        this.currentHealth -= damage;
        //Debug.Log(gameObject.ToString() + " took " + damage + " damage.");
        FoeVanquished();
    }

    public void FoeVanquished()
    {
        if (currentHealth <= 0 && !vanquished)
        {
            if (playerState.targetLock == gameObject) playerState.UnsetTarget();
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
