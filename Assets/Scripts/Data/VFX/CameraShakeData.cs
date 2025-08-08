using UnityEngine;

/// <summary>
/// Data structure for camera shake VFX.
/// Defines the intensity, duration, and characteristics of camera shake.
/// </summary>
[System.Serializable]
public class CameraShakeData : VFXDataBase
{
    [Header("Shake Parameters")]
    [Tooltip("Intensity/magnitude of the camera shake.")]
    [Range(0.1f, 10f)]
    public float magnitude = 1f;

    [Tooltip("Duration of the shake effect (seconds).")]
    [Range(0.1f, 5f)]
    public float duration = 0.5f;

    [Header("Shake Characteristics")]
    [Tooltip("Frequency of the shake oscillation.")]
    [Range(1f, 50f)]
    public float frequency = 25f;

    [Tooltip("How quickly the shake intensity decreases over time.")]
    public AnimationCurve dampening = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    [Tooltip("Whether the shake affects position.")]
    public bool shakePosition = true;

    [Tooltip("Whether the shake affects rotation.")]
    public bool shakeRotation = false;

    [Header("Directional Constraints")]
    [Tooltip("Shake intensity multiplier for X-axis.")]
    [Range(0f, 2f)]
    public float xMultiplier = 1f;

    [Tooltip("Shake intensity multiplier for Y-axis.")]
    [Range(0f, 2f)]
    public float yMultiplier = 1f;

    [Tooltip("Shake intensity multiplier for Z-axis.")]
    [Range(0f, 2f)]
    public float zMultiplier = 1f;

    public override VFXType VFXType => VFXType.CameraShake;

    public override bool IsValid()
    {
        return base.IsValid() && magnitude > 0f && duration > 0f;
    }
}

