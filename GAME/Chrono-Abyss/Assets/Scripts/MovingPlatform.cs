using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] waypoints;  // Array of waypoints the platform moves between
    public float moveSpeed = 2f;  // Speed of movement in normal time
    public float slowMoveSpeed = 0.5f;  // Speed of movement in slow-mo
    public float slowMoMultiplier = 0.5f;  // Multiplier for slow motion speed

    private int currentWaypoint = 0;
    private bool movingForward = true;

    void Update()
    {
        float currentSpeed = Time.timeScale < 1f ? slowMoveSpeed : moveSpeed;

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
}
