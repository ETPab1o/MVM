using UnityEngine;

public class SpawnAreaTrigger : MonoBehaviour
{
    public Transform spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.ActivateSpawnPoint(spawnPoint);
                gameObject.SetActive(false);
            }
        }
    }
}
