using UnityEngine;
using System.Collections;

public class Enemy : EnemyBase
{
    public float speed = 3f;
    private Transform player;
    private bool isIdle = false;
    private Rigidbody rb;
    public GameObject destroyEffect;
    public Animator animator;  // Animator component reference
    private Renderer modelRenderer;
    private Material originalMaterial;
    private Material idleMaterial;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        modelRenderer = GetComponentInChildren<Renderer>();

        if (modelRenderer != null)
        {
            originalMaterial = modelRenderer.material;
            idleMaterial = new Material(originalMaterial);
            idleMaterial.color = Color.cyan;
        }

        StartCoroutine(RunTowardsPlayer());
    }

    void Update()
    {
        if (!isIdle)
        {
            Vector3 direction = transform.forward;
            rb.velocity = new Vector3(direction.x * speed * Time.deltaTime * 50, rb.velocity.y, direction.z * speed * Time.deltaTime * 50);
            animator.SetBool("isWalking", true);  // Set walking animation
            modelRenderer.material = originalMaterial;  // Set normal color
        }
        else
        {
            animator.SetBool("isWalking", false);  // Stop walking animation
            modelRenderer.material = idleMaterial;  // Set idle color
        }

        if (transform.position.y <= -3f)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator RunTowardsPlayer()
    {
        while (true)
        {
            isIdle = true;
            rb.velocity = Vector3.zero;
            animator.SetBool("isIdle", true);  // Set idle animation

            float idleTime = 2f;
            while (idleTime > 0)
            {
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                directionToPlayer.y = 0;
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
                idleTime -= Time.deltaTime;
                yield return null;
            }

            isIdle = false;
            animator.SetBool("isIdle", false);  // Stop idle animation
            yield return new WaitForSeconds(2f * Time.timeScale);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null && playerController.IsFalling() && !isIdle)
            {
                playerController.DestroyEnemy(gameObject);
                Instantiate(destroyEffect, transform.position, Quaternion.identity);
                DestroyEnemy(); // Notify GameManager and destroy the enemy
            }
            else
            {
                playerController.TakeDamage();
            }
        }
    }
}
