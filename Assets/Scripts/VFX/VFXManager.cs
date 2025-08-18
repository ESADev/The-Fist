using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Advanced facade manager that orchestrates all visual feedback using data-driven configurations.
/// Supports particle effects, post-processing effects, and camera shakes through VFXLibrary.
/// </summary>
public class VFXManager : MonoBehaviour
{
    /// <summary>
    /// Global singleton instance.
    /// </summary>
    public static VFXManager Instance { get; private set; }

    [Header("VFX Library")]
    [Tooltip("Comprehensive library containing all VFX configurations.")]
    public VFXLibrary vfxLibrary;

    [Header("Specialist Managers")]
    [Tooltip("Handles particle effect spawning using data configurations.")]
    public ParticleEffectManager particleManager;

    [Tooltip("Handles camera shake effects using data configurations.")]
    public CameraShakeManager cameraShakeManager;

    [Tooltip("Handles post-processing effects using data configurations.")]
    public PostProcessingEffectManager postProcessingManager;

    [Header("Event-Based VFX Configuration")]
    [Tooltip("VFX configurations triggered by specific game events.")]
    public VFXEventMapping[] eventMappings;

    [Header("Default Entity Event VFX Keys")]
    [Tooltip("Default VFX key used when a unit takes damage (overridden per-entity via EntityVFXOverrideSO).")]
    public string unitDamagedVFXKey = "unit_damage";
    [Tooltip("Default VFX key used when a unit dies (overridden per-entity via EntityVFXOverrideSO).")]
    public string unitDeathVFXKey = "unit_death";
    [Tooltip("Default VFX key used for footsteps if an override component wants to query (optional).")]
    public string unitStepVFXKey = "footstep";

    private void Awake()
    {
        // Singleton enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize the VFX library
        if (vfxLibrary != null)
        {
            vfxLibrary.Initialize();
        }
        else
        {
            Debug.LogError("[VFXManagerV2] VFXLibraryV2 is not assigned.", this);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnUnitDamaged += HandleUnitDamaged;
        GameEvents.OnUnitDied += HandleUnitDied;
        GameEvents.OnVictory += HandleVictory;
    GameEvents.OnDefeat += HandleDefeat;
    }

    private void OnDisable()
    {
        GameEvents.OnUnitDamaged -= HandleUnitDamaged;
        GameEvents.OnUnitDied -= HandleUnitDied;
        GameEvents.OnVictory -= HandleVictory;
    GameEvents.OnDefeat -= HandleDefeat;
    }

    #region Public VFX Trigger Methods

    /// <summary>
    /// Plays a particle effect by key.
    /// </summary>
    /// <param name="key">Key identifying the particle effect.</param>
    /// <param name="position">World position for the effect.</param>
    /// <param name="rotation">Rotation for the effect.</param>
    /// <param name="parent">Optional parent transform.</param>
    public GameObject PlayParticleEffect(string key, Vector3 position, Quaternion rotation = default, Transform parent = null)
    {
        if (particleManager != null)
        {
            return particleManager.PlayParticle(key, position, rotation, parent);
        }
        else
        {
            Debug.LogError("[VFXManager] ParticleEffectManager reference not assigned.", this);
            return null;
        }
    }

    /// <summary>
    /// Triggers a camera shake effect by key.
    /// </summary>
    /// <param name="key">Key identifying the camera shake effect.</param>
    public void TriggerCameraShake(string key)
    {
        if (cameraShakeManager != null)
        {
            cameraShakeManager.Shake(key);
        }
        else
        {
            Debug.LogError("[VFXManager] CameraShakeManager reference not assigned.", this);
        }
    }

    /// <summary>
    /// Triggers a camera shake effect with magnitude multiplier.
    /// </summary>
    /// <param name="key">Key identifying the camera shake effect.</param>
    /// <param name="magnitudeMultiplier">Multiplier for shake intensity.</param>
    public void TriggerCameraShakeWithMagnitude(string key, float magnitudeMultiplier)
    {
        if (cameraShakeManager != null)
        {
            cameraShakeManager.ShakeWithMagnitude(key, magnitudeMultiplier);
        }
        else
        {
            Debug.LogError("[VFXManager] CameraShakeManager reference not assigned.", this);
        }
    }

    /// <summary>
    /// Plays a post-processing effect by key.
    /// </summary>
    /// <param name="key">Key identifying the post-processing effect.</param>
    public void PlayPostProcessingEffect(string key)
    {
        if (postProcessingManager != null)
        {
            postProcessingManager.PlayPostProcessingEffect(key);
        }
        else
        {
            Debug.LogError("[VFXManager] PostProcessingEffectManager reference not assigned.", this);
        }
    }

    /// <summary>
    /// Stops a post-processing effect by key.
    /// </summary>
    /// <param name="key">Key identifying the post-processing effect to stop.</param>
    public void StopPostProcessingEffect(string key)
    {
        if (postProcessingManager != null)
        {
            postProcessingManager.StopPostProcessingEffect(key);
        }
        else
        {
            Debug.LogError("[VFXManager] PostProcessingEffectManager reference not assigned.", this);
        }
    }

    /// <summary>
    /// Plays multiple VFX effects simultaneously.
    /// </summary>
    /// <param name="keys">Array of VFX keys to trigger.</param>
    /// <param name="position">Position for particle effects.</param>
    /// <param name="rotation">Rotation for particle effects.</param>
    public void PlayMultipleEffects(List<string> keys, Vector3 position, Quaternion rotation = default)
    {
        foreach (string key in keys)
        {
            if (string.IsNullOrEmpty(key)) continue;

            PlayEffect(key, position);
        }
    }

    /// <summary>
    /// Plays a single VFX effect by key at a specified position.
    /// </summary>
    public void PlayEffect(string key, Vector3 position)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("[VFXManager] Attempted to play an empty effect key.", this);
            return;
        }

