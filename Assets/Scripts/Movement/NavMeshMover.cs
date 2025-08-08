using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Moves an NPC using Unity's <see cref="NavMeshAgent"/> component.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMover : MonoBehaviour, IMoveable
{
    [Header("Dynamic Target Settings")]
    [SerializeField] private float updateDistanceThreshold = 3f; // Minimum distance target must move before updating
    [SerializeField] private float maxUpdateInterval = 1f; // Maximum time between updates (seconds)
    [SerializeField] private float minUpdateInterval = 0.3f; // Minimum time between updates (seconds)
    [SerializeField] private bool usePredictiveTargeting = true; // Aim ahead of moving targets
    [SerializeField] private float predictionTime = 0.5f; // How far ahead to predict (seconds)
    [SerializeField] private float pathDeviationThreshold = 5f; // How far off the path before forced update

    private NavMeshAgent agent;
    private MovementStatsSO stats;
    private Transform target;
    
    // Dynamic target tracking variables
    private Vector3 lastTargetPosition;
    private Vector3 lastSetDestination;
    private float lastUpdateTime;
    private Vector3 targetVelocity;
    private Vector3 previousTargetPosition;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError($"[NavMeshMover] Missing NavMeshAgent on {gameObject.name}", this);
            enabled = false;
            return;
        }

        // Initialize tracking variables
        lastUpdateTime = Time.time;
        
        StartCoroutine(CustomUpdate());
    }

    IEnumerator CustomUpdate()
    {
        while (true)
        {
            // Refresh target destination if it exists
            if (target != null)
            {
                UpdateTargetTracking();
                
                if (ShouldUpdateDestination())
                {
                    RefreshTargetDestination();
                }
            }

            // Wait for a longer interval to reduce interruptions
            yield return new WaitForSeconds(minUpdateInterval);
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
        agent.acceleration = stats.movementSmoothness * 500f;
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

        Debug.Log($"[NavMeshMover] Moving to {destination} from {gameObject.name}", this);

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
        if (target != null)
        {
            // Initialize tracking when setting new target
            lastTargetPosition = target.position;
            previousTargetPosition = target.position;
            targetVelocity = Vector3.zero;
            lastUpdateTime = Time.time;
        }
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
        agent.updateRotation = false;
        Debug.Log($"[NavMeshMover] {gameObject.name} has stopped moving.");
    }

    /// <summary>
    /// Refreshes the target destination if it exists.
    /// </summary>
    void RefreshTargetDestination()
    {
        if (target != null)
        {
            Vector3 destinationToSet = target.position;
            
            // Apply predictive targeting if enabled
            if (usePredictiveTargeting && targetVelocity.magnitude > 0.5f)
            {
                Vector3 predictedPosition = target.position + (targetVelocity * predictionTime);
                
                // Check if predicted position is on NavMesh
                NavMeshHit hit;
                if (NavMesh.SamplePosition(predictedPosition, out hit, agent.height * 2f, NavMesh.AllAreas))
                {
                    destinationToSet = hit.position;
                }
            }
            
            // Only set destination if it's significantly different from current destination
            float distanceFromCurrentDestination = Vector3.Distance(destinationToSet, lastSetDestination);
            if (distanceFromCurrentDestination > 1f)
            {
                lastSetDestination = destinationToSet;
                lastTargetPosition = target.position;
                lastUpdateTime = Time.time;
                
                agent.SetDestination(destinationToSet);
                
                Debug.Log($"[NavMeshMover] Updated destination to {destinationToSet} for {gameObject.name} (distance change: {distanceFromCurrentDestination:F1})");
            }
        }
    }

    /// <summary>
    /// Updates target tracking variables to calculate velocity and movement.
    /// </summary>
    private void UpdateTargetTracking()
    {
        if (target == null) return;
        
        Vector3 currentTargetPosition = target.position;
        float deltaTime = minUpdateInterval; // Use consistent delta time
        
        // Calculate target velocity
        Vector3 newVelocity = (currentTargetPosition - previousTargetPosition) / deltaTime;
        
        // Smooth velocity calculation to avoid jitter - more conservative smoothing
        targetVelocity = Vector3.Lerp(targetVelocity, newVelocity, deltaTime * 2f);
        
        previousTargetPosition = currentTargetPosition;
    }

    /// <summary>
    /// Determines if the destination should be updated based on distance and time thresholds.
    /// </summary>
    private bool ShouldUpdateDestination()
    {
        if (target == null) return false;
        
        float timeSinceLastUpdate = Time.time - lastUpdateTime;
        float distanceFromLastPosition = Vector3.Distance(target.position, lastTargetPosition);
        
        // Don't update if agent is still actively moving toward destination
        // This prevents the stop-start behavior
        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance && agent.velocity.magnitude > 0.1f)
        {
            // Only update if target moved very far or if we're way off course
            if (distanceFromLastPosition < updateDistanceThreshold * 2f && timeSinceLastUpdate < maxUpdateInterval)
            {
                // Check if current path would still get us reasonably close to target
                Vector3 finalDestination = agent.path.corners[agent.path.corners.Length - 1];
                float distanceFromFinalToTarget = Vector3.Distance(finalDestination, target.position);
                
                // If our current path destination is still close enough to target, don't update
                if (distanceFromFinalToTarget < pathDeviationThreshold)
                {
                    return false;
                }
            }
        }
        
        // Force update if maximum interval exceeded
        if (timeSinceLastUpdate >= maxUpdateInterval)
        {
            return true;
        }
        
        // Update if target moved significantly and minimum interval has passed
        if (distanceFromLastPosition >= updateDistanceThreshold && timeSinceLastUpdate >= minUpdateInterval)
        {
            return true;
        }
        
        // Update if agent has reached destination but target is still far
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            float distanceToCurrentTarget = Vector3.Distance(agent.transform.position, target.position);
            if (distanceToCurrentTarget > agent.stoppingDistance + 1f)
            {
                return true;
            }
        }
        
        return false;
    }

    public void Continue()
    {
        agent.isStopped = false;
        agent.updateRotation = true;
        
        // Reset tracking when continuing
        if (target != null)
        {
            lastTargetPosition = target.position;
            previousTargetPosition = target.position;
            lastUpdateTime = Time.time;
        }
        
        RefreshTargetDestination();
    }

    /// <summary>
    /// Sets the stopping distance for the agent.
    /// </summary>
    /// <param name="stoppingDistance">The stopping distance value.</param>
    public void SetStoppingDistance(float stoppingDistance)
    {
        agent.stoppingDistance = stoppingDistance;
    }

    /// <summary>
    /// Configures dynamic target tracking parameters.
    /// </summary>
    /// <param name="distanceThreshold">Minimum distance target must move before updating destination.</param>
    /// <param name="maxInterval">Maximum time between destination updates.</param>
    /// <param name="minInterval">Minimum time between destination updates.</param>
    /// <param name="pathDeviation">How far off the current path before forcing an update.</param>
    public void ConfigureTargetTracking(float distanceThreshold, float maxInterval = 1f, float minInterval = 0.3f, float pathDeviation = 5f)
    {
        updateDistanceThreshold = distanceThreshold;
        maxUpdateInterval = maxInterval;
        minUpdateInterval = minInterval;
        pathDeviationThreshold = pathDeviation;
    }

    /// <summary>
    /// Enables or disables predictive targeting.
    /// </summary>
    /// <param name="enabled">Whether to use predictive targeting.</param>
    /// <param name="predictionTime">How far ahead to predict movement.</param>
    public void SetPredictiveTargeting(bool enabled, float predictionTime = 0.5f)
    {
        usePredictiveTargeting = enabled;
        this.predictionTime = predictionTime;
    }
}
