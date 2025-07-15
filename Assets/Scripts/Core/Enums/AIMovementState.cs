using UnityEngine;

/// <summary>
/// Represents high level movement states for AI controlled entities.
/// </summary>
public enum AIMovementState
{
    /// <summary>
    /// Moving toward a long range strategic target.
    /// </summary>
    MovingStrategic,

    /// <summary>
    /// Focusing on a tactical target announced by an <see cref="Interactor"/>.
    /// </summary>
    EngagingTactical
}
