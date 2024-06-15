using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public GameObject DoorClosed;
    public GameObject DoorOpen;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.PickUpKey();
                Destroy(gameObject); // Remove the key from the ground
                DoorClosed.SetActive(false);
                DoorOpen.SetActive(true);
            }
        }
    }
}
