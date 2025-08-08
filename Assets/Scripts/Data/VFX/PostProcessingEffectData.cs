using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Data structure for post-processing effect VFX.
/// Handles profile switching, timing, and fade transitions.
/// </summary>
[System.Serializable]
public class PostProcessingEffectData : VFXDataBase
{
    [Header("Post-Processing Settings")]
    [Tooltip("Post-processing profile to apply during this effect.")]
    public VolumeProfile postProcessingProfile;

    [Header("Timing Configuration")]
    [Tooltip("Duration of the fade-in transition (seconds).")]
    [Range(0f, 5f)]
    public float fadeInDuration = 0.2f;

    [Tooltip("Duration to maintain the effect at full intensity (seconds).")]
    [Range(0f, 30f)]
    public float mainDuration = 1f;

    [Tooltip("Duration of the fade-out transition (seconds).")]
    [Range(0f, 5f)]
    public float fadeOutDuration = 0.5f;

    [Header("Blending")]
    [Tooltip("Maximum weight/intensity of the effect (0-1).")]
    [Range(0f, 1f)]
    public float maxWeight = 1f;

    [Tooltip("How this effect blends with other active post-processing effects.")]
    public VolumeBlendMode blendMode = VolumeBlendMode.Override;

    /// <summary>
    /// Gets the total duration of the post-processing effect.
    /// </summary>
    public float TotalDuration => fadeInDuration + mainDuration + fadeOutDuration;

    public override VFXType VFXType => VFXType.PostProcessingEffect;

    public override bool IsValid()
    {
        return base.IsValid() && postProcessingProfile != null && TotalDuration > 0f;
    }
}

/// <summary>
/// Blend modes for volume effects.
/// </summary>
public enum VolumeBlendMode
{
    Override,
    Additive,
    Multiply
}

