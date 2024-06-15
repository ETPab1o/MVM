using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject door; // Reference to the door GameObject

    private void OnColliderEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null && playerController.HasKey())
            {
                playerController.UseKey();
                Destroy(door); // Remove the door when it opens
            }
            else
            {
                Debug.Log("The door is locked. Find the key to open it.");
            }
        }
    }
}
