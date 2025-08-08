using UnityEngine;

/// <summary>
/// Base class for all VFX data types.
/// </summary>
[System.Serializable]
public abstract class VFXDataBase
{
    [Header("Base VFX Properties")]
    [Tooltip("Unique identifier used to reference this VFX.")]
    public string key;

    [Tooltip("Display name for this VFX (optional, for editor purposes).")]
    public string displayName;

    [Tooltip("Priority level for this effect (higher values take precedence).")]
    [Range(0, 10)]
    public int priority = 1;

    /// <summary>
    /// Gets the type of VFX this data represents.
    /// </summary>
    public abstract VFXType VFXType { get; }

    /// <summary>
    /// Validates the VFX data configuration.
    /// </summary>
    /// <returns>True if the configuration is valid, false otherwise.</returns>
    public virtual bool IsValid()
    {
        return !string.IsNullOrEmpty(key);
    }
}

/// <summary>
/// Enumeration of supported VFX types.
/// </summary>
public enum VFXType
{
    ParticleEffect,
    PostProcessingEffect,
    CameraShake
}

