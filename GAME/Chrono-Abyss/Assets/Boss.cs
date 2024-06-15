using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public Transform player;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public GameObject minionPrefab;
    public Transform[] minionSpawnPoints;
    public float bulletSpeed = 10f;
    public float attackDuration = 0.5f;
    public float hoverHeight = 10f;
    public float speed = 5f;
    public float detectionRange = 15f;
    public float shootAtPlayerInterval = 1f;
    public float spawnMinionInterval = 1f;
    public int currentHealth = 10;
    public int maxHealth = 10;
    public Slider healthSlider;
    public float activationDistance = 30f;
    public GameObject door; // Added for door activation
    public GameObject damageParticlePrefab; // Added for damage particle
    public GameObject ending; // Added for ending activation
    private bool isActive = false;
    private bool isAttacking = false;
    private bool isShooting = false; // Added for shooting state
    private bool isSpawning = false; // Added for spawning state
    private Rigidbody rb;
    private Collider bossCollider;
    public float rotationSpeed = 10f;
    // Variables for color change
    private Renderer bossRenderer;
    private Color normalColor = Color.red;
    private Color groundedColor = Color.cyan;

    // Audio for damage
    public AudioSource audioSource;
    public AudioClip damageSound;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        bossCollider = GetComponent<Collider>();
        bossRenderer = GetComponent<Renderer>();
        // Set the slider's max value to match the boss's max health
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        SetBossColor(normalColor);
    }

    void Update()
    {
        if (player == null) return;
        if (!isActive)
        {
            if (Vector3.Distance(transform.position, player.position) <= activationDistance)
            {
                isActive = true;
                healthSlider.gameObject.SetActive(true);
                StartCoroutine(BossBehavior());
                if (door != null)
                {
                    door.SetActive(true); // Activate the door when the boss is activated
                }
            }
            return;
        }

        // Toggle isTrigger on when the boss is not attacking and on the ground
        if (!isAttacking && IsGrounded())
        {
            SetColliderTrigger(true);
        }

        if (IsGrounded())
        {
            SetBossColor(groundedColor);
        }
        else
        {
            SetBossColor(normalColor);
        }
    }

    private IEnumerator BossBehavior()
    {
        while (currentHealth > 0)
        {
            Debug.Log("Switching to shoot at player mode.");
            yield return StartCoroutine(ShootAtPlayer());
            Debug.Log("Starting minion spawning.");
            yield return StartCoroutine(SpawnMinions());
            Debug.Log("Starting flying behavior.");
            yield return StartCoroutine(FlyingBehavior());
        }
    }

    private IEnumerator FlyingBehavior()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 10f)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer < detectionRange && !isAttacking)
            {
                yield return StartCoroutine(AttackPlayer());
            }
            else if (!isAttacking)
            {
                HoverAbovePlayer();
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void HoverAbovePlayer()
    {
        if (transform.position.y <= 12f)
        {
            Vector3 targetPosition = player.position + Vector3.up * hoverHeight;
            Vector3 direction = targetPosition - transform.position;
            rb.velocity = direction.normalized * speed * Time.deltaTime * 50;
        }
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        SetColliderTrigger(false); // Turn off trigger during attack
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
        SetColliderTrigger(true); // Turn on trigger after attack if grounded
    }

    private IEnumerator ShootAtPlayer()
    {
        isAttacking = true;
        isShooting = true; // Set shooting state
        SetColliderTrigger(false); // Ensure the boss can be damaged during shooting
        float elapsedTime = 0f;
        while (elapsedTime < 10f)
        {
            // Smoothly move the boss to the ground (y = 1.2f)
            while (Mathf.Abs(transform.position.y - 1.2f) > 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 1.2f, transform.position.z), Time.deltaTime * speed);
                yield return null;
            }
            // Rotate to face the player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.LookRotation(directionToPlayer));
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = directionToPlayer * bulletSpeed;
            yield return new WaitForSeconds(shootAtPlayerInterval);
            elapsedTime += shootAtPlayerInterval;
        }
        isAttacking = false;
        isShooting = false; // Reset shooting state
        SetColliderTrigger(true); // Allow damage when not shooting
    }

    private IEnumerator SpawnMinions()
    {
        isAttacking = true;
        isSpawning = true; // Set spawning state
        SetColliderTrigger(false); // Ensure the boss can be damaged during spawning
        float elapsedTime = 0f;
        while (elapsedTime < 10f)
        {
            // Smoothly move the boss to the ground (y = 1.2f)
            while (Mathf.Abs(transform.position.y - 1.2f) > 0.1f)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 1.2f, transform.position.z), Time.deltaTime * speed);
                yield return null;
            }
            foreach (Transform spawnPoint in minionSpawnPoints)
            {
                Instantiate(minionPrefab, spawnPoint.position, spawnPoint.rotation);
            }
            yield return new WaitForSeconds(spawnMinionInterval);
            elapsedTime += spawnMinionInterval;
        }
        isAttacking = false;
        isSpawning = false; // Reset spawning state
        SetColliderTrigger(true); // Allow damage when not spawning
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // Update the slider value
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
        SetColliderTrigger(true); // Allow damage during other states
        if (damageParticlePrefab != null)
        {
            Instantiate(damageParticlePrefab, transform.position, Quaternion.identity);
        }
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        Debug.Log("Boss defeated!");
        if (ending != null)
        {
            ending.SetActive(true);
        }
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f);
    }

    private void SetColliderTrigger(bool isTrigger)
    {
        bossCollider.isTrigger = isTrigger;
    }

    private void SetBossColor(Color color)
    {
        if (bossRenderer != null)
        {
            bossRenderer.material.color = color;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Check if the player is jumping on the boss
                ContactPoint contact = collision.contacts[0];
                if (contact.normal.y > 0.5f && playerController.IsFalling())
                {
                    // Player jumped on the boss, deal damage to the boss
                    TakeDamage(1); // Adjust damage value as needed
                    playerController.GetComponent<Rigidbody>().AddForce(Vector3.up * playerController.jumpForce, ForceMode.Impulse); // Bounce the player up
                }
            }
        }
    }
}
