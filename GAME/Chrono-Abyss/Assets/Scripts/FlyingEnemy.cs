using UnityEngine;
using System.Collections;

public class FlyingEnemy : EnemyBase
{
    public float hoverHeight = 10f;
    public float speed = 5f;
    public float attackDuration = 0.5f;
    public float detectionRange = 15f;
    public GameObject destroyEffect;

    private Transform player;
    private bool isAttacking = false;
    private Rigidbody rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < detectionRange && !isAttacking)
        {
            StopAllCoroutines();
            StartCoroutine(AttackPlayer());
        }
        else if (!isAttacking)
        {
            HoverAbovePlayer();
        }
        if (transform.position.y <= -2f)
        {
            Destroy(gameObject);
        }
    }

    private void HoverAbovePlayer()
    {
        Vector3 targetPosition = player.position + Vector3.up * hoverHeight;
        Vector3 direction = targetPosition - transform.position;
        rb.velocity = direction.normalized * speed * Time.deltaTime * 50;
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;

        // Dive down
        Vector3 divePosition = player.position;
        float elapsedTime = 0f;

        while (elapsedTime < attackDuration)
        {
            transform.position = Vector3.Lerp(transform.position, divePosition, elapsedTime / attackDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Stay in place for 0.5 seconds
        yield return new WaitForSeconds(0.5f * Time.timeScale);

        // Fly back up to hover height
        elapsedTime = 0f;
        Vector3 returnPosition = player.position + Vector3.up * hoverHeight;
        while (elapsedTime < attackDuration)
        {
            transform.position = Vector3.Lerp(transform.position, returnPosition, elapsedTime / attackDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null && playerController.IsFalling() && Time.timeScale < 1f)
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
