using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

/// <summary>
/// Centralized manager responsible for playing all sound effects in the game.
/// Integrates with the More Mountains FEEL package for playback and
/// listens to global events while providing a direct API for manual sound triggers.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    /// <summary>
    /// Global singleton instance.
    /// </summary>
    public static SFXManager Instance { get; private set; }

    [Header("Configuration")]
    [Tooltip("Library asset containing all sound effects.")]
    public AudioLibrarySO audioLibrary;

    [Header("Event Keys")]
    [Tooltip("Sound key to play when a unit is damaged.")]
    public string unitDamagedKey = "unit_hit";

    /// <summary>
    /// Cached audio source component.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Dictionary for quick lookup of audio clips by key.
    /// </summary>
    private readonly Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

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

        audioSource = GetComponent<AudioSource>();
        if (audioLibrary == null)
        {
            Debug.LogError("[SFXManager] AudioLibrarySO is not assigned.", this);
            return;
        }

        BuildLookup();
    }

    private void OnEnable()
    {
        GameEvents.OnUnitDamaged += HandleUnitDamaged;
    }

    private void OnDisable()
    {
        GameEvents.OnUnitDamaged -= HandleUnitDamaged;
    }

    /// <summary>
    /// Builds the internal lookup dictionary from the assigned <see cref="audioLibrary"/>.
    /// </summary>
    private void BuildLookup()
    {
        clips.Clear();

        foreach (SoundEffect effect in audioLibrary.soundEffects)
        {
            if (effect == null)
            {
                continue;
            }

            if (string.IsNullOrEmpty(effect.key))
            {
                Debug.LogWarning("[SFXManager] Encountered sound effect with empty key.", this);
                continue;
            }

            if (effect.clip == null)
            {
                Debug.LogWarning($"[SFXManager] Sound effect '{effect.key}' has no clip.", this);
                continue;
            }

            if (clips.ContainsKey(effect.key))
            {
                Debug.LogWarning($"[SFXManager] Duplicate sound key '{effect.key}' ignored.", this);
                continue;
            }

            clips.Add(effect.key, effect.clip);
        }
    }

    /// <summary>
    /// Plays a sound effect corresponding to the provided key.
    /// </summary>
    /// <param name="key">Key of the sound effect to play.</param>
    public void PlaySound(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("[SFXManager] PlaySound called with empty key.", this);
            return;
        }

        if (!clips.TryGetValue(key, out AudioClip clip))
        {
            Debug.LogWarning($"[SFXManager] Sound with key '{key}' not found.", this);
            return;
        }

        MMSoundManagerSoundPlayEvent.Trigger(
            clip,
            MMSoundManager.MMSoundManagerTracks.Sfx,
            transform.position);

        Debug.Log($"[SFXManager] Playing sound '{key}' via FEEL.");
    }

    /// <summary>
    /// Handles the <see cref="GameEvents.OnUnitDamaged"/> event.
    /// </summary>
    /// <param name="damageInfo">Information about the damage event.</param>
    private void HandleUnitDamaged(DamageInfo damageInfo)
    {
        PlaySound(unitDamagedKey);
    }
}
