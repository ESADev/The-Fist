using UnityEngine;

/// <summary>
/// Defines a single attack used by an entity.
/// </summary>
[CreateAssetMenu(fileName = "AttackDefinition", menuName = "TheFist/Attack Definition")]
public class AttackDefinitionSO : ScriptableObject
{
    [Header("General")]

    /// <summary>
    /// Display name of the attack.
    /// </summary>
    [Tooltip("Display name of the attack.")]
    public string attackName = "New Attack";

    /// <summary>
    /// Classification of the attack behaviour.
    /// </summary>
    [Tooltip("Classification of the attack behaviour.")]
    public AttackType attackType = AttackType.Melee;

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
    /// Name of the animator trigger used when this attack is performed.
    /// </summary>
    [Tooltip("Name of the animator trigger used when this attack is performed.")]
    public string animationTriggerName = "Attack";

    /// <summary>
    /// Name of the animation clip to load from Resources folder and play directly.
    /// If not empty, this will override the animator trigger approach.
    /// </summary>
    [Tooltip("Name of the animation clip in Resources folder to play directly (without using animator states).")]
    public string animationClipName;

    [Header("Projectiles")]

    /// <summary>
    /// Prefab spawned when executing a ranged attack. Null for melee attacks.
    /// </summary>
    [Tooltip("Prefab spawned when executing a ranged attack. Null for melee attacks.")]
    public GameObject projectilePrefab;

    /// <summary>
    /// Movement speed for projectiles spawned by this attack.
    /// Only used when <see cref="projectilePrefab"/> is assigned.
    /// </summary>
    [Tooltip("Movement speed of projectiles spawned by this attack.")]
    public float projectileSpeed = 10f;
}
