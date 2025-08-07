using UnityEngine;

/// <summary>
/// Rotates the character towards the current target selected by AutoInteractor.
/// This component requires an AutoInteractor on the same GameObject.
/// </summary>
[RequireComponent(typeof(AutoInteractor))]
public class RotateTowardsBestTarget : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Speed at which the character rotates towards the target (degrees per second)")]
    [SerializeField] private float rotationSpeed = 360f;
    
    [Tooltip("If true, rotation will be smoothed over time. If false, rotation will be instant.")]
    [SerializeField] private bool smoothRotation = true;
    
    [Tooltip("Minimum angle difference (in degrees) before rotation stops to avoid jittering")]
    [SerializeField] private float rotationThreshold = 1f;
    
    [Tooltip("If true, only rotates around the Y-axis (suitable for ground-based units)")]
    [SerializeField] private bool lockToYAxis = true;

    private AutoInteractor autoInteractor;
    private GameObject currentTarget;
    private Transform targetTransform;

    void Start()
    {
        // Get the AutoInteractor component
        autoInteractor = GetComponentInParent<AutoInteractor>();
        
        if (autoInteractor == null)
        {
            Debug.LogError($"[RotateTowardsBestTarget] Missing AutoInteractor component on {gameObject.name}", this);
            enabled = false;
            return;
        }

        // Subscribe to target change events
        autoInteractor.OnNewTargetAcquired += OnTargetAcquired;
        autoInteractor.OnTargetLost += OnTargetLost;
    }

    void Update()
    {
        if (currentTarget != null && targetTransform != null)
        {
            RotateTowardsTarget();
        }
    }

    /// <summary>
    /// Called when AutoInteractor acquires a new target
    /// </summary>
    /// <param name="newTarget">The newly acquired target</param>
    private void OnTargetAcquired(GameObject newTarget)
    {
        currentTarget = newTarget;
        targetTransform = newTarget?.transform;
        
        if (currentTarget != null)
        {
            Debug.Log($"[RotateTowardsBestTarget] Now rotating towards: {currentTarget.name}", this);
        }
    }

    /// <summary>
    /// Called when AutoInteractor loses its current target
    /// </summary>
    private void OnTargetLost()
    {
        currentTarget = null;
        targetTransform = null;
        Debug.Log($"[RotateTowardsBestTarget] Target lost, stopping rotation", this);
    }

    /// <summary>
    /// Handles the actual rotation towards the target
    /// </summary>
    private void RotateTowardsTarget()
    {
        if (targetTransform == null) return;

        // Calculate direction to target
        Vector3 directionToTarget = targetTransform.position - transform.position;
        
        // Lock to Y-axis if specified (for ground-based units)
        if (lockToYAxis)
        {
            directionToTarget.y = 0f;
        }

        // Check if direction is valid
        if (directionToTarget.sqrMagnitude < 0.01f)
        {
            return; // Too close or same position
        }

        // Calculate target rotation
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        
        // Check if we're close enough to the target rotation
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
        if (angleDifference < rotationThreshold)
        {
            return; // Close enough, no need to rotate
        }

        // Apply rotation
        if (smoothRotation)
        {
            // Smooth rotation over time
            float rotationStep = rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationStep);
        }
        else
        {
            // Instant rotation
            transform.rotation = targetRotation;
        }
    }

    /// <summary>
    /// Manually sets a target to rotate towards (useful for testing or special cases)
    /// </summary>
    /// <param name="target">The target to rotate towards</param>
    public void SetManualTarget(GameObject target)
    {
        currentTarget = target;
        targetTransform = target?.transform;
    }

    /// <summary>
    /// Gets the current target this component is rotating towards
    /// </summary>
    /// <returns>The current target GameObject, or null if no target</returns>
    public GameObject GetCurrentTarget()
    {
        return currentTarget;
    }

    /// <summary>
    /// Gets the current angle difference to the target in degrees
    /// </summary>
    /// <returns>Angle difference in degrees, or 0 if no target</returns>
    public float GetAngleToTarget()
    {
        if (targetTransform == null) return 0f;

        Vector3 directionToTarget = targetTransform.position - transform.position;
        if (lockToYAxis)
        {
            directionToTarget.y = 0f;
        }

        if (directionToTarget.sqrMagnitude < 0.01f) return 0f;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        return Quaternion.Angle(transform.rotation, targetRotation);
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (autoInteractor != null)
        {
            autoInteractor.OnNewTargetAcquired -= OnTargetAcquired;
            autoInteractor.OnTargetLost -= OnTargetLost;
        }
    }

    // Gizmos for debugging
    private void OnDrawGizmosSelected()
    {
        if (currentTarget != null)
        {
            // Draw line to current target
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentTarget.transform.position);
            
            // Draw forward direction
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.forward * 2f);
        }
    }
}