        // Try particle effect first
        var particleData = vfxLibrary?.GetParticleEffect(key);
        if (particleData != null)
        {
            PlayParticleEffect(key, position);
        }

        // Try post-processing effect
        var ppData = vfxLibrary?.GetPostProcessingEffect(key);
        if (ppData != null)
        {
            PlayPostProcessingEffect(key);
        }

        // Try camera shake
        var shakeData = vfxLibrary?.GetCameraShake(key);
        if (shakeData != null)
        {
            TriggerCameraShake(key);
            return;
        }

        Debug.LogWarning($"[VFXManager] No VFX found with key '{key}'.", this);
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles the defeat (lose level) event.
    /// </summary>
    private void HandleDefeat()
    {
        foreach (var mapping in eventMappings)
        {
            if (mapping.eventType == VFXEventType.Defeat)
            {
                ExecuteVFXMapping(mapping, Vector3.zero, null);
            }
        }
    }

    /// <summary>
    /// Handles the unit damaged event with comprehensive VFX response.
    /// Includes player-specific mapping if victim tagged as Player.
    /// </summary>
    private void HandleUnitDamaged(DamageInfo info)
    {
        Vector3 effectPosition = info.victim != null ? info.victim.transform.position : Vector3.zero;
        if (info.victim != null)
        {
            var entity = info.victim.GetComponent<Entity>();
            if (entity != null)
            {
                string key = GetEntityVFXKey(entity, "damage", unitDamagedVFXKey);
                if (!string.IsNullOrEmpty(key))
                {
                    PlayEffect(key, effectPosition);
                }
            }
        }

        // Find and execute event-specific VFX mappings
        foreach (var mapping in eventMappings)
        {
            if (mapping.eventType == VFXEventType.UnitDamaged && mapping.ShouldTrigger(info))
            {
                ExecuteVFXMapping(mapping, effectPosition, info);
            }
        }
    }

    /// <summary>
    /// Handles the unit died event.
    /// </summary>
    private void HandleUnitDied(GameObject unit)
    {
        Vector3 effectPosition = unit != null ? unit.transform.position : Vector3.zero;
        if (unit != null)
        {
            var entity = unit.GetComponent<Entity>();
            if (entity != null)
            {
                string key = GetEntityVFXKey(entity, "death", unitDeathVFXKey);
                if (!string.IsNullOrEmpty(key))
                {
                    PlayEffect(key, effectPosition);
                }
            }
        }

        foreach (var mapping in eventMappings)
        {
            if (mapping.eventType == VFXEventType.UnitDied)
            {
                ExecuteVFXMapping(mapping, effectPosition, null);
            }
        }
    }

    /// <summary>
    /// Handles the victory event.
    /// </summary>
    private void HandleVictory()
    {
        foreach (var mapping in eventMappings)
        {
            if (mapping.eventType == VFXEventType.Victory)
            {
                ExecuteVFXMapping(mapping, Vector3.zero, null);
            }
        }
    }

