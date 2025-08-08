using System.Collections;
using UnityEngine;

/// <summary>
/// Handles spawning and management of particle effects using VFXLibrary data.
/// </summary>
public class ParticleEffectManager : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Library asset containing all VFX configurations.")]
    public VFXLibrary vfxLibrary;

    private void Awake()
    {
        if (vfxLibrary == null)
        {
            Debug.LogError("[ParticleEffectManager] VFXLibrary is not assigned.", this);
            return;
        }

        vfxLibrary.Initialize();
    }

    /// <summary>
    /// Spawns a particle effect using data-driven configuration.
    /// </summary>
    /// <param name="key">Key identifying which effect to spawn.</param>
    /// <param name="position">World position for the effect.</param>
    /// <param name="rotation">Rotation for the spawned effect.</param>
    /// <param name="parent">Optional parent transform.</param>
    /// <returns>The instantiated effect GameObject, or null if spawn failed.</returns>
    public GameObject PlayParticle(string key, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("[ParticleEffectManager] PlayParticle called with empty key.", this);
            return null;
        }

        ParticleEffectData effectData = vfxLibrary.GetParticleEffect(key);
        if (effectData == null)
        {
            Debug.LogWarning($"[ParticleEffectManager] Particle effect with key '{key}' not found.", this);
            return null;
        }

        return SpawnParticleEffect(effectData, position, rotation, parent);
    }

    /// <summary>
    /// Spawns a particle effect with custom scale override.
    /// </summary>
    /// <param name="key">Key identifying which effect to spawn.</param>
    /// <param name="position">World position for the effect.</param>
    /// <param name="rotation">Rotation for the spawned effect.</param>
    /// <param name="scale">Scale multiplier override.</param>
    /// <param name="parent">Optional parent transform.</param>
    /// <returns>The instantiated effect GameObject, or null if spawn failed.</returns>
    public GameObject PlayParticleWithScale(string key, Vector3 position, Quaternion rotation, float scale, Transform parent = null)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("[ParticleEffectManager] PlayParticleWithScale called with empty key.", this);
            return null;
        }

        ParticleEffectData effectData = vfxLibrary.GetParticleEffect(key);
        if (effectData == null)
        {
            Debug.LogWarning($"[ParticleEffectManager] Particle effect with key '{key}' not found.", this);
            return null;
        }

        // Create a temporary copy with modified scale
        var modifiedData = new ParticleEffectData();
        modifiedData.key = effectData.key;
        modifiedData.particlePrefab = effectData.particlePrefab;
        modifiedData.autoDestroy = effectData.autoDestroy;
        modifiedData.maxLifetime = effectData.maxLifetime;
        modifiedData.playOnAwake = effectData.playOnAwake;
        modifiedData.scale = scale;

        return SpawnParticleEffect(modifiedData, position, rotation, parent);
    }

    /// <summary>
    /// Internal method to spawn a particle effect from data configuration.
    /// </summary>
    private GameObject SpawnParticleEffect(ParticleEffectData data, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject instance = Instantiate(data.particlePrefab, position, rotation, parent);
        
        // Apply scale
        if (data.scale != 1f)
        {
            instance.transform.localScale *= data.scale;
        }

        // Configure particle systems
        ParticleSystem[] particleSystems = instance.GetComponentsInChildren<ParticleSystem>();
        
        foreach (ParticleSystem ps in particleSystems)
        {
            if (!data.playOnAwake)
            {
                ps.Stop();
            }
            else if (!ps.isPlaying)
            {
                ps.Play();
            }
        }

        // Handle auto-destruction
        if (data.autoDestroy)
        {
            StartCoroutine(HandleAutoDestroy(instance, data, particleSystems));
        }

        Debug.Log($"[ParticleEffectManager] Spawned particle effect '{data.key}' at {position}.");
        return instance;
    }

    /// <summary>
    /// Coroutine to handle automatic destruction of particle effects.
    /// </summary>
    private IEnumerator HandleAutoDestroy(GameObject instance, ParticleEffectData data, ParticleSystem[] particleSystems)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < data.maxLifetime)
        {
            // Check if all particle systems have finished
            bool allFinished = true;
            foreach (ParticleSystem ps in particleSystems)
            {
                if (ps != null && (ps.isPlaying || ps.particleCount > 0))
                {
                    allFinished = false;
                    break;
                }
            }

            if (allFinished)
            {
                break;
            }

            yield return null;
            elapsedTime += Time.deltaTime;
        }

        if (instance != null)
        {
            Destroy(instance);
        }
    }

    /// <summary>
    /// Stops all particle effects with the specified key.
    /// </summary>
    /// <param name="key">Key of the particle effect to stop.</param>
    public void StopParticleEffect(string key)
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains(key))
            {
                ParticleSystem[] particleSystems = obj.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem ps in particleSystems)
                {
                    ps.Stop();
                }
            }
        }
    }
}
