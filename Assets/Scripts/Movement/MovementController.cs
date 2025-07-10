using UnityEngine;

/// <summary>
/// Provides a common API for moving characters using a pluggable <see cref="IMoveable"/> implementation.
/// </summary>
public class MovementController : MonoBehaviour
{
    private IMoveable mover;

    /// <summary>
    /// Current movement speed in units per second.
    /// </summary>
    public float CurrentSpeed { get; private set; }

    private void Awake()
    {
        mover = GetComponent<IMoveable>();
        if (mover == null)
        {
            Debug.LogError($"[MovementController] IMoveable component not found on {gameObject.name}", this);
            enabled = false;
        }
    }

    /// <summary>
    /// Initializes the underlying mover with the provided movement stats.
    /// </summary>
    /// <param name="stats">Movement configuration asset.</param>
    public void Initialize(MovementStatsSO stats)
    {
        if (mover == null)
        {
            mover = GetComponent<IMoveable>();
        }

        if (mover == null)
        {
            Debug.LogError($"[MovementController] No IMoveable component found on {gameObject.name}", this);
            return;
        }

        mover.Initialize(stats);
        CurrentSpeed = stats != null ? stats.moveSpeed : 0f;
    }

    /// <summary>
    /// Moves the character in a world-space direction.
    /// </summary>
    /// <param name="direction">Normalized direction vector.</param>
    public void MoveInDirection(Vector3 direction)
    {
        mover?.MoveInDirection(direction);
    }

    /// <summary>
    /// Moves the character toward a specific destination.
    /// </summary>
    /// <param name="destination">World space destination.</param>
    public void MoveTo(Vector3 destination)
    {
        mover?.MoveTo(destination);
    }

    /// <summary>
    /// Stops any ongoing movement.
    /// </summary>
    public void Stop()
    {
        mover?.Stop();
    }
}
