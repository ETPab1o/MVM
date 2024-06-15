using UnityEngine;

public class BigRunningEnemy : MonoBehaviour
{
    public Transform[] waypoints;  // Array of waypoints the enemy moves between
    public float moveSpeed = 2f;  // Speed of movement
    public float activationThreshold = 10f;  // Distance threshold for stopping movement
    public Transform activationZone;  // The activation zone's transform

    private int currentWaypoint = 0;
    private bool movingForward = true;
    private bool isActive = false;
    public PlayerController player;
    public Animator animator;  // Animator component reference

    void Update()
    {
        if (isActive)
        {
            MoveEnemy();
            CheckActivationDistance();
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    public void Activate()
    {
        isActive = true;
        animator.SetBool("isWalking", true);  // Start walking animation
    }

    private void MoveEnemy()
    {
        // Calculate the actual movement speed, unaffected by Time.timeScale
        float currentSpeed = moveSpeed * (1f / Time.timeScale);

        // Move towards current waypoint
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint].position, currentSpeed * Time.deltaTime);

        // Check if reached the current waypoint
        if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.01f)
        {
            // Move to next waypoint
            if (movingForward)
            {
                currentWaypoint++;
                if (currentWaypoint >= waypoints.Length)
                {
                    currentWaypoint = waypoints.Length - 1;
                    movingForward = false;
                }
            }
            else
            {
                currentWaypoint--;
                if (currentWaypoint < 0)
                {
                    currentWaypoint = 0;
                    movingForward = true;
                }
            }
        }
    }

    private void CheckActivationDistance()
    {
        if (transform.position.x <= activationThreshold)
        {
            isActive = false;
            animator.SetBool("isWalking", false);  // Stop walking animation
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Kill the player (implement your player kill logic here)
            Debug.Log("Player killed by enemy.");
            player.Respawn();
        }
        else if (other.CompareTag("Destroyable"))
        {
            // Destroy the object
            Destroy(other.gameObject);
        }
    }
}