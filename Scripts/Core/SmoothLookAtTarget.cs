using UnityEngine;

public class SmoothLookAtTarget : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private bool useMainCameraAsTarget = false;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private bool rotateOnlyYAxis = false;
    [SerializeField] private bool lookAwayFromTarget = false;

    [Header("Look Offset")]
    [SerializeField] private Vector3 lookOffset = Vector3.zero;

    private Quaternion targetRotation;
    private Vector3 targetDirection;

    private void Start()
    {
        if (useMainCameraAsTarget && Camera.main != null)
        {
            target = Camera.main.transform;
        }
        else
        {
            Debug.LogError("Main Camera не найдена!");
        }
    }

    private void Update()
    {
        if (target == null) return;

        SmoothLookAt();
    }

    private void SmoothLookAt()
    {
        // Вычисляем направление к цели
        Vector3 targetPos = target.position + lookOffset;
        targetDirection = targetPos - transform.position;

        if (lookAwayFromTarget)
        {
            targetDirection = -targetDirection;
        }

        // Если нужно поворачивать только по Y оси
        if (rotateOnlyYAxis)
        {
            targetDirection.y = 0;
        }

        // Вычисляем целевой поворот
        if (targetDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(targetDirection * -1f);

            // Плавный поворот
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
