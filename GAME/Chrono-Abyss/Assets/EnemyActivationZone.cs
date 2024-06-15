using UnityEngine;

public class EnemyActivationZone : MonoBehaviour
{
    public BigRunningEnemy enemy;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.Activate();
        }
    }
}
