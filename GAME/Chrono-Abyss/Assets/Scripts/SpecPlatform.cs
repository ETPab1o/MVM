using UnityEngine;
using TMPro;

public class SpecPlatform : MonoBehaviour
{
    public Transform[] waypoints;  // Array of waypoints the platform moves between
    public float moveSpeed = 2f;  // Speed of movement in normal time
    public float slowMoveSpeed = 0.5f;  // Speed of movement in slow-mo
    public bool Specific = false;  // Determine if this platform has specific jumping rules
    public TMP_Text timerText;  // Reference to the TMPro text for the timer

    private int currentWaypoint = 0;
    private bool movingForward = true;
    private bool insideSpecificPlatform = false;
    private float timeInside = 0f;
    public float targetTime = 3f;  // Time to wait inside specific platform
    public Renderer rend;
    private bool platformStopped = false;
    public Transform player;
    void Update()
    {
        //if (spawnpoint.)
        //{
        //    moveSpeed = 20;
        //    slowMoveSpeed = 0.5f;
        //}
        //else
        //{
        //    moveSpeed = 0;
        //    slowMoveSpeed = 0;
        //}

        if (platformStopped) return;

        float currentSpeed = Time.timeScale < 1f ? slowMoveSpeed : moveSpeed;

        // Move platform towards the current waypoint
        if (player.position.x >= 70)
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint].position, currentSpeed * Time.deltaTime);


        // Check if reached the current waypoint
        if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.01f)
        {
            // Move to next waypoint or reverse direction if needed
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

        if (!Specific && insideSpecificPlatform)
        {
            timeInside += Time.deltaTime;  // Use unscaled time to account for slow motion
            if (timeInside >= targetTime)
            {
                StopPlatform();
            }

            timerText.text = (targetTime - timeInside).ToString("F1") + "s";  // Update the timer text
        }
        else if (!Specific && !insideSpecificPlatform)
        {
            timeInside = 0f;
            GetComponent<Collider>().isTrigger = false;
            timerText.text = "3.0s";  // Clear the timer text when outside the platform
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Specific && other.CompareTag("SpecificPlatform"))
        {
            insideSpecificPlatform = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Specific && other.CompareTag("SpecificPlatform"))
        {
            insideSpecificPlatform = false;
        }
    }

    private void StopPlatform()
    {
        platformStopped = true;
        GetComponent<Collider>().isTrigger = false;
        rend.material.color = Color.white;  // Change the platform color to black
        timerText.text = "0.0s";  // Ensure the timer shows 0.0s
    }
}
