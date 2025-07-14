using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Moves an NPC using Unity's <see cref="NavMeshAgent"/> component.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMover : MonoBehaviour, IMoveable
{
    private NavMeshAgent agent;
    private MovementStatsSO stats;
    Transform target;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError($"[NavMeshMover] Missing NavMeshAgent on {gameObject.name}", this);
            enabled = false;
        }
    }

    void Update()
    {
        // Refresh target destination if it exists
        if (target != null)
        {
            RefreshTargetDestination();
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
            Debug.LogError($"[NavMeshMover] MovementStatsSO is null on {gameObject.name}", this);
            return;
        }
        this.stats = stats;
        agent.speed = stats.moveSpeed;
        agent.angularSpeed = stats.turnSpeed;
    }

    /// <summary>
    /// Moves the agent in a direction.
    /// </summary>
    /// <param name="direction">Normalized direction vector.</param>
    public void MoveInDirection(Vector3 direction)
    {
        if (stats == null) return;
        agent.isStopped = false;
        Vector3 velocity = direction * stats.moveSpeed;
        agent.velocity = Vector3.Lerp(agent.velocity, velocity, 1f - Mathf.Pow(1f - stats.movementSmoothness, Time.deltaTime * 60f));
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f - Mathf.Pow(1f - stats.rotationSmoothness, Time.deltaTime * 60f));
        }
    }

    /// <summary>
    /// Moves the agent toward a destination.
    /// </summary>
    /// <param name="destination">World space destination.</param>
    public void MoveTo(Vector3 destination)
    {
        if (stats == null) return;
        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    /// <summary>
    /// Moves the agent toward a destination after assigning the target.
    /// </summary>
    /// <param name="destinationTransform"></param>
    public void MoveTo(Transform destinationTransform)
    {
        target = destinationTransform;
        MoveTo(destinationTransform.position);
    }

    /// <summary>
    /// Sets a new movement speed.
    /// </summary>
    /// <param name="newSpeed">Speed value.</param>
    public void SetSpeed(float newSpeed)
    {
        if (stats == null) return;
        agent.speed = newSpeed;
    }

    /// <summary>
    /// Stops the agent's movement.
    /// </summary>
    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }

    /// <summary>
    /// Refreshes the target destination if it exists.
    /// </summary>
    void RefreshTargetDestination()
    {
        if (target != null)
        {
            MoveTo(target);
        }
    }

    public void Continue()
    {
        agent.isStopped = false;
        RefreshTargetDestination();
    }
}
