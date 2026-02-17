using UnityEngine;

[RequireComponent(typeof(IShooter))]
public class Weapon : MonoBehaviour
{
    [Header("Common Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float fireRate = 0.2f;

    private float _nextFireTime;
    private IShooter _shooter;

    public float Damage => damage;

    private void Awake()
    {
        _shooter = GetComponent<IShooter>();
    }

    public void TryShoot()
    {
        if (Time.time < _nextFireTime)
            return;

        _nextFireTime = Time.time + fireRate;
        _shooter.Shoot();
    }
}
