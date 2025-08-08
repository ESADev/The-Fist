using UnityEngine;

[System.Serializable]
public class SFXClip
{
    public AudioClip clip;
    public float volume = 1f;
    public float pitch = 1f;

    [Space]
    public bool UseRandomVolume = true;
    public float RandomVolumeVariance = 0.05f;

    [Space]
    public bool UseRandomPitch = true;
    public float RandomPitchVariance = 0.1f;
}
