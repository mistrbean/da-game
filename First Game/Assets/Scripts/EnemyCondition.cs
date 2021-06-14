using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyCondition : MonoBehaviour
{
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
    [SerializeField] private GameObject target;
    private Vector3 targetPos;
    private Vector3 lookAt;
    private Vector3 rotationMask;
    

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

    private void FixedUpdate()
    {
        //transform.rotation = Quaternion.LookRotation(player.transform.position);
        //lookAt = Quaternion.LookRotation(player.transform.position - transform.position).eulerAngles;
        if (!vanquished && target != null)
        {
            lookAt = Quaternion.LookRotation(target.transform.position - transform.position).eulerAngles;
            if (!vanquished)
            {
                transform.rotation = Quaternion.Euler(Vector3.Scale(lookAt, rotationMask));

                /*if (Vector3.Distance(transform.position, target.transform.position) > 1)
                {
                    targetPos = transform.position;
                    targetPos.x = Mathf.MoveTowards(transform.position.x, target.transform.position.x, enemySpeed * Time.deltaTime);
                    targetPos.z = Mathf.MoveTowards(transform.position.z, target.transform.position.z, enemySpeed * Time.deltaTime);

                    myRigidbody.MovePosition(targetPos);
                }*/
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

    public void ResetState()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CancelInvoke("ResetState");
            this.target = other.gameObject;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject == target)
        {
            Invoke("ResetState", 10);
        }
    }
}
