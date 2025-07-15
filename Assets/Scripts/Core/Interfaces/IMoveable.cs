using UnityEngine;

/// <summary>
/// Provides common movement functionality for any object that can move.
/// </summary>
public interface IMoveable
{
    /// <summary>
    /// Initializes the moveable object with movement statistics.
    /// </summary>
    /// <param name="stats">The stats asset that defines movement behavior.</param>
    void Initialize(MovementStatsSO stats);

    /// <summary>
    /// Moves the object in a world-space direction.
    /// </summary>
    /// <param name="direction">Normalized direction vector.</param>
    void MoveInDirection(Vector3 direction);

    /// <summary>
    /// Moves the object toward a specific destination in world space.
    /// </summary>
    /// <param name="destination">Destination position.</param>
    void MoveTo(Vector3 destination);

    /// <summary>
    /// Moves the object toward a specific destination using a Transform reference.
    /// </summary>
    /// <param name="destinationTransform">Destination transform.</param>
    void MoveTo(Transform destinationTransform);

    /// <summary>
    /// Sets the current movement speed.
    /// </summary>
    /// <param name="newSpeed">The new speed value.</param>
    void SetSpeed(float newSpeed);

    /// <summary>
    /// Immediately stops any movement.
    /// </summary>
    void Stop();

    /// <summary>
    /// Immediately continues movement towards a destination.
    /// </summary>
    void Continue();

    /// <summary>
    /// Sets the stopping distance for the movement.
    /// </summary>
    /// <param name="stoppingDistance">The stopping distance value.</param>
    void SetStoppingDistance(float stoppingDistance);
}
