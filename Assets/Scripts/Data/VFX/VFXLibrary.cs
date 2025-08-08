using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Comprehensive library asset holding all VFX data configurations.
/// Supports particle effects, post-processing effects, and camera shakes.
/// </summary>
[CreateAssetMenu(menuName = "VFX/VFX Library", fileName = "VFXLibrary")]
public class VFXLibrary : ScriptableObject
{
    [Header("Particle Effects")]
    [Tooltip("List of particle effect configurations.")]
    public List<ParticleEffectData> particleEffects = new List<ParticleEffectData>();

    [Header("Post-Processing Effects")]
    [Tooltip("List of post-processing effect configurations.")]
    public List<PostProcessingEffectData> postProcessingEffects = new List<PostProcessingEffectData>();

    [Header("Camera Shakes")]
    [Tooltip("List of camera shake configurations.")]
    public List<CameraShakeData> cameraShakes = new List<CameraShakeData>();

    /// <summary>
    /// Internal lookup dictionaries for fast access.
    /// </summary>
    private Dictionary<string, ParticleEffectData> _particleLookup;
    private Dictionary<string, PostProcessingEffectData> _postProcessingLookup;
    private Dictionary<string, CameraShakeData> _cameraShakeLookup;

    /// <summary>
    /// Initializes the lookup dictionaries. Call this during Awake or Start.
    /// </summary>
    public void Initialize()
    {
        BuildParticleLookup();
        BuildPostProcessingLookup();
        BuildCameraShakeLookup();
    }

    #region Particle Effects

    /// <summary>
    /// Gets particle effect data by key.
    /// </summary>
    /// <param name="key">Unique identifier for the particle effect.</param>
    /// <returns>Particle effect data, or null if not found.</returns>
    public ParticleEffectData GetParticleEffect(string key)
    {
        if (_particleLookup == null)
            BuildParticleLookup();

        _particleLookup.TryGetValue(key, out ParticleEffectData effect);
        return effect;
    }

    private void BuildParticleLookup()
    {
        _particleLookup = new Dictionary<string, ParticleEffectData>();

        foreach (var effect in particleEffects)
        {
            if (effect == null || !effect.IsValid())
            {
                continue;
            }

            if (_particleLookup.ContainsKey(effect.key))
            {
                Debug.LogWarning($"[VFXLibrary] Duplicate particle effect key '{effect.key}' ignored.", this);
                continue;
            }

            _particleLookup.Add(effect.key, effect);
        }
    }

    #endregion

    #region Post-Processing Effects

    /// <summary>
    /// Gets post-processing effect data by key.
    /// </summary>
    /// <param name="key">Unique identifier for the post-processing effect.</param>
    /// <returns>Post-processing effect data, or null if not found.</returns>
    public PostProcessingEffectData GetPostProcessingEffect(string key)
    {
        if (_postProcessingLookup == null)
            BuildPostProcessingLookup();

        _postProcessingLookup.TryGetValue(key, out PostProcessingEffectData effect);
        return effect;
    }

    private void BuildPostProcessingLookup()
    {
        _postProcessingLookup = new Dictionary<string, PostProcessingEffectData>();

        foreach (var effect in postProcessingEffects)
        {
            if (effect == null || !effect.IsValid())
            {
                continue;
            }

            if (_postProcessingLookup.ContainsKey(effect.key))
            {
                Debug.LogWarning($"[VFXLibrary] Duplicate post-processing effect key '{effect.key}' ignored.", this);
                continue;
            }

            _postProcessingLookup.Add(effect.key, effect);
        }
    }

    #endregion

    #region Camera Shakes

    /// <summary>
    /// Gets camera shake data by key.
    /// </summary>
    /// <param name="key">Unique identifier for the camera shake.</param>
    /// <returns>Camera shake data, or null if not found.</returns>
    public CameraShakeData GetCameraShake(string key)
    {
        if (_cameraShakeLookup == null)
            BuildCameraShakeLookup();

        _cameraShakeLookup.TryGetValue(key, out CameraShakeData shake);
        return shake;
    }

    private void BuildCameraShakeLookup()
    {
        _cameraShakeLookup = new Dictionary<string, CameraShakeData>();

        foreach (var shake in cameraShakes)
        {
            if (shake == null || !shake.IsValid())
            {
                continue;
            }

            if (_cameraShakeLookup.ContainsKey(shake.key))
            {
                Debug.LogWarning($"[VFXLibrary] Duplicate camera shake key '{shake.key}' ignored.", this);
                continue;
            }

            _cameraShakeLookup.Add(shake.key, shake);
        }
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Gets all available VFX keys organized by type.
    /// </summary>
    /// <returns>Dictionary mapping VFX types to their available keys.</returns>
    public Dictionary<VFXType, List<string>> GetAllVFXKeys()
    {
        var result = new Dictionary<VFXType, List<string>>();

        // Particle effects
        var particleKeys = new List<string>();
        foreach (var effect in particleEffects)
        {
            if (effect != null && effect.IsValid())
                particleKeys.Add(effect.key);
        }
        result[VFXType.ParticleEffect] = particleKeys;

        // Post-processing effects
        var ppKeys = new List<string>();
        foreach (var effect in postProcessingEffects)
        {
            if (effect != null && effect.IsValid())
                ppKeys.Add(effect.key);
        }
        result[VFXType.PostProcessingEffect] = ppKeys;

        // Camera shakes
        var shakeKeys = new List<string>();
        foreach (var shake in cameraShakes)
        {
            if (shake != null && shake.IsValid())
                shakeKeys.Add(shake.key);
        }
        result[VFXType.CameraShake] = shakeKeys;

        return result;
    }

    /// <summary>
    /// Validates all VFX configurations in the library.
    /// </summary>
    /// <returns>True if all configurations are valid, false otherwise.</returns>
    public bool ValidateLibrary()
    {
        bool isValid = true;

        foreach (var effect in particleEffects)
        {
            if (effect == null || !effect.IsValid())
            {
                Debug.LogError($"[VFXLibrary] Invalid particle effect configuration.", this);
                isValid = false;
            }
        }

        foreach (var effect in postProcessingEffects)
        {
            if (effect == null || !effect.IsValid())
            {
                Debug.LogError($"[VFXLibrary] Invalid post-processing effect configuration.", this);
                isValid = false;
            }
        }

        foreach (var shake in cameraShakes)
        {
            if (shake == null || !shake.IsValid())
            {
                Debug.LogError($"[VFXLibrary] Invalid camera shake configuration.", this);
                isValid = false;
            }
        }

        return isValid;
    }

    #endregion
}

