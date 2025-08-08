using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializable data container mapping keys to audio clips.
/// </summary>
[System.Serializable]
public class SoundEffect
{
    /// <summary>
    /// Unique identifier used to reference this sound.
    /// </summary>
    public string key;

    /// <summary>
    /// Audio clip to play for this effect.
    /// </summary>
    public AudioClip clip;
}

/// <summary>
/// Library asset holding references to all sound effects used by the game.
/// </summary>
[CreateAssetMenu(menuName = "Audio/Audio Library", fileName = "AudioLibrarySO")]
public class AudioLibrarySO : ScriptableObject
{
    [Header("Sound Effects")]
    [Tooltip("List of sound effects keyed by unique identifiers.")]
    public List<SoundEffect> soundEffects = new List<SoundEffect>();
}
