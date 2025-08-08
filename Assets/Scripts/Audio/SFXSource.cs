using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXSource : MonoBehaviour
{
    [Header("Audio Configuration")]
    [Tooltip("AudioSource component used to play this sound.")]
    public AudioSource audioSource;

    private void Awake()
    {
        // If no audio source is assigned, try to get one from this GameObject
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Plays an audio clip with specified volume and pitch, then destroys this GameObject
    /// </summary>
    /// <param name="clip">The audio clip to play</param>
    /// <param name="volume">Volume level (0.0 to 1.0)</param>
    /// <param name="pitch">Pitch value (typically 0.1 to 3.0)</param>
    public void Play(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (audioSource == null)
        {
            Debug.LogWarning("SFXSource: No AudioSource component found!");
            Destroy(gameObject);
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("SFXSource: No AudioClip provided!");
            Destroy(gameObject);
            return;
        }

        // Set audio source properties
        audioSource.clip = clip;
        audioSource.volume = Mathf.Clamp01(volume);
        audioSource.pitch = pitch;

        // Play the sound
        audioSource.Play();

        // Destroy this GameObject after the clip finishes playing
        Destroy(gameObject, clip.length / Mathf.Abs(pitch));
    }
}
