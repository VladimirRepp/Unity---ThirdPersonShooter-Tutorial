using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    private float _maxLifetime;
    private float _damage;
    private Rigidbody _rb;
    private ProjectilePool _pool;

    private float _timeElapsed = 0;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CountdownLifetime();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Пуля маленькая - берем первое попадание 
        ContactPoint contact = collision.contacts[0];

        Vector3 hitPoint = contact.point;
        Vector3 hitNormal = contact.normal;

        BaseDamageable damageable = collision.collider.GetComponent<BaseDamageable>();

        if (damageable == null)
        {
            damageable = collision.collider.GetComponentInParent<BaseDamageable>();

            if (damageable == null)
                damageable = collision.collider.GetComponentInChildren<BaseDamageable>();
        }

        if (damageable != null)
        {
            damageable.TakeDamage(_damage, hitPoint, hitNormal);
        }

        if (_pool != null)
            _pool.ReturnToPool(this);
    }

    private void CountdownLifetime()
    {
        _timeElapsed += Time.deltaTime;

        if (_timeElapsed >= _maxLifetime)
        {
            if (_pool != null)
                _pool.ReturnToPool(this);
        }
    }

    // Инициализация пули 
    public void Startup(float damage, Vector3 direction, float speed, ProjectilePool pool, float maxLifetime)
    {
        _damage = damage;
        _pool = pool;
        _maxLifetime = maxLifetime;
        _timeElapsed = 0;

        _rb.linearVelocity = Vector3.zero;
        _rb.AddForce(direction * speed, ForceMode.Impulse);
    }
}
