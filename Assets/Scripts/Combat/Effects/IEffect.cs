using UnityEngine;

/// <summary>
/// Interface for all effect components. An effect performs an action on a target.
/// Keeping effects generic allows for high reusability.
/// </summary>
public interface IEffect
{
    /// <summary>
    /// Apply the effect to the provided target GameObject.
    /// </summary>
    /// <param name="target">The GameObject to affect.</param>
    void Apply(GameObject target);
}
