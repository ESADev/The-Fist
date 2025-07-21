using UnityEngine;

/// <summary>
/// Defines a ranged attack that spawns a projectile.
/// </summary>
[CreateAssetMenu(fileName = "RangedAttackDefinition", menuName = "TheFist/Attack Definition/Ranged")]
public class RangedAttackDefinitionSO : AttackDefinitionSO
{
    [Header("Projectiles")]
    [Tooltip("Prefab spawned when executing a ranged attack.")]
    public GameObject projectilePrefab;

    [Tooltip("Movement speed of projectiles spawned by this attack.")]
    public float projectileSpeed = 20f;

    /// <inheritdoc/>
    public override AttackType AttackType => AttackType.Ranged;
}
