using System.Collections;
using UnityEngine;

/// <summary>
/// Handles camera shake effects using data-driven configurations from VFXLibrary.
/// Supports multiple concurrent shakes with different characteristics.
/// </summary>
public class CameraShakeManager : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Library asset containing all VFX configurations.")]
    public VFXLibrary vfxLibrary;

    [Tooltip("Camera transform to apply shake effects to.")]
    public Transform cameraTransform;

    [Header("Runtime Info")]
    [Tooltip("Current accumulated shake offset (for debugging).")]
    [SerializeField] private Vector3 currentShakeOffset;

    /// <summary>
    /// Original position of the camera before any shake effects.
    /// </summary>
    private Vector3 originalPosition;

    /// <summary>
    /// Original rotation of the camera before any shake effects.
    /// </summary>
    private Quaternion originalRotation;

    /// <summary>
    /// Current shake intensity from all active shakes combined.
    /// </summary>
    private Vector3 totalShakeOffset;

    /// <summary>
    /// Current rotational shake from all active shakes combined.
    /// </summary>
    private Vector3 totalRotationShake;

    /// <summary>
    /// Counter for active shake coroutines.
    /// </summary>
    private int activeShakeCount = 0;

    private void Awake()
    {
        if (vfxLibrary == null)
        {
            Debug.LogError("[CameraShakeManager] VFXLibrary is not assigned.", this);
            return;
        }

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
            if (cameraTransform == null)
            {
                Debug.LogError("[CameraShakeManager] Camera transform is not assigned and no main camera found.", this);
                return;
            }
        }

        vfxLibrary.Initialize();
        originalPosition = cameraTransform.localPosition;
        originalRotation = cameraTransform.localRotation;
    }

    /// <summary>
    /// Triggers a camera shake effect by key.
    /// </summary>
    /// <param name="key">Unique identifier for the camera shake effect.</param>
    public void Shake(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("[CameraShakeManager] Shake called with empty key.", this);
            return;
        }

        CameraShakeData shakeData = vfxLibrary.GetCameraShake(key);
        if (shakeData == null)
        {
            Debug.LogWarning($"[CameraShakeManager] Camera shake with key '{key}' not found.", this);
            return;
        }

        StartCoroutine(ExecuteShake(shakeData));
        Debug.Log($"[CameraShakeManager] Started camera shake '{key}' (magnitude: {shakeData.magnitude}, duration: {shakeData.duration}).");
    }

    /// <summary>
    /// Triggers a camera shake effect with magnitude override.
    /// </summary>
    /// <param name="key">Unique identifier for the camera shake effect.</param>
    /// <param name="magnitudeMultiplier">Multiplier for the shake magnitude.</param>
    public void ShakeWithMagnitude(string key, float magnitudeMultiplier)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("[CameraShakeManager] ShakeWithMagnitude called with empty key.", this);
            return;
        }

        CameraShakeData shakeData = vfxLibrary.GetCameraShake(key);
        if (shakeData == null)
        {
            Debug.LogWarning($"[CameraShakeManager] Camera shake with key '{key}' not found.", this);
            return;
        }

        // Create a modified copy with adjusted magnitude
        var modifiedData = new CameraShakeData();
        modifiedData.key = shakeData.key;
        modifiedData.magnitude = shakeData.magnitude * magnitudeMultiplier;
        modifiedData.duration = shakeData.duration;
        modifiedData.frequency = shakeData.frequency;
        modifiedData.dampening = shakeData.dampening;
        modifiedData.shakePosition = shakeData.shakePosition;
        modifiedData.shakeRotation = shakeData.shakeRotation;
        modifiedData.xMultiplier = shakeData.xMultiplier;
        modifiedData.yMultiplier = shakeData.yMultiplier;
        modifiedData.zMultiplier = shakeData.zMultiplier;

        StartCoroutine(ExecuteShake(modifiedData));
        Debug.Log($"[CameraShakeManager] Started camera shake '{key}' with magnitude multiplier {magnitudeMultiplier}.");
    }

    /// <summary>
    /// Executes a camera shake effect over time.
    /// </summary>
    private IEnumerator ExecuteShake(CameraShakeData data)
    {
        activeShakeCount++;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < data.duration)
        {
            float progress = elapsedTime / data.duration;
            float dampeningFactor = data.dampening.Evaluate(progress);
            float currentMagnitude = data.magnitude * dampeningFactor;

            // Generate shake offsets
            Vector3 shakeOffset = Vector3.zero;
            Vector3 rotationShake = Vector3.zero;

            if (data.shakePosition)
            {
                float time = elapsedTime * data.frequency;
                shakeOffset = new Vector3(
                    Mathf.PerlinNoise(time, 0f) * 2f - 1f,
                    Mathf.PerlinNoise(0f, time) * 2f - 1f,
                    Mathf.PerlinNoise(time, time) * 2f - 1f
                ) * currentMagnitude;

                // Apply directional multipliers
                shakeOffset.x *= data.xMultiplier;
                shakeOffset.y *= data.yMultiplier;
                shakeOffset.z *= data.zMultiplier;
            }

            if (data.shakeRotation)
            {
                float time = elapsedTime * data.frequency;
                rotationShake = new Vector3(
                    (Mathf.PerlinNoise(time + 100f, 0f) * 2f - 1f) * currentMagnitude * 2f,
                    (Mathf.PerlinNoise(0f, time + 100f) * 2f - 1f) * currentMagnitude * 2f,
                    (Mathf.PerlinNoise(time + 100f, time + 100f) * 2f - 1f) * currentMagnitude * 2f
                );

                // Apply directional multipliers to rotation
                rotationShake.x *= data.xMultiplier;
                rotationShake.y *= data.yMultiplier;
                rotationShake.z *= data.zMultiplier;
            }

            // Add to total shake (for multiple concurrent shakes)
            totalShakeOffset += shakeOffset;
            totalRotationShake += rotationShake;

            // Apply shake to camera
            ApplyShakeToCamera();

            // Remove this shake's contribution for next frame
            totalShakeOffset -= shakeOffset;
            totalRotationShake -= rotationShake;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        activeShakeCount--;

        // If no more active shakes, reset camera position
        if (activeShakeCount <= 0)
        {
            ResetCameraPosition();
        }
    }

    /// <summary>
    /// Applies the accumulated shake to the camera transform.
    /// </summary>
    private void ApplyShakeToCamera()
    {
        if (cameraTransform == null) return;

        // Apply position shake
        cameraTransform.localPosition = originalPosition + totalShakeOffset;
        currentShakeOffset = totalShakeOffset;

        // Apply rotation shake
        Quaternion rotationOffset = Quaternion.Euler(totalRotationShake);
        cameraTransform.localRotation = originalRotation * rotationOffset;
    }

    /// <summary>
    /// Resets the camera to its original position and rotation.
    /// </summary>
    private void ResetCameraPosition()
    {
        if (cameraTransform == null) return;

        cameraTransform.localPosition = originalPosition;
        cameraTransform.localRotation = originalRotation;
        currentShakeOffset = Vector3.zero;
        totalShakeOffset = Vector3.zero;
        totalRotationShake = Vector3.zero;
    }

    /// <summary>
    /// Stops all active camera shake effects immediately.
    /// </summary>
    public void StopAllShakes()
    {
        StopAllCoroutines();
        activeShakeCount = 0;
        ResetCameraPosition();
        Debug.Log("[CameraShakeManager] All camera shakes stopped.");
    }

    /// <summary>
    /// Gets the current number of active shake effects.
    /// </summary>
    /// <returns>Number of currently running shake coroutines.</returns>
    public int GetActiveShakeCount()
    {
        return activeShakeCount;
    }

    /// <summary>
    /// Checks if any shake effects are currently active.
    /// </summary>
    /// <returns>True if any shakes are active, false otherwise.</returns>
    public bool IsShaking()
    {
        return activeShakeCount > 0;
    }

    private void OnDestroy()
    {
        StopAllShakes();
    }
}
