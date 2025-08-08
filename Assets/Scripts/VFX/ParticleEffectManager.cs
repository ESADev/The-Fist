using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles spawning of particle effect prefabs based on keys.
/// </summary>
public class ParticleEffectManager : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Library asset containing all visual effects.")]
    public VFXLibrarySO vfxLibrary;

    /// <summary>
    /// Lookup dictionary for effect prefabs.
    /// </summary>
    private readonly Dictionary<string, GameObject> effects = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (vfxLibrary == null)
        {
            Debug.LogError("[ParticleEffectManager] VFXLibrarySO is not assigned.", this);
            return;
        }

        BuildLookup();
    }

    /// <summary>
    /// Builds the internal lookup dictionary from the assigned <see cref="vfxLibrary"/>.
    /// </summary>
    private void BuildLookup()
    {
        effects.Clear();

        foreach (VisualEffect effect in vfxLibrary.effects)
        {
            if (effect == null)
            {
                continue;
            }

            if (string.IsNullOrEmpty(effect.key))
            {
                Debug.LogWarning("[ParticleEffectManager] Encountered effect with empty key.", this);
                continue;
            }

            if (effect.effectPrefab == null)
            {
                Debug.LogWarning($"[ParticleEffectManager] Effect '{effect.key}' has no prefab.", this);
                continue;
            }

            if (effects.ContainsKey(effect.key))
            {
                Debug.LogWarning($"[ParticleEffectManager] Duplicate effect key '{effect.key}' ignored.", this);
                continue;
            }

            effects.Add(effect.key, effect.effectPrefab);
        }
    }

    /// <summary>
    /// Spawns a particle effect at the specified position and rotation.
    /// </summary>
    /// <param name="key">Key identifying which effect to spawn.</param>
    /// <param name="position">World position for the effect.</param>
    /// <param name="rotation">Rotation for the spawned effect.</param>
    public void PlayParticle(string key, Vector3 position, Quaternion rotation)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("[ParticleEffectManager] PlayParticle called with empty key.", this);
            return;
        }

        if (!effects.TryGetValue(key, out GameObject prefab))
        {
            Debug.LogWarning($"[ParticleEffectManager] Effect with key '{key}' not found.", this);
            return;
        }

        Instantiate(prefab, position, rotation);
        Debug.Log($"[ParticleEffectManager] Spawned particle effect '{key}'.");
    }
}
