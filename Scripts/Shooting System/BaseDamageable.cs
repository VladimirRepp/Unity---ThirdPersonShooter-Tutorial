using System;
using UnityEngine;

public abstract class BaseDamageable : MonoBehaviour
{
    [Header("BaseDamageable Settings")]
    [SerializeField] protected float maxHealth;

    protected float _currentHealth;

    public Action<float> OnHealthChanged;
    public Action OnDeath;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        TakeDamageCore(damage);

        OnHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        TakeDamageCore(damage, hitPoint, hitNormal);

        OnHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    protected abstract void TakeDamageCore(float damage);
    protected abstract void TakeDamageCore(float damage, Vector3 hitPoint, Vector3 hitNormal);
}
