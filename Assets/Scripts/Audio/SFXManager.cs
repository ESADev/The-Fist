using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Centralized manager responsible for playing all sound effects in the game.
/// Uses a lightweight prefab-based system for playback and listens to global
/// events while providing a direct API for manual sound triggers.
/// </summary>
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
    /// Prefab used to play sound effects.
    /// </summary>
    [Header("Playback")]
    [Tooltip("Prefab used to play sound effects.")]
    [SerializeField] private SFXSource _sfxSourcePrefab;

    /// <summary>
    /// Dictionary for quick lookup of audio clips by key.
    /// </summary>
    private readonly Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();

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
        soundEffects.Clear();

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

            if (effect.clips == null)
            {
                Debug.LogWarning($"[SFXManager] Sound effect '{effect.key}' has no clip.", this);
                continue;
            }

            if (soundEffects.ContainsKey(effect.key))
            {
                Debug.LogWarning($"[SFXManager] Duplicate sound key '{effect.key}' ignored.", this);
                continue;
            }

            soundEffects.Add(effect.key, effect);
        }
    }

    /// <summary>
    /// Plays a sound effect corresponding to the provided key.
    /// </summary>
    /// <param name="key">Key of the sound effect to play.</param>
    public void PlaySound(string key, Vector3 position)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("[SFXManager] PlaySound called with empty key.", this);
            return;
        }

        if (!soundEffects.TryGetValue(key, out SoundEffect soundEffect))
        {
            Debug.LogWarning($"[SFXManager] Sound with key '{key}' not found.", this);
            return;
        }

        SFXClip randomSFXClip = soundEffect.clips[Random.Range(0, soundEffect.clips.Count)];
        float volume = randomSFXClip.UseRandomVolume ? randomSFXClip.volume * Random.Range(1f - randomSFXClip.RandomVolumeVariance, 1f - randomSFXClip.RandomVolumeVariance) : randomSFXClip.volume;
        float pitch = randomSFXClip.UseRandomPitch ? randomSFXClip.pitch * Random.Range(1f - randomSFXClip.RandomPitchVariance, 1f - randomSFXClip.RandomPitchVariance) : randomSFXClip.pitch;

        if (_sfxSourcePrefab == null)
        {
            Debug.LogWarning("[SFXManager] No SFXSource prefab assigned.", this);
            return;
        }

        SFXSource sourceInstance = Instantiate(_sfxSourcePrefab, position, Quaternion.identity);
        sourceInstance.Play(randomSFXClip.clip, volume, pitch);

        Debug.Log($"[SFXManager] Playing sound '{key}'.");
    }

    /// <summary>
    /// Handles the <see cref="GameEvents.OnUnitDamaged"/> event.
    /// </summary>
    /// <param name="damageInfo">Information about the damage event.</param>
    private void HandleUnitDamaged(DamageInfo damageInfo)
    {
        PlaySound(unitDamagedKey, damageInfo.victim.transform.position);
    }
}
