using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 5f;
    public float dashForce = 10f;
    public int maxHealth = 3;
    public float invincibilityDuration = 2f;
    public float slowDownMultiplier = 2f;
    public float wallBounceForce = 15f;
    public GameObject[] hearts;
    public GameObject slowDownEffect;
    public GameObject dead;
    public Slider slowMotionSlider;
    public float slowMotionDuration = 5f;
    public float slowMotionCooldown = 10f;
    public AudioSource backgroundMusic;
    public AudioSource slowMotionStartSound;
    public AudioSource slowMotionEndSound;

    private int currentHealth;
    private bool isGrounded;
    private bool isInvincible;
    private bool isFalling;
    private bool isStickingToWall;
    private Rigidbody rb;
    private float originalMoveSpeed;
    private Vector3 spawnPoint;
    private float slowMotionTimer;
    private bool isSlowMotionActive;
    private float slowMotionCooldownTimer;

    public GameObject dashParticles;
    public Animator animator;

    private Material originalMaterial;
    private Material flashMaterial;
    private Renderer modelRenderer;
    private bool canMove = true;
    private bool hasKey = false; // Track if the player has the key
    public Vector3 initialSpawnPoint;
    public CameraController cameraController;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        currentHealth = maxHealth;
        UpdateHearts();
        slowDownEffect.SetActive(false);
        originalMoveSpeed = moveSpeed;
        hasKey = false;

        // Load the spawn point from PlayerPrefs
        float spawnX = PlayerPrefs.GetFloat("SpawnPointX", initialSpawnPoint.x);
        float spawnY = PlayerPrefs.GetFloat("SpawnPointY", initialSpawnPoint.y);
        float spawnZ = PlayerPrefs.GetFloat("SpawnPointZ", initialSpawnPoint.z);
        spawnPoint = new Vector3(spawnX, spawnY, spawnZ);

        transform.position = spawnPoint;

        slowMotionTimer = slowMotionDuration;
        slowMotionSlider.maxValue = slowMotionDuration;
        slowMotionSlider.value = slowMotionDuration;

        // Get the renderer and materials
        modelRenderer = GetComponentInChildren<Renderer>();
        if (modelRenderer != null)
        {
            originalMaterial = modelRenderer.material;
            flashMaterial = new Material(originalMaterial);
            flashMaterial.color = Color.red;
        }
    }
    public bool IsJumpingOnTop(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        return contact.normal.y > 0.5f && IsFalling();
    }


    public void PickUpKey()
    {
        hasKey = true;
        Debug.Log("Key picked up!");
    }

    public bool HasKey()
    {
        return hasKey;
    }

    public void UseKey()
    {
        if (hasKey)
        {
            hasKey = false;
            Debug.Log("Key used to open the door.");
        }
    }

    public void EnableMovement(bool enable)
    {
        canMove = enable;
    }

    void Update()
    {
        if (canMove)
        {
            HandleMovement();
            HandleJumping();
            HandleSlowMotion();
        }

        if (transform.position.y <= -3f)
        {
            Respawn();
        }
    }

    void HandleMovement()
    {
        if (!isStickingToWall)
        {
            Vector3 moveDirection = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) moveDirection += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) moveDirection += Vector3.back;
            if (Input.GetKey(KeyCode.A)) moveDirection += Vector3.left;
            if (Input.GetKey(KeyCode.D)) moveDirection += Vector3.right;

            moveDirection = moveDirection.normalized;
            Vector3 targetPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

            if (moveDirection != Vector3.zero)
            {
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

                animator.SetBool("isRunning", true);
            }
            else
            {
                animator.SetBool("isRunning", false);
            }
        }
    }

    void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            else if (isStickingToWall)
            {
                WallJump();
            }
            else
            {
                rb.AddForce(Vector3.down * dashForce, ForceMode.Impulse);
                Dash();
                animator.SetTrigger("Dash");
            }
        }

        if (isStickingToWall && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)))
        {
            WallBounce();
        }

        isFalling = rb.velocity.y < 0;
    }

    void Dash()
    {
        if (dashParticles != null)
        {
            Instantiate(dashParticles, transform.position, Quaternion.identity);
        }
    }

    void HandleSlowMotion()
    {
        if (Input.GetKey(KeyCode.LeftShift) && slowMotionTimer > 0)
        {
            if (!isSlowMotionActive)
            {
                isSlowMotionActive = true;
                Time.timeScale = 0.5f;
                moveSpeed = originalMoveSpeed * slowDownMultiplier;
                slowDownEffect.SetActive(true);
                slowMotionStartSound.Play();
                backgroundMusic.pitch = 0.5f;
                backgroundMusic.volume = 0.5f;
                cameraController.SetZoom(true);
            }
            slowMotionTimer -= Time.deltaTime;
            slowMotionSlider.value = slowMotionTimer;
        }
        else
        {
            if (isSlowMotionActive)
            {
                isSlowMotionActive = false;
                Time.timeScale = 1f;
                moveSpeed = originalMoveSpeed;
                slowDownEffect.SetActive(false);
                slowMotionEndSound.Play();
                backgroundMusic.pitch = 1f;
                backgroundMusic.volume = 1f;
                cameraController.SetZoom(false);
                slowMotionCooldownTimer = slowMotionCooldown;
            }

            if (slowMotionTimer < slowMotionDuration)
            {
                slowMotionCooldownTimer -= Time.deltaTime;
                if (slowMotionCooldownTimer <= 0)
                {
                    slowMotionTimer += Time.deltaTime;
                    slowMotionSlider.value = slowMotionTimer;
                }
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && isFalling)
            {
                Destroy(other.gameObject);
                Debug.Log("Enemy destroyed by player.");
            }
            else
            {
                TakeDamage();
                Debug.Log("Player takes damage.");
            }
        }

        if (other.gameObject.CompareTag("Bullet"))
        {
            {
                TakeDamage();
                Destroy(other.gameObject);
            }
        }
        else if (other.gameObject.CompareTag("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce * 3, ForceMode.Impulse);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            Boss boss = collision.gameObject.GetComponent<Boss>();
            if (IsJumpingOnTop(collision))
            {
                if (boss != null)
                {
                    boss.TakeDamage(2); // Adjust damage value as needed
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Optionally, add a bounce effect
                }
            }
            else if (boss != null && !boss.IsGrounded())
            {
                TakeDamage();
                rb.AddForce(-collision.contacts[0].normal * 5f, ForceMode.Impulse); // Reduced push-back
            }
        }
        
    }





    public bool IsFalling()
    {
        return isFalling;
    }

    public bool IsStickingToWall()
    {
        return isStickingToWall;
    }

    public void TakeDamage(int damageAmount = 1)
    {
        if (isInvincible) return;

        currentHealth -= damageAmount;
        UpdateHearts();
        cameraController.TriggerShake();

        if (currentHealth <= 0)
        {
            dead.SetActive(true);
            Time.timeScale = 0.1f;
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < currentHealth);
        }
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        for (int i = 0; i < 5; i++)
        {
            modelRenderer.material = flashMaterial;
            yield return new WaitForSeconds(0.1f);
            modelRenderer.material = originalMaterial;
            yield return new WaitForSeconds(0.1f);
        }
        isInvincible = false;
    }

    public void DestroyEnemy(GameObject enemy)
    {
        Destroy(enemy);
        Debug.Log("Enemy destroyed by player.");
    }

    public void SetSpawnPoint(Vector3 newSpawnPoint)
    {
        spawnPoint = newSpawnPoint;
        PlayerPrefs.SetFloat("SpawnPointX", newSpawnPoint.x);
        PlayerPrefs.SetFloat("SpawnPointY", newSpawnPoint.y);
        PlayerPrefs.SetFloat("SpawnPointZ", newSpawnPoint.z);
        PlayerPrefs.Save();
    }

    public void Respawn()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void StickToWall()
    {
        isStickingToWall = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }

    public void UnstickFromWall()
    {
        isStickingToWall = false;
        rb.useGravity = true;
    }

    private void WallJump()
    {
        UnstickFromWall();
        rb.AddForce(Vector3.up * jumpForce * 1.5f, ForceMode.Impulse); // Increase the vertical force for higher jumps
    }

    private void WallBounce()
    {
        float bounceDirection = 0;
        if (Input.GetKey(KeyCode.A))
        {
            bounceDirection = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            bounceDirection = 1f;
        }

        if (isStickingToWall)
        {
            UnstickFromWall();
            Vector3 bounceForce = new Vector3(bounceDirection * wallBounceForce, jumpForce * 1.5f, 0); // Increase the vertical force for higher bounces
            rb.AddForce(bounceForce, ForceMode.Impulse);
        }
        else
        {
            WallJump();
        }
    }
}

