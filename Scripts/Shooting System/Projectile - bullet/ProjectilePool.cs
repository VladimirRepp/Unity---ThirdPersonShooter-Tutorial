using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Projectile bulletPrefab;
    [SerializeField] private int poolSize = 20;
    [SerializeField] private float maxLifetime = 5; // в секундах 

    private Queue<Projectile> _pool = new Queue<Projectile>();

    public float MaxLifetime => maxLifetime;

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            Create();
        }
    }

    private void Create()
    {
        Projectile bullet = Instantiate(bulletPrefab);
        bullet.gameObject.SetActive(false);
        _pool.Enqueue(bullet);
    }

    public Projectile Get()
    {
        if (_pool.Count == 0)
        {
            Debug.LogError("BulletPool.GetBullet: Pool was null, create one bullet");
            return null;
        }

        Projectile bullet = _pool.Dequeue();
        bullet.gameObject.SetActive(true);
        return bullet;
    }

    public void ReturnToPool(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        _pool.Enqueue(projectile);
    }
}
