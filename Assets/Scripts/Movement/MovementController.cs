using UnityEngine;

/// <summary>
/// Provides a common API for moving characters using a pluggable <see cref="IMoveable"/> implementation.
/// </summary>
[RequireComponent(typeof(Entity))]
public class MovementController : MonoBehaviour
{
    private IMoveable mover;
    private Entity entity;

    /// <summary>
    /// Current movement speed in units per second.
    /// </summary>
    public float CurrentSpeed { get; private set; }

    private void Awake()
    {
        mover = GetComponent<IMoveable>();
        entity = GetComponent<Entity>();

        if (mover == null)
        {
            Debug.LogError($"[MovementController] IMoveable component not found on {gameObject.name}", this);
            enabled = false;
        }

        if (entity == null)
        {
            Debug.LogError($"[MovementController] Entity component missing on {gameObject.name}", this);
            enabled = false;
        }
    }

    /// <summary>
    /// Initializes the underlying mover with the provided movement stats.
    /// </summary>
    /// <param name="stats">Movement configuration asset.</param>
    public void Initialize(MovementStatsSO stats)
    {
        if (stats == null)
        {
            Debug.LogError($"[MovementController] Movement stats are null for {gameObject.name}", this);
            enabled = false;
            return;
        }

        if (mover == null)
        {
            mover = GetComponent<IMoveable>();
        }

        if (mover == null)
        {
            Debug.LogError($"[MovementController] No IMoveable component found on {gameObject.name}", this);
            enabled = false;
            return;
        }

        mover.Initialize(stats);
        CurrentSpeed = stats.moveSpeed;
    }

    /// <summary>
    /// Moves the character in a world-space direction.
    /// </summary>
    /// <param name="direction">Normalized direction vector.</param>
    public void MoveInDirection(Vector3 direction)
    {
        if (entity == null || entity.CurrentState != EntityState.Active)
        {
            return;
        }

        mover?.MoveInDirection(direction);
    }

    /// <summary>
    /// Moves the character toward a specific destination.
    /// </summary>
    /// <param name="destination">World space destination.</param>
    public void MoveTo(Vector3 destination)
    {
        if (entity == null || entity.CurrentState != EntityState.Active)
        {
            return;
        }

        mover?.MoveTo(destination);
    }

    /// <summary>
    /// Stops any ongoing movement.
    /// </summary>
    public void Stop()
    {
        if (entity == null || entity.CurrentState != EntityState.Active)
        {
            return;
        }

        mover?.Stop();
    }

    /// <summary>
    /// Immediately continues movement toward the current destination.
    /// </summary>
    public void Continue()
    {
        if (entity == null || entity.CurrentState != EntityState.Active)
        {
            return;
        }

        mover?.Continue();
    }

    /// <summary>
    /// Moves the character toward a specific destination using a Transform reference.
    /// </summary>
    /// <param name="destinationTransform"></param>
    public void MoveTo(Transform destinationTransform)
    {
        if (entity == null || entity.CurrentState != EntityState.Active)
        {
            return;
        }

        mover?.MoveTo(destinationTransform);
    }

    /// <summary>
    /// Sets the stopping distance for the mover.
    /// </summary>
    /// <param name="stoppingDistance">The stopping distance value.</param>
    public void SetStoppingDistance(float stoppingDistance)
    {
        if (entity == null || entity.CurrentState != EntityState.Active)
        {
            return;
        }

        mover?.SetStoppingDistance(stoppingDistance);
    }
}
