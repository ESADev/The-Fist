using UnityEngine;

/// <summary>
/// Utility component that automatically destroys a GameObject when all particle systems finish playing.
/// Attach this to particle effect prefabs for automatic cleanup.
/// </summary>
public class AutoDestroyParticleEffect : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Maximum time to wait before forcing destruction (safety fallback).")]
    [Range(1f, 60f)]
    public float maxLifetime = 10f;

    [Tooltip("Whether to check for particle systems in child objects.")]
    public bool includeChildren = true;

    [Tooltip("Delay before starting the destruction check.")]
    [Range(0f, 5f)]
    public float initialDelay = 0f;

    /// <summary>
    /// Cached particle systems to monitor.
    /// </summary>
    private ParticleSystem[] particleSystems;

    /// <summary>
    /// Time when this component was enabled.
    /// </summary>
    private float startTime;

    /// <summary>
    /// Whether the destruction check has started.
    /// </summary>
    private bool checkStarted = false;

    private void Start()
    {
        startTime = Time.time;

        // Cache particle systems
        if (includeChildren)
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>();
        }
        else
        {
            particleSystems = GetComponents<ParticleSystem>();
        }

        if (particleSystems.Length == 0)
        {
            Debug.LogWarning("[AutoDestroyParticleEffect] No particle systems found. Destroying immediately.", this);
            Destroy(gameObject);
            return;
        }

        // Start all particle systems if they're not playing
        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps != null && !ps.isPlaying && ps.main.playOnAwake)
            {
                ps.Play();
            }
        }
    }

    private void Update()
    {
        // Wait for initial delay
        if (!checkStarted)
        {
            if (Time.time - startTime >= initialDelay)
            {
                checkStarted = true;
            }
            else
            {
                return;
            }
        }

        // Safety check: destroy after max lifetime
        if (Time.time - startTime >= maxLifetime)
        {
            Debug.Log($"[AutoDestroyParticleEffect] Max lifetime reached ({maxLifetime}s). Destroying {gameObject.name}.");
            Destroy(gameObject);
            return;
        }

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
            Debug.Log($"[AutoDestroyParticleEffect] All particle systems finished. Destroying {gameObject.name}.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Forces immediate destruction of the GameObject.
    /// </summary>
    public void ForceDestroy()
    {
        Debug.Log($"[AutoDestroyParticleEffect] Force destroying {gameObject.name}.");
        Destroy(gameObject);
    }

    /// <summary>
    /// Stops all particle systems and triggers destruction.
    /// </summary>
    public void StopAndDestroy()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps != null)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        Debug.Log($"[AutoDestroyParticleEffect] Stopped and destroying {gameObject.name}.");
        Destroy(gameObject);
    }

    /// <summary>
    /// Gets the current status of all particle systems.
    /// </summary>
    /// <returns>True if any particle system is still playing or has particles.</returns>
    public bool IsAnyParticleSystemActive()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps != null && (ps.isPlaying || ps.particleCount > 0))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Gets the total number of active particles across all systems.
    /// </summary>
    /// <returns>Sum of particle counts from all systems.</returns>
    public int GetTotalParticleCount()
    {
        int total = 0;
        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps != null)
            {
                total += ps.particleCount;
            }
        }
        return total;
    }
}
