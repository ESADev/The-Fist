using UnityEngine;

/// <summary>
/// Represents the life state of an <see cref="Entity"/>.
/// </summary>
public enum EntityState
{
    /// <summary>
    /// The entity is alive and functional.
    /// </summary>
    Active,

    /// <summary>
    /// The entity has died and should no longer act.
    /// </summary>
    Dead
}

