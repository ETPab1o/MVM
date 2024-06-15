using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Array of different enemy prefabs
    public Transform[] spawnPoints;
    public float spawnRadius = 5f;
    public float spawnDelay = 2f;

    private bool playerEnteredSpawnArea = false;
    private List<Transform> activeSpawnPoints = new List<Transform>();
    private List<GameObject> activeEnemies = new List<GameObject>(); // List to track active enemies
    public GameObject Wall;

    void Start()
    {
        // Optionally, you can spawn some enemies at the start
        // SpawnEnemies();
    }

    void Update()
    {
        // Check if the player has entered a spawn area
        if (playerEnteredSpawnArea)
        {
            playerEnteredSpawnArea = false; // Reset the flag
            SpawnEnemies();
        }

        // Check if all enemies are defeated
        CheckEnemiesDefeated();
    }

    public void ActivateSpawnPoint(Transform spawnPoint)
    {
        if (!activeSpawnPoints.Contains(spawnPoint))
        {
            activeSpawnPoints.Add(spawnPoint);
            playerEnteredSpawnArea = true;
            Debug.Log("Activated spawn point: " + spawnPoint.name);
        }
    }

    private void SpawnEnemies()
    {
        foreach (Transform spawnPoint in activeSpawnPoints)
        {
            StartCoroutine(SpawnEnemyAtPoint(spawnPoint));
        }
    }

    private IEnumerator SpawnEnemyAtPoint(Transform spawnPoint)
    {
        yield return new WaitForSeconds(spawnDelay);

        // Find the index of the spawn point in the array
        int spawnPointIndex = System.Array.IndexOf(spawnPoints, spawnPoint);
        if (spawnPointIndex >= 0 && spawnPointIndex < enemyPrefabs.Length)
        {
            GameObject enemyPrefab = enemyPrefabs[spawnPointIndex];
            Vector3 spawnPosition = spawnPoint.position + new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y) * spawnRadius;
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            activeEnemies.Add(enemy); // Add enemy to active enemies list
            Debug.Log("Spawned enemy at: " + spawnPosition);
            if (!Wall.activeInHierarchy)
            {
                Wall.SetActive(true);
            }

            // Add a callback to remove the enemy from the list when it is destroyed
            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                enemyBase.OnDestroyed += () => OnEnemyDestroyed(enemy);
            }
        }
    }

    private void CheckEnemiesDefeated()
    {
        if (activeEnemies.Count == 0 && Wall.activeInHierarchy)
        {
            Wall.SetActive(false);
            Debug.Log("All enemies defeated. Wall deactivated.");
        }
    }

    private void OnEnemyDestroyed(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
        CheckEnemiesDefeated();
    }
}
