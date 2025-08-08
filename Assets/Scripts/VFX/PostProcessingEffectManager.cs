using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Handles post-processing effects using data-driven configurations from VFXLibrary.
/// Manages timing, blending, and priority of multiple concurrent effects.
/// </summary>
public class PostProcessingEffectManager : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Library asset containing all VFX configurations.")]
    public VFXLibrary vfxLibrary;

    [Tooltip("Global volume component for post-processing effects.")]
    public Volume globalVolume;

    /// <summary>
    /// Tracks active post-processing effects.
    /// </summary>
    private readonly Dictionary<string, ActivePostProcessingEffect> activeEffects = new Dictionary<string, ActivePostProcessingEffect>();

    /// <summary>
    /// Coroutines managing effect timelines.
    /// </summary>
    private readonly Dictionary<string, Coroutine> effectCoroutines = new Dictionary<string, Coroutine>();

    private void Awake()
    {
        if (vfxLibrary == null)
        {
            Debug.LogError("[PostProcessingEffectManager] VFXLibrary is not assigned.", this);
            return;
        }

        if (globalVolume == null)
        {
            Debug.LogError("[PostProcessingEffectManager] Global Volume is not assigned.", this);
            return;
        }

        vfxLibrary.Initialize();
    }

    /// <summary>
    /// Plays a post-processing effect by key.
    /// </summary>
    /// <param name="key">Unique identifier for the post-processing effect.</param>
    public void PlayPostProcessingEffect(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("[PostProcessingEffectManager] PlayPostProcessingEffect called with empty key.", this);
            return;
        }

        PostProcessingEffectData effectData = vfxLibrary.GetPostProcessingEffect(key);
        if (effectData == null)
        {
            Debug.LogWarning($"[PostProcessingEffectManager] Post-processing effect with key '{key}' not found.", this);
            return;
        }

        // Stop existing effect with the same key
        StopPostProcessingEffect(key);

        // Create and start new effect
        var activeEffect = new ActivePostProcessingEffect
        {
            data = effectData,
            volume = CreateEffectVolume(effectData),
            startTime = Time.time
        };

        activeEffects[key] = activeEffect;
        effectCoroutines[key] = StartCoroutine(ManageEffectTimeline(key, activeEffect));

        Debug.Log($"[PostProcessingEffectManager] Started post-processing effect '{key}'.");
    }

    /// <summary>
    /// Stops a post-processing effect by key.
    /// </summary>
    /// <param name="key">Unique identifier for the post-processing effect to stop.</param>
    public void StopPostProcessingEffect(string key)
    {
        if (activeEffects.TryGetValue(key, out ActivePostProcessingEffect effect))
        {
            // Stop the coroutine
            if (effectCoroutines.TryGetValue(key, out Coroutine coroutine))
            {
                StopCoroutine(coroutine);
                effectCoroutines.Remove(key);
            }

            // Destroy the volume
            if (effect.volume != null)
            {
                DestroyImmediate(effect.volume.gameObject);
            }

            activeEffects.Remove(key);
            Debug.Log($"[PostProcessingEffectManager] Stopped post-processing effect '{key}'.");
        }
    }

    /// <summary>
    /// Stops all active post-processing effects.
    /// </summary>
    public void StopAllEffects()
    {
        var keysToStop = new List<string>(activeEffects.Keys);
        foreach (string key in keysToStop)
        {
            StopPostProcessingEffect(key);
        }
    }

    /// <summary>
    /// Creates a volume component for the post-processing effect.
    /// </summary>
    private Volume CreateEffectVolume(PostProcessingEffectData data)
    {
        GameObject volumeObject = new GameObject($"PP_Effect_{data.key}");
        volumeObject.transform.SetParent(transform);

        Volume volume = volumeObject.AddComponent<Volume>();
        volume.profile = data.postProcessingProfile;
        volume.priority = data.priority;
        volume.weight = 0f; // Start with zero weight

        return volume;
    }

    /// <summary>
    /// Manages the timeline of a post-processing effect (fade-in, main, fade-out).
    /// </summary>
    private IEnumerator ManageEffectTimeline(string key, ActivePostProcessingEffect effect)
    {
        PostProcessingEffectData data = effect.data;
        Volume volume = effect.volume;

        // Fade-in phase
        if (data.fadeInDuration > 0f)
        {
            float fadeInTime = 0f;
            while (fadeInTime < data.fadeInDuration)
            {
                float progress = fadeInTime / data.fadeInDuration;
                volume.weight = Mathf.Lerp(0f, data.maxWeight, progress);
                
                fadeInTime += Time.deltaTime;
                yield return null;
            }
        }

        // Set to full intensity
        volume.weight = data.maxWeight;

        // Main duration phase
        if (data.mainDuration > 0f)
        {
            yield return new WaitForSeconds(data.mainDuration);
        }

        // Fade-out phase
        if (data.fadeOutDuration > 0f)
        {
            float fadeOutTime = 0f;
            while (fadeOutTime < data.fadeOutDuration)
            {
                float progress = fadeOutTime / data.fadeOutDuration;
                volume.weight = Mathf.Lerp(data.maxWeight, 0f, progress);
                
                fadeOutTime += Time.deltaTime;
                yield return null;
            }
        }

        // Effect completed, clean up
        StopPostProcessingEffect(key);
    }

    /// <summary>
    /// Gets information about currently active post-processing effects.
    /// </summary>
    /// <returns>Dictionary of active effects with their current status.</returns>
    public Dictionary<string, float> GetActiveEffectsStatus()
    {
        var status = new Dictionary<string, float>();

        foreach (var kvp in activeEffects)
        {
            float elapsed = Time.time - kvp.Value.startTime;
            float totalDuration = kvp.Value.data.TotalDuration;
            float progress = totalDuration > 0f ? elapsed / totalDuration : 1f;
            
            status[kvp.Key] = Mathf.Clamp01(progress);
        }

        return status;
    }

    /// <summary>
    /// Checks if a specific post-processing effect is currently active.
    /// </summary>
    /// <param name="key">Key of the effect to check.</param>
    /// <returns>True if the effect is active, false otherwise.</returns>
    public bool IsEffectActive(string key)
    {
        return activeEffects.ContainsKey(key);
    }

    private void OnDestroy()
    {
        StopAllEffects();
    }
}

/// <summary>
/// Internal structure to track active post-processing effects.
/// </summary>
internal class ActivePostProcessingEffect
{
    public PostProcessingEffectData data;
    public Volume volume;
    public float startTime;
}
