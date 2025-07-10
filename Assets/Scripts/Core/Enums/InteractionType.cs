using System;
using UnityEngine;

/// <summary>
/// Describes the possible interactions that can be applied to a game object.
/// Multiple values can be combined using bitwise operations.
/// </summary>
[Flags]
public enum InteractionType
{
    /// <summary>
    /// No interaction available.
    /// </summary>
    None = 0,

    /// <summary>
    /// Object can be destroyed.
    /// </summary>
    Destructible = 1 << 0,

    /// <summary>
    /// Object can be upgraded.
    /// </summary>
    Upgradable = 1 << 1,

    /// <summary>
    /// Object can be unlocked.
    /// </summary>
    Unlockable = 1 << 2,

    /// <summary>
    /// Object can be collected.
    /// </summary>
    Collectible = 1 << 3
}
