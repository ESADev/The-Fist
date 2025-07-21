using UnityEngine;

/// <summary>
/// Defines a single attack used by an entity.
/// </summary>
public abstract class AttackDefinitionSO : ScriptableObject
{
    [Header("General")]

    /// <summary>
    /// Display name of the attack.
    /// </summary>
    [Tooltip("Display name of the attack.")]
    public string attackName = "New Attack";

    /// <summary>
    /// Type of attack represented by this definition.
    /// </summary>
    public abstract AttackType AttackType { get; }

    [Header("Stats")]

    /// <summary>
    /// Damage dealt by this attack.
    /// </summary>
    [Tooltip("Damage dealt by this attack.")]
    public float damage = 10f;

    /// <summary>
    /// Effective range of the attack.
    /// </summary>
    [Tooltip("Effective range of the attack.")]
    public float range = 1f;

    /// <summary>
    /// Cooldown time between consecutive uses in seconds.
    /// </summary>
    [Tooltip("Cooldown time between consecutive uses in seconds.")]
    public float cooldown = 1f;

    [Header("Animation")]

    /// <summary>
    /// Name of the animation clip to load from Resources folder and play directly.
    /// If not empty, this will override the animator trigger approach.
    /// </summary>
    [Tooltip("Name of the animation clip in Resources folder to play directly (without using animator states).")]
    public string animationClipName;

}
