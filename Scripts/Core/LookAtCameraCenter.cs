using UnityEngine;

public class LookAtCameraCenter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private bool smoothRotation = true;

    [Header("Aim Settings")]
    [SerializeField] private float maxAimDistance = 1000f;
    [SerializeField] private LayerMask aimMask;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        if (playerCamera == null)
            return;

        Vector3 aimPoint = GetAimPoint();
        TurnedTowards(aimPoint);
    }

    private Vector3 GetAimPoint()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // Если луч попал в объект — целимся в точку попадания
        if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance, aimMask))
        {
            return hit.point;
        }

        // Если не попал — целимся в точку далеко вперед
        return ray.origin + ray.direction * maxAimDistance;
    }

    private void TurnedTowards(Vector3 targetPoint)
    {
        Vector3 direction = (targetPoint - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        if (smoothRotation)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }
}
