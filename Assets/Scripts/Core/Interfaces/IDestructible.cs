using UnityEngine;

/// <summary>
/// Defines behaviour for objects that can take damage and be destroyed.
/// </summary>
public interface IDestructible
{
    /// <summary>
    /// Applies damage to the object from a specified attacker.
    /// </summary>
    /// <param name="baseDamage">The raw damage amount before mitigation.</param>
    /// <param name="attacker">The GameObject that initiated the attack.</param>
    /// <param name="attackData">Data describing the type of attack used.</param>
    void TakeDamage(float baseDamage, GameObject attacker, AttackDefinitionSO attackData);
}
