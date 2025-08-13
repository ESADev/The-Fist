using UnityEngine;

/// <summary>
/// Defines entity-specific SFX key overrides for common events.
/// Allows different entity types (buildings, soldiers, etc.) to have unique sound effects
/// for the same types of events.
/// </summary>
[CreateAssetMenu(menuName = "Audio/Entity SFX Override", fileName = "EntitySFXOverride")]
public class EntitySFXOverrideSO : ScriptableObject
{
    [Header("Entity Sound Effect Overrides")]
    
    [Tooltip("Sound key to play when this entity takes damage. If empty, uses default SFX Manager key.")]
    public string takeDamageKey = "";
    
    [Tooltip("Sound key to play when this entity dies. If empty, uses default SFX Manager key.")]
    public string deathKey = "";
    
    [Tooltip("Sound key to play when this entity steps/moves (looped). If empty, uses default SFX Manager key.")]
    public string stepKey = "";

    /// <summary>
    /// Gets the take damage sound key for this entity, or returns the fallback if not specified.
    /// </summary>
    /// <param name="fallbackKey">Default key to use if entity override is not specified.</param>
    /// <returns>The sound key to use for take damage events.</returns>
    public string GetTakeDamageKey(string fallbackKey)
    {
        return string.IsNullOrEmpty(takeDamageKey) ? fallbackKey : takeDamageKey;
    }

    /// <summary>
    /// Gets the death sound key for this entity, or returns the fallback if not specified.
    /// </summary>
    /// <param name="fallbackKey">Default key to use if entity override is not specified.</param>
    /// <returns>The sound key to use for death events.</returns>
    public string GetDeathKey(string fallbackKey)
    {
        return string.IsNullOrEmpty(deathKey) ? fallbackKey : deathKey;
    }

    /// <summary>
    /// Gets the step sound key for this entity, or returns the fallback if not specified.
    /// </summary>
    /// <param name="fallbackKey">Default key to use if entity override is not specified.</param>
    /// <returns>The sound key to use for step/movement events.</returns>
    public string GetStepKey(string fallbackKey)
    {
        return string.IsNullOrEmpty(stepKey) ? fallbackKey : stepKey;
    }

    /// <summary>
    /// Checks if this entity has any sound overrides configured.
    /// </summary>
    /// <returns>True if at least one sound key is overridden.</returns>
    public bool HasAnyOverrides()
    {
        return !string.IsNullOrEmpty(takeDamageKey) || 
               !string.IsNullOrEmpty(deathKey) || 
               !string.IsNullOrEmpty(stepKey);
    }
}
