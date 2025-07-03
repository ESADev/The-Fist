using UnityEngine;

/// <summary>
/// Controls the orthographic camera's movement and zoom.
/// This component is responsible for following a target, smoothly zooming,
/// and recentering the view. It is designed to be modular and data-driven,
/// using a CameraSettings ScriptableObject for its configuration.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("The ScriptableObject containing the camera settings.")]
    [SerializeField] private CameraSettings settings;

    [Tooltip("The target transform for the camera to follow.")]
    [SerializeField] private Transform target;

    [Tooltip("The camera to control.")]
    [SerializeField] private Camera _camera;

    [Header("Debug")]
    [Tooltip("A debug slider to control the zoom level in the inspector.")]
    [Range(0f, 1f)]
    [SerializeField] private float debugZoomSlider = 0f;

    private Vector3 _cameraOffset;
    private Vector3 _velocity = Vector3.zero;
    private float _targetZoom;
    private float _lastMoveTime;

    private void Awake()
    {
        if (_camera == null)
        {
            Debug.LogError("[CameraController] Camera is not assigned!", this);
            this.enabled = false;
            return;
        }

        if (!_camera.orthographic)
        {
            Debug.LogError("[CameraController] This script requires an orthographic Camera component.", this);
            this.enabled = false;
            return;
        }

        if (settings == null)
        { 
            Debug.LogError("[CameraController] CameraSettings is not assigned!", this);
            this.enabled = false;
            return;
        }

        if (target != null)
        {
            // Calculate the initial distance from the target.
            float initialDistance = Vector3.Distance(transform.position, target.position);

            // Position the camera to look at the target from that distance, based on its rotation.
            // This centers the target perfectly on startup.
            transform.position = target.position - (transform.forward * initialDistance);

            // Recalculate the offset based on this new, ideal position.
            _cameraOffset = transform.position - target.position;
        }

        _targetZoom = _camera.orthographicSize;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        HandleFollowing();
        HandleZoom();
    }

    /// <summary>
    /// Handles the camera's following behavior.
    /// </summary>
    private void HandleFollowing()
    {
        // The desired position is the target's position plus the initial offset.
        Vector3 desiredPosition = target.position + _cameraOffset;

        // Check if the camera needs to move
        if (Vector3.Distance(transform.position, desiredPosition) > 0.01f)
        {
            _lastMoveTime = Time.time;
        }

        // If the target has been still for a while, recenter the camera smoothly.
        if (Time.time - _lastMoveTime > settings.centerDelay)
        {
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, 1f / settings.followSpeed);
        }
        else
        {
            // Otherwise, follow the target with a lerp for a smoother feel.
            transform.position = Vector3.Lerp(transform.position, desiredPosition, settings.followSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Handles the camera's zoom behavior.
    /// </summary>
    private void HandleZoom()
    {
        // Update zoom based on the debug slider
        SetZoom(debugZoomSlider);

        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetZoom, settings.zoomSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Sets the desired zoom level as a normalized value (0 to 1).
    /// This provides a protected interface for external scripts to control the zoom.
    /// </summary>
    /// <param name="normalizedZoom">The desired zoom level, from 0 (min) to 1 (max).</param>
    public void SetZoom(float normalizedZoom)
    {
        // Clamp the input value to ensure it's within the valid range.
        normalizedZoom = Mathf.Clamp01(normalizedZoom);

        // Linearly interpolate between the min and max zoom levels.
        _targetZoom = Mathf.Lerp(settings.minZoom, settings.maxZoom, normalizedZoom);
    }
}
