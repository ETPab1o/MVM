using UnityEngine;
using System.Collections;

public class JumpingEnemy : EnemyBase
{
    public float speed = 3f;
    private Transform player;
    private bool isIdle = false;
    private Rigidbody rb;
    public GameObject destroyEffect;
    public Renderer enemyRenderer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        StartCoroutine(RunTowardsPlayer());
    }

    void Update()
    {
        if (!isIdle)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector3(direction.x * speed, direction.y * speed, direction.z * speed);
        }
    }

    private IEnumerator RunTowardsPlayer()
    {
        while (true)
        {
            isIdle = true;
            enemyRenderer.material.color = new Color(1, 1, 1, 0.5f);
            rb.velocity = Vector3.zero;

            float idleTime = 2f;
            while (idleTime > 0)
            {
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                directionToPlayer.y = 0; // Keep the rotation in the horizontal plane
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
                idleTime -= Time.deltaTime;
                yield return null;
            }

            isIdle = false;
            enemyRenderer.material.color = Color.gray;

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
