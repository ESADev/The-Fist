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
    public List<SFXClip> clips;
}
