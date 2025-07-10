using System;
using UnityEngine;

/// <summary>
/// Contains detailed information about a damage event.
/// </summary>
[Serializable]
public struct DamageInfo
{
    /// <summary>
    /// GameObject that caused the damage.
    /// </summary>
    public GameObject attacker;

    /// <summary>
    /// GameObject that received the damage.
    /// </summary>
    public GameObject victim;

    /// <summary>
    /// Final amount of damage dealt.
    /// </summary>
    public float damageAmount;

    /// <summary>
    /// Scriptable object describing the attack, if available.
    /// </summary>
    public AttackDefinitionSO attackData;

    /// <summary>
    /// Creates a new instance of DamageInfo.
    /// </summary>
    /// <param name="attacker">The GameObject that inflicted the damage.</param>
    /// <param name="victim">The GameObject that received the damage.</param>
    /// <param name="damageAmount">The final amount of damage dealt.</param>
    /// <param name="attackData">Optional attack data associated with the damage.</param>
    public DamageInfo(GameObject attacker, GameObject victim, float damageAmount, AttackDefinitionSO attackData)
    {
        this.attacker = attacker;
        this.victim = victim;
        this.damageAmount = damageAmount;
        this.attackData = attackData;
    }
}