    /// <summary>
    /// Executes a VFX mapping configuration.
    /// </summary>
    private void ExecuteVFXMapping(VFXEventMapping mapping, Vector3 position, DamageInfo? damageInfo)
    {
        // Execute particle effects
        foreach (string key in mapping.particleEffectKeys)
        {
            if (!string.IsNullOrEmpty(key))
            {
                PlayParticleEffect(key, position);
            }
        }

        // Execute camera shakes
        foreach (var shakeConfig in mapping.cameraShakeConfigs)
        {
            if (!string.IsNullOrEmpty(shakeConfig.key))
            {
                if (shakeConfig.useMagnitudeMultiplier && damageInfo.HasValue)
                {
                    float multiplier = CalculateShakeMagnitudeMultiplier(damageInfo.Value, shakeConfig);
                    TriggerCameraShakeWithMagnitude(shakeConfig.key, multiplier);
                }
                else
                {
                    TriggerCameraShake(shakeConfig.key);
                }
            }
        }

        // Execute post-processing effects
        foreach (string key in mapping.postProcessingEffectKeys)
        {
            if (!string.IsNullOrEmpty(key))
            {
                PlayPostProcessingEffect(key);
            }
        }
    }

    /// <summary>
    /// Calculates shake magnitude multiplier based on damage info.
    /// </summary>
    private float CalculateShakeMagnitudeMultiplier(DamageInfo damageInfo, CameraShakeConfig config)
    {
        if (config.damageThreshold <= 0f) return 1f;

        float ratio = damageInfo.damageAmount / config.damageThreshold;
        return Mathf.Clamp(ratio * config.magnitudeMultiplier, config.minMultiplier, config.maxMultiplier);
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Gets the current status of the VFX system.
    /// </summary>
    /// <returns>VFX system status information.</returns>
    public VFXSystemStatus GetSystemStatus()
    {
        var status = new VFXSystemStatus();

        if (particleManager != null)
        {
            // Add particle system status if needed
        }

        if (cameraShakeManager != null)
        {
            status.activeCameraShakes = cameraShakeManager.GetActiveShakeCount();
            status.isCameraShaking = cameraShakeManager.IsShaking();
        }

        if (postProcessingManager != null)
        {
            status.activePostProcessingEffects = postProcessingManager.GetActiveEffectsStatus();
        }

        return status;
    }

    /// <summary>
    /// Stops all active VFX effects.
    /// </summary>
    public void StopAllEffects()
    {
        cameraShakeManager?.StopAllShakes();
        postProcessingManager?.StopAllEffects();
        
        Debug.Log($"[VFXManager] All VFX effects stopped.");
    }

    #region Entity Override Helpers
    /// <summary>
    /// Resolves an entity-specific VFX key for a given event type, falling back to defaults.
    /// </summary>
    private string GetEntityVFXKey(Entity entity, string eventType, string fallbackKey)
    {
        if (entity == null || entity.characterDefinition == null || entity.characterDefinition.vfxOverrides == null)
        {
            return fallbackKey;
        }
        var overrides = entity.characterDefinition.vfxOverrides;
        switch (eventType.ToLower())
        {
            case "damage": return overrides.GetDamageKey(fallbackKey);
            case "death": return overrides.GetDeathKey(fallbackKey);
            case "step": return overrides.GetStepKey(fallbackKey);
            default: return fallbackKey;
        }
    }
    #endregion

    #endregion
}

#region Supporting Data Structures

/// <summary>
/// Configuration for event-based VFX triggering.
/// </summary>
[System.Serializable]
public class VFXEventMapping
{
    [Header("Event Configuration")]
    public VFXEventType eventType;
    public string description;

    [Header("Trigger Conditions")]
    public float damageThreshold = 0f;
    public bool requiresPlayerVictim = false;

    [Header("VFX to Trigger")]
    public string[] particleEffectKeys;
    public CameraShakeConfig[] cameraShakeConfigs;
    public string[] postProcessingEffectKeys;

    /// <summary>
    /// Determines if this mapping should trigger based on damage info.
    /// </summary>
    public bool ShouldTrigger(DamageInfo damageInfo)
    {
        if (damageInfo.damageAmount < damageThreshold) return false;
        
        if (requiresPlayerVictim)
        {
            // Add player detection logic here if needed
            // For now, assume any victim can trigger
        }

        return true;
    }
}

/// <summary>
/// Configuration for camera shake with conditional parameters.
/// </summary>
[System.Serializable]
public class CameraShakeConfig
{
    public string key;
    public bool useMagnitudeMultiplier = false;
    public float damageThreshold = 50f;
    public float magnitudeMultiplier = 1f;
    public float minMultiplier = 0.1f;
    public float maxMultiplier = 3f;
}

/// <summary>
/// Types of events that can trigger VFX.
/// </summary>
public enum VFXEventType
{
    UnitDamaged,
    UnitDied,
    Victory,
    Defeat,
    Custom
}

/// <summary>
/// Status information for the VFX system.
/// </summary>
public class VFXSystemStatus
{
    public int activeCameraShakes;
    public bool isCameraShaking;
    public System.Collections.Generic.Dictionary<string, float> activePostProcessingEffects;
}

#endregion
