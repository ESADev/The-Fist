using UnityEngine;

/// <summary>
/// ScriptableObject containing health related statistics for an entity.
/// </summary>
[CreateAssetMenu(fileName = "HealthStats", menuName = "TheFist/Health Stats")]
public class HealthStatsSO : ScriptableObject
{
    [Header("Health")]

    /// <summary>
    /// The maximum amount of health an entity can have.
    /// </summary>
    [Tooltip("The maximum amount of health an entity can have.")]
    public float maxHealth = 100f;

    /// <summary>
    /// Extra health that functions as armor and is depleted before health.
    /// </summary>
    [Tooltip("Extra health that functions as armor and is depleted before health.")]
    public float armor = 0f;

    [Header("Auto Revive / Regeneration")]
    [Tooltip("If enabled, the entity regenerates health after not taking damage for Revive Delay seconds.")]
    public bool autoReviveEnabled = true;

    [Tooltip("Seconds without taking damage before regeneration starts.")]
    public float reviveDelay = 4.2f;

    [Tooltip("Percent (0-1) of max health regenerated per second once revival starts. 0.025 = 2.5% per second.")]
    public float reviveSpeed = 0.025f; // 2.5% of Max HP per second
}
