using System;
using UnityEngine;

public class DamageableObject : BaseDamageable
{
    [Header("DamageableObject Settings")]
    [SerializeField] private GameObject hitEffect;

    [Header("Destroy Settings")]
    [SerializeField] private bool isDestroyAfterDeath = false;
    [SerializeField][Min(0.1f)] private float destroyDelay = 2f;

    [Header("Debug Settings")]
    [SerializeField] private bool isViewDebug = false;

    protected override void TakeDamageCore(float damage)
    {
        TakeDamage(damage, transform.position, Vector3.up);
    }

    protected override void TakeDamageCore(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        _currentHealth = Math.Max(_currentHealth - damage, 0);

        if (isViewDebug)
            Debug.Log($"{gameObject.name} получил {damage} урона. Здоровье: {_currentHealth}/{maxHealth}");

        if (hitEffect != null)
        {
            // TODO: пул или спавн? 
            GameObject effect = Instantiate(hitEffect, hitPoint, Quaternion.LookRotation(hitNormal));
            effect.transform.SetParent(transform);
            //Destroy(effect, 2f);
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isViewDebug)
            Debug.Log($"{gameObject.name} погиб/уничтожен!");

        if (isDestroyAfterDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }
}
