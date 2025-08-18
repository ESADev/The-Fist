using UnityEditor.EditorTools;
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

    /// <summary>
    /// Key to use for playing sound effects associated with this attack.
    /// This should match the keys used in the AudioManager.
    /// </summary>
    [Tooltip("Key for sound effects associated with this attack (matches SFXManager keys).")]
    public string sfxKey;

    /// <summary>
    /// Key to use for playing visual effects associated with this attack.
    /// </summary>
    [Tooltip("Key for visual effects associated with this attack (matches VFXManager keys).")]
    public string vfxKey;

    [Header("Timing")]
    /// <summary>
    /// Seconds after the attack animation (windup) starts when the actual impact (damage / projectile spawn)
    /// should occur. 0 = immediate. This allows aligning gameplay impact with animation anticipation frames
    /// while keeping the system data-driven and decoupled from specific clips.
    /// </summary>
    [Tooltip("Seconds after attack start when impact (damage/projectile) is applied. 0 = immediate.")]
    public float impactDelay = 0f;
}
