using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class RaycastShooter : MonoBehaviour, IShooter
{
    [Header("RaycastShooter Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float range = 1000f;
    [SerializeField] private LayerMask hitMask;

    private Weapon _weapon;

    private void Awake()
    {
        _weapon = GetComponent<Weapon>();

        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    public void Shoot()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
        {
            BaseDamageable damageable = hit.collider.GetComponent<BaseDamageable>();

            if (damageable == null)
            {
                damageable = hit.collider.GetComponentInParent<BaseDamageable>();

                if (damageable == null)
                    damageable = hit.collider.GetComponentInChildren<BaseDamageable>();
            }

            if (damageable != null)
            {
                damageable.TakeDamage(_weapon.Damage, hit.point, hit.normal);
            }

            Debug.DrawLine(ray.origin, hit.point, Color.red, 0.2f);
        }
    }
}
