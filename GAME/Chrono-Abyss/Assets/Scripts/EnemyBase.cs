using UnityEngine;
using System;

public class EnemyBase : MonoBehaviour
{
    public event Action OnDestroyed;

    protected void DestroyEnemy()
    {
        OnDestroyed?.Invoke();
        Destroy(gameObject);
    }
}
