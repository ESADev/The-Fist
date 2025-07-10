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
}
