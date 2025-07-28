using UnityEngine;

/// <summary>
/// Makes a WorldSpace canvas or UI element align with an orthographic camera's rotation.
/// Instead of facing towards the camera position, it faces the same direction as the camera.
/// Useful for UI elements in orthographic games where camera direction matters more than position.
/// </summary>
public class FaceOrtographicCamera : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("If true, faces the same direction as the camera. If false, faces the opposite direction.")]
    [SerializeField] private bool faceForward = true;
    
    [Tooltip("If true, only rotates around the Y-axis (world up). Useful for UI elements that should stay upright.")]
    [SerializeField] private bool lockYRotation = false;
    
    [Tooltip("If true, uses LateUpdate for smoother camera following. If false, uses Update.")]
    [SerializeField] private bool useLateUpdate = true;
    
    [Tooltip("Camera to align with. If null, will automatically find the main camera.")]
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
            Debug.LogWarning($"[FaceOrtographicCamera] No camera found for {gameObject.name}. FaceOrtographicCamera component will be disabled.", this);
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
    /// Updates the rotation to align with the camera's rotation.
    /// </summary>
    private void UpdateRotation()
    {
        if (cameraTransform == null) return;

        Quaternion targetRotation;
        
        if (faceForward)
        {
            // Face the same direction as the camera
            targetRotation = cameraTransform.rotation;
        }
        else
        {
            // Face the opposite direction from the camera
            targetRotation = Quaternion.Inverse(cameraTransform.rotation);
        }

        if (lockYRotation)
        {
            // Only rotate around the Y-axis to keep the object upright
            Vector3 eulerAngles = targetRotation.eulerAngles;
            eulerAngles.x = 0f;
            eulerAngles.z = 0f;
            targetRotation = Quaternion.Euler(eulerAngles);
        }

        transform.rotation = targetRotation;
    }

    /// <summary>
    /// Sets the target camera manually.
    /// </summary>
    /// <param name="camera">The camera to align with.</param>
    public void SetTargetCamera(Camera camera)
    {
        targetCamera = camera;
        cameraTransform = camera != null ? camera.transform : null;
        
        if (camera == null)
        {
            Debug.LogWarning($"[FaceOrtographicCamera] Target camera set to null for {gameObject.name}.", this);
        }
    }

    /// <summary>
    /// Toggles between facing forward and backward relative to the camera.
    /// </summary>
    public void ToggleFaceDirection()
    {
        faceForward = !faceForward;
    }
}
