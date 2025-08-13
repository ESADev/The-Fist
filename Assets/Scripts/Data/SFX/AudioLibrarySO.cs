using System.Collections.Generic;
using UnityEngine;

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