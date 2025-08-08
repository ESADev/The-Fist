using UnityEngine;

/// <summary>
/// Data structure for particle effect VFX.
/// Particle effects are automatically played on instantiation and destroyed after completion.
/// </summary>
[System.Serializable]
public class ParticleEffectData : VFXDataBase
{
    [Header("Particle Effect Settings")]
    [Tooltip("Prefab containing the particle system(s) to spawn.")]
    public GameObject particlePrefab;

    [Tooltip("Whether to automatically destroy the effect after all particle systems finish.")]
    public bool autoDestroy = true;

    [Tooltip("Maximum lifetime for the effect (safety fallback if autoDestroy fails).")]
    [Range(0.1f, 60f)]
    public float maxLifetime = 10f;

    [Tooltip("Whether to play the effect immediately upon instantiation.")]
    public bool playOnAwake = true;

    [Tooltip("Scale multiplier for the particle effect.")]
    [Range(0.1f, 5f)]
    public float scale = 1f;

    public override VFXType VFXType => VFXType.ParticleEffect;

    public override bool IsValid()
    {
        return base.IsValid() && particlePrefab != null;
    }
}

