using System.Runtime.InteropServices;
using Unity.Mathematics;
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
    [SerializeField] private Camera targetCamera;

    [Header("Speed-Based Zoom")]
    [Tooltip("Enable automatic zoom based on player movement speed.")]
    [SerializeField] private bool enableSpeedZoom = true;

    [Tooltip("Speed threshold above which zoom starts changing.")]
    [SerializeField] private float speedThreshold = 0.25f;

    //private Vector3 targetCameraOffset;
    private Vector3 _velocity = Vector3.zero;
    private float _zoomVelocity;
    private float _targetZoom;
    private float _lastMoveTime;
    private float _speedBasedZoom = 1f;
    public float baseDistance = 5;

    private void Awake()
    {
        if (targetCamera == null)
        {
            Debug.LogError("[CameraController] Camera is not assigned!", this);
            this.enabled = false;
            return;
        }

        if (targetCamera.orthographic)
        {
            Debug.LogError("[CameraController] This script requires a perspective Camera component.", this);
            this.enabled = false;
            return;
        }

        if (settings == null)
        { 
            Debug.LogError("[CameraController] CameraSettings is not assigned!", this);
            this.enabled = false;
            return;
        }

        /*if (target != null)
        {
            // Position the camera to look at the target from that distance, based on its rotation.
            // This centers the target perfectly on startup.
            transform.position = target.position - (transform.forward * CalculateDistanceBasedOnFOV(targetCamera.fieldOfView, baseDistance));

            // Recalculate the offset based on this new, ideal position.
            targetCameraOffset = transform.position - target.position;
        }*/

        _targetZoom = targetCamera.orthographicSize;
        _lastMoveTime = Time.time;

        if (target != null)
        {
            Vector3 startupOffset = CalculateCameraOffsetBasedOnFOVAndBaseDistance(targetCamera.fieldOfView, baseDistance);
            transform.position = target.position + startupOffset;
        }
    }

    private void OnEnable()
    {
        if (enableSpeedZoom)
        {
            GameEvents.OnPlayerSpeedChanged += HandlePlayerSpeedChanged;
            Debug.Log("[CameraController] Subscribed to OnPlayerSpeedChanged event");
        }
    }

    private void OnDisable()
    {
        if (enableSpeedZoom)
        {
            GameEvents.OnPlayerSpeedChanged -= HandlePlayerSpeedChanged;
            Debug.Log("[CameraController] Unsubscribed from OnPlayerSpeedChanged event");
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        HandleCameraXRotation();
        HandleFollowing();
        HandleZoom();
    }

    /// <summary>
    /// Handles the camera's following behavior.
    /// </summary>
    private void HandleFollowing()
    {
        // The desired position is the target's position plus the offset.
        Vector3 desiredPosition = target.position + CalculateCameraOffsetBasedOnFOVAndBaseDistance(targetCamera.fieldOfView, baseDistance);

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
        _targetZoom = Mathf.Lerp(settings.minZoom, settings.maxZoom, _speedBasedZoom);
        float smoothTime = Mathf.Max(0.01f, 1f / settings.zoomSpeed);
        targetCamera.fieldOfView = Mathf.SmoothDamp(
            targetCamera.fieldOfView,
            _targetZoom,
            ref _zoomVelocity,
            smoothTime,
            Mathf.Infinity,
            Time.deltaTime
        );
    }

    /// <summary>
    /// Handles player speed changes for speed-based camera zoom.
    /// </summary>
    /// <param name="currentSpeed">Current player movement speed.</param>
    /// <param name="maxSpeed">Maximum player movement speed.</param>
    private void HandlePlayerSpeedChanged(float currentSpeed, float maxSpeed)
    {
        if (!enableSpeedZoom || maxSpeed <= 0f)
        {
            Debug.LogWarning($"[CameraController] Invalid speed values - current: {currentSpeed}, max: {maxSpeed}");
            return;
        }

        // Only apply speed zoom above threshold
        if (currentSpeed > speedThreshold)
        {
            // Map speed to zoom level (0 to 1) - higher speed = more zoom out
            float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);
            _speedBasedZoom = speedRatio;
            Debug.Log($"[CameraController] Speed-based zoom: {_speedBasedZoom:F2} (speed: {currentSpeed:F1}/{maxSpeed:F1})");
        }
        else
        {
            _speedBasedZoom = 0f;
        }
    }

    private void HandleCameraXRotation()
    {
        transform.rotation = Quaternion.Euler(CalculateXRotationDegreesBasedOnFOV(targetCamera.fieldOfView), transform.eulerAngles.y, transform.eulerAngles.z);
    }

    private float CalculateDistanceBasedOnFOV(float FOV, float referenceDistance)
    {
        float dividend = 3;
        float divisor = Mathf.Tan(FOV / 2 * Mathf.Deg2Rad);
        return referenceDistance * dividend / divisor;
    }

    private Vector3 CalculateCameraOffsetBasedOnFOVAndBaseDistance(float FOV, float referenceDistance)
    {
        float distance = CalculateDistanceBasedOnFOV(FOV, referenceDistance);
        return -(transform.forward * distance);
    }

    private float CalculateXRotationDegreesBasedOnFOV(float FOV)
    {
        return FOV/2;
    }
}
