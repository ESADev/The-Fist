using UnityEngine;

/// <summary>
/// Facade manager that orchestrates all visual feedback in response to game events.
/// </summary>
public class VFXManager : MonoBehaviour
{
    /// <summary>
    /// Global singleton instance.
    /// </summary>
    public static VFXManager Instance { get; private set; }

    [Header("Specialist Managers")]
    [Tooltip("Handles particle effect spawning.")]
    public ParticleEffectManager particleManager;

    [Tooltip("Handles camera shake effects.")]
    public CameraShakeManager cameraShakeManager;

    [Tooltip("Handles global post-processing effects.")]
    public PostProcessingEffectManager postProcessingManager;

    [Header("Configuration")]
    [Tooltip("Particle effect key played when a unit is damaged.")]
    public string unitDamagedParticleKey = "unit_hit";

    [Tooltip("Damage threshold that triggers a heavy camera shake.")]
    public float heavyShakeThreshold = 50f;

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
    /// Handles the <see cref="GameEvents.OnUnitDamaged"/> event.
    /// Orchestrates particle effects, camera shake, and post-processing feedback.
    /// </summary>
    /// <param name="info">Information about the damage event.</param>
    private void HandleUnitDamaged(DamageInfo info)
    {
        Vector3 effectPosition = info.victim != null ? info.victim.transform.position : Vector3.zero;

        if (particleManager != null)
        {
            particleManager.PlayParticle(unitDamagedParticleKey, effectPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("[VFXManager] ParticleEffectManager reference not assigned.", this);
        }

        if (cameraShakeManager != null)
        {
            CameraShakeManager.ShakeIntensity intensity = info.damageAmount > heavyShakeThreshold
                ? CameraShakeManager.ShakeIntensity.Heavy
                : CameraShakeManager.ShakeIntensity.Light;
            cameraShakeManager.Shake(intensity);
        }
        else
        {
            Debug.LogError("[VFXManager] CameraShakeManager reference not assigned.", this);
        }

        if (postProcessingManager != null)
        {
            // Placeholder logic: toggle off low health effect after any hit.
            // In a real implementation this would check if the victim is the player and health is low.
            postProcessingManager.ToggleLowHealthEffect(false);
        }
        else
        {
            Debug.LogError("[VFXManager] PostProcessingEffectManager reference not assigned.", this);
        }
    }
}
