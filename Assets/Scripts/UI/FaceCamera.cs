using UnityEngine;

/// <summary>
/// Makes a WorldSpace canvas or UI element always face the camera.
/// Useful for health bars, damage numbers, or other UI elements that should remain readable from any angle.
/// </summary>
public class FaceCamera : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("If true, the object will face the camera. If false, it will face away from the camera.")]
    [SerializeField] private bool faceCamera = true;
    
    [Tooltip("If true, only rotates around the Y-axis (world up). Useful for UI elements that should stay upright.")]
    [SerializeField] private bool lockYRotation = false;
    
    [Tooltip("If true, uses LateUpdate for smoother camera following. If false, uses Update.")]
    [SerializeField] private bool useLateUpdate = true;
    
    [Tooltip("Camera to face towards. If null, will automatically find the main camera.")]
    [SerializeField] private Camera targetCamera;

    private Transform cameraTransform;

    private void Start()
    {
        // Find the target camera if not assigned
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            
            // Fallback to any camera with the "MainCamera" tag
            if (targetCamera == null)
            {
                GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
                if (cameraObject != null)
                {
                    targetCamera = cameraObject.GetComponent<Camera>();
                }
            }
            
            // Final fallback to any active camera
            if (targetCamera == null)
            {
                targetCamera = FindFirstObjectByType<Camera>();
            }
        }

        if (targetCamera != null)
        {
            cameraTransform = targetCamera.transform;
        }
        else
        {
            Debug.LogWarning($"[FaceCamera] No camera found for {gameObject.name}. FaceCamera component will be disabled.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (!useLateUpdate)
        {
            UpdateRotation();
        }
    }

    private void LateUpdate()
    {
        if (useLateUpdate)
        {
            UpdateRotation();
        }
    }

    /// <summary>
    /// Updates the rotation to face the camera.
    /// </summary>
    private void UpdateRotation()
    {
        if (cameraTransform == null) return;

        Vector3 directionToCamera;
        
        if (faceCamera)
        {
            // Face towards the camera
            directionToCamera = (cameraTransform.position - transform.position).normalized;
        }
        else
        {
            // Face away from the camera
            directionToCamera = (transform.position - cameraTransform.position).normalized;
        }

        if (lockYRotation)
        {
            // Only rotate around the Y-axis to keep the object upright
            directionToCamera.y = 0f;
            directionToCamera = directionToCamera.normalized;
        }

        // Only rotate if we have a valid direction
        if (directionToCamera != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            transform.rotation = targetRotation;
        }
    }

    /// <summary>
    /// Sets the target camera manually.
    /// </summary>
    /// <param name="camera">The camera to face towards.</param>
    public void SetTargetCamera(Camera camera)
    {
        targetCamera = camera;
        cameraTransform = camera != null ? camera.transform : null;
        
        if (camera == null)
        {
            Debug.LogWarning($"[FaceCamera] Target camera set to null for {gameObject.name}.", this);
        }
    }

    /// <summary>
    /// Toggles between facing towards and away from the camera.
    /// </summary>
    public void ToggleFaceDirection()
    {
        faceCamera = !faceCamera;
    }
}
