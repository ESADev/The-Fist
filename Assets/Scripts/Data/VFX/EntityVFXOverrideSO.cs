using UnityEngine;

/// <summary>
/// Defines entity-specific VFX key overrides for common gameplay events (damage, death, footsteps).
/// Provides per-entity visual customization similar to <see cref="EntitySFXOverrideSO"/>.
/// </summary>
[CreateAssetMenu(menuName = "VFX/Entity VFX Override", fileName = "EntityVFXOverride")]
public class EntityVFXOverrideSO : ScriptableObject
{
    [Header("Entity VFX Overrides")] 
    [Tooltip("VFX key to play when this entity takes damage. If empty, default VFX Manager key is used.")] 
    public string takeDamageVFXKey = "";

    [Tooltip("VFX key to play when this entity dies. If empty, default VFX Manager key is used.")] 
    public string deathVFXKey = "";

    [Tooltip("VFX key to play for footsteps / movement. If empty, default (or component local) key is used.")] 
    public string stepVFXKey = "";

    /// <summary>
    /// Returns damage VFX key or fallback if not overridden.
    /// </summary>
    public string GetDamageKey(string fallback) => string.IsNullOrEmpty(takeDamageVFXKey) ? fallback : takeDamageVFXKey;

    /// <summary>
    /// Returns death VFX key or fallback if not overridden.
    /// </summary>
    public string GetDeathKey(string fallback) => string.IsNullOrEmpty(deathVFXKey) ? fallback : deathVFXKey;

    /// <summary>
    /// Returns step VFX key or fallback if not overridden.
    /// </summary>
    public string GetStepKey(string fallback) => string.IsNullOrEmpty(stepVFXKey) ? fallback : stepVFXKey;

    /// <summary>
    /// Checks if any override is defined.
    /// </summary>
    public bool HasAnyOverrides() => !string.IsNullOrEmpty(takeDamageVFXKey) || !string.IsNullOrEmpty(deathVFXKey) || !string.IsNullOrEmpty(stepVFXKey);
}
