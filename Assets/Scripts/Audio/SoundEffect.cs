using System.Collections.Generic;

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
    /// Overall volume multiplier for this sound effect.
    /// </summary>
    public float overallVolume = 1f;

    /// <summary>
    /// Audio clip to play for this effect.
    /// </summary>
    public List<SFXClip> clips;

    /// <summary>
    /// Whether to use 3D spatialization for this sound.
    /// If true, the sound will be positioned in 3D space.
    /// If false, it will be played as a 2D sound.
    /// </summary>
    public bool Use3DSound = true;
}
