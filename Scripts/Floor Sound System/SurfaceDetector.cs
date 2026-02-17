using UnityEngine;

public class SurfaceDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float raycastDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer = ~0;
    [SerializeField] private Vector3 raycastOffset = new Vector3(0, 0.1f, 0);

    [Header("Debug")]
    [SerializeField] private bool showDebugRay = true;
    [SerializeField] private string currentSurfaceTag = "Ground";
    [SerializeField] private PhysicsMaterial currentPhysicsMaterial;

    public string CurrentSurfaceTag => currentSurfaceTag;
    public PhysicsMaterial CurrentPhysicsMaterial => currentPhysicsMaterial;

    private void Update()
    {
        DetectSurface();
    }

    private void DetectSurface()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + raycastOffset;

        if (showDebugRay)
        {
            Debug.DrawRay(rayOrigin, Vector3.down * raycastDistance, Color.red);
        }

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            currentSurfaceTag = hit.collider.tag;
            currentPhysicsMaterial = hit.collider.sharedMaterial as PhysicsMaterial;
        }
        else
        {
            currentSurfaceTag = "Ground";
            currentPhysicsMaterial = null;
        }
    }

    public (string, PhysicsMaterial) GetCurrentSurfaceInfo()
    {
        return (currentSurfaceTag, currentPhysicsMaterial);
    }
}
