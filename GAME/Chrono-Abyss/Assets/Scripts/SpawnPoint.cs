using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Color activatedColor = Color.yellow;  // Color when the spawn point is activated
    private bool isActivated = false;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.color = Color.white;  // Initial color
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SetSpawnPoint(transform.position);
                rend.material.color = activatedColor;
                isActivated = true;
            }
        }
    }
}
