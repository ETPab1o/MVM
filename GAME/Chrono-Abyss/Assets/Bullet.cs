
using UnityEngine;
public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    private Transform shootPoint; // Reference to the shoot point for maintaining Y position
    private Transform player;
    private Vector3 targetPosition;
    private void Start()
    {
        shootPoint = GameObject.FindGameObjectWithTag("ShootingPoint").transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Destroy(gameObject, lifetime); // Destroy the bullet after a certain time
        // Calculate the initial target position
        if (player != null)
        {
            targetPosition = new Vector3(player.position.x, shootPoint.position.y, player.position.z);
        }
    }
    private void Update()
    {
        if (player != null)
        {
            // Update the target position while maintaining the Y position of the shoot point
            targetPosition = new Vector3(player.position.x, shootPoint.position.y, player.position.z);
            Vector3 direction = (targetPosition - transform.position).normalized;
            // Move towards the target position
            transform.position += direction * speed * Time.deltaTime;
            // Only rotate around the Y-axis
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            transform.rotation = targetRotation;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Check if the player is on top of the bullet
                ContactPoint contact = collision.contacts[0];
                if (contact.normal.y > 0.5f)
                {
                    // Player is on top of the bullet, make the player jump
                    playerController.GetComponent<Rigidbody>().AddForce(Vector3.up * 5f, ForceMode.Impulse);
                }
                else
                {
                    // Player is hit by the side of the bullet, apply some effect (optional)
                }
            }
        }
    }
}