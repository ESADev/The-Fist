using UnityEngine;

/// <summary>
/// Defines a melee attack with no projectile data.
/// </summary>
[CreateAssetMenu(fileName = "MeleeAttackDefinition", menuName = "TheFist/Attack Definition/Melee")]
public class MeleeAttackDefinitionSO : AttackDefinitionSO
{
    /// <inheritdoc/>
    public override AttackType AttackType => AttackType.Melee;
}
