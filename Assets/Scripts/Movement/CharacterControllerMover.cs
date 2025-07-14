using UnityEngine;

/// <summary>
/// Moves a character using Unity's <see cref="CharacterController"/> component.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class CharacterControllerMover : MonoBehaviour, IMoveable
{
    private CharacterController characterController;
    private MovementStatsSO stats;
    private Vector3 currentVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError($"[CharacterControllerMover] Missing CharacterController on {gameObject.name}", this);
            enabled = false;
        }
    }

    /// <summary>
    /// Initializes the mover with movement statistics.
    /// </summary>
    /// <param name="stats">Movement stats asset.</param>
    public void Initialize(MovementStatsSO stats)
    {
        if (stats == null)
        {
            Debug.LogError($"[CharacterControllerMover] MovementStatsSO is null on {gameObject.name}", this);
            return;
        }
        this.stats = stats;
    }

    /// <summary>
    /// Moves the character in a direction.
    /// </summary>
    /// <param name="direction">Normalized direction.</param>
    public void MoveInDirection(Vector3 direction)
    {
        if (stats == null) return;
        Vector3 targetVelocity = direction * stats.moveSpeed;
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, 1f - Mathf.Pow(1f - stats.movementSmoothness, Time.deltaTime * 60f));
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f - Mathf.Pow(1f - stats.rotationSmoothness, Time.deltaTime * 60f));
        }
        characterController.Move(currentVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Moves the character towards a destination.
    /// </summary>
    /// <param name="destination">World space destination.</param>
    public void MoveTo(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position).normalized;
        MoveInDirection(direction);
    }

    /// <summary>
    /// Sets a new movement speed.
    /// </summary>
    /// <param name="newSpeed">Speed value.</param>
    public void SetSpeed(float newSpeed)
    {
        if (stats == null) return;
        stats.moveSpeed = newSpeed;
    }

    /// <summary>
    /// Stops the character's movement.
    /// </summary>
    public void Stop()
    {
        currentVelocity = Vector3.zero;
    }

    public void MoveTo(Transform destinationTransform)
    {
        throw new System.NotImplementedException();
    }

    public void Continue()
    {
        throw new System.NotImplementedException();
    }
}
