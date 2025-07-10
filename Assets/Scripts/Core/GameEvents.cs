using System;
using UnityEngine;

/// <summary>
/// Central hub for global game events used to decouple communication
/// between independent systems.
/// </summary>
public static class GameEvents
{
    /// <summary>
    /// Fired when an attack is performed.
    /// Parameters: the attack definition and the GameObject performing the attack.
    /// </summary>
    public static event Action<AttackDefinitionSO, GameObject> OnAttackPerformed;

    /// <summary>
    /// Fired when an entity's health value changes.
    /// Parameters: new health value and the affected GameObject.
    /// </summary>
    public static event Action<float, GameObject> OnHealthChanged;

    /// <summary>
    /// Invokes the <see cref="OnAttackPerformed"/> event.
    /// </summary>
    /// <param name="attack">The attack data used.</param>
    /// <param name="attacker">The GameObject performing the attack.</param>
    public static void RaiseAttackPerformed(AttackDefinitionSO attack, GameObject attacker)
    {
        if (attack == null)
        {
            Debug.LogError("[GameEvents] RaiseAttackPerformed called with null attack definition.");
            return;
        }

        if (attacker == null)
        {
            Debug.LogError("[GameEvents] RaiseAttackPerformed called with null attacker.");
            return;
        }

        Debug.Log($"[GameEvents] Attack performed: {attack.attackName} by {attacker.name}");
        OnAttackPerformed?.Invoke(attack, attacker);
    }

    /// <summary>
    /// Invokes the <see cref="OnHealthChanged"/> event.
    /// </summary>
    /// <param name="newHealth">Current health value.</param>
    /// <param name="target">The GameObject whose health changed.</param>
    public static void RaiseHealthChanged(float newHealth, GameObject target)
    {
        if (target == null)
        {
            Debug.LogError("[GameEvents] RaiseHealthChanged called with null target.");
            return;
        }

        Debug.Log($"[GameEvents] Health changed for {target.name}: {newHealth}");
        OnHealthChanged?.Invoke(newHealth, target);
    }
}
