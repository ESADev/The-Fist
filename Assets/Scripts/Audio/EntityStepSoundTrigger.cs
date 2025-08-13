using UnityEngine;

/// <summary>
/// Component that triggers step sounds at regular intervals when an entity is moving.
/// Attach this to entities that should play footstep sounds.
/// </summary>
[RequireComponent(typeof(MovementController))][RequireComponent(typeof(Entity))]
public class EntityStepSoundTrigger : MonoBehaviour
{
    [Header("Step Sound Configuration")]
    [Tooltip("Base time interval between step sounds in seconds at normal speed.")]
    [SerializeField] private float baseStepInterval = 0.5f;
    
    [Tooltip("Minimum movement speed required to trigger step sounds.")]
    [SerializeField] private float minimumSpeedThreshold = 0.1f;
    
    [Header("Speed-Based Step Frequency")]
    [Tooltip("Reference speed for normal step frequency. Higher speeds will increase step frequency.")]
    [SerializeField] private float referenceSpeed = 3f;
    
    [Tooltip("Multiplier for how much speed affects step frequency. Higher values = more dramatic changes.")]
    [SerializeField] private float speedInfluenceMultiplier = 1f;
    
    [Tooltip("Minimum step interval (maximum step frequency) regardless of speed.")]
    [SerializeField] private float minimumStepInterval = 0.1f;

    private Entity entity;
    private MovementController movementController;
    private float timeSinceLastStep = 0f;
    private Vector3 lastPosition;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        movementController = GetComponent<MovementController>();

        if (entity == null)
        {
            Debug.LogError($"[EntityStepSoundTrigger] Entity component missing on {gameObject.name}.", this);
            enabled = false;
        }
        
        if(movementController == null)
        {
            Debug.LogError($"[EntityStepSoundTrigger] MovementController component missing on {gameObject.name}.", this);
            enabled = false;
        }
    }

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (entity == null || entity.CurrentState != EntityState.Active)
        {
            return;
        }

        // Calculate movement speed
        Vector3 currentPosition = transform.position;
        float distanceMoved = Vector3.Distance(currentPosition, lastPosition);
        float currentSpeed = distanceMoved / Time.deltaTime;

        // Calculate dynamic step interval based on speed
        float dynamicStepInterval = CalculateDynamicStepInterval(currentSpeed);

        // Update timer
        timeSinceLastStep += Time.deltaTime;

        // Check if we should play a step sound
        if (currentSpeed >= minimumSpeedThreshold && timeSinceLastStep >= dynamicStepInterval)
        {
            TriggerStepSound();
            timeSinceLastStep = 0f;
        }

        lastPosition = currentPosition;
    }

    /// <summary>
    /// Triggers a step sound through the SFXManager.
    /// </summary>
    private void TriggerStepSound()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayStepSound(entity);
        }
    }

    /// <summary>
    /// Calculates the dynamic step interval based on current movement speed.
    /// Higher speeds result in shorter intervals (more frequent steps).
    /// </summary>
    /// <param name="currentSpeed">The current movement speed of the entity.</param>
    /// <returns>The calculated step interval in seconds.</returns>
    private float CalculateDynamicStepInterval(float currentSpeed)
    {
        if (currentSpeed <= 0f)
            return baseStepInterval;

        // Calculate speed ratio compared to reference speed
        float speedRatio = currentSpeed / referenceSpeed;
        
        // Apply speed influence multiplier
        float adjustedSpeedRatio = 1f + ((speedRatio - 1f) * speedInfluenceMultiplier);
        
        // Calculate new interval (inverse relationship: higher speed = shorter interval)
        float dynamicInterval = baseStepInterval / adjustedSpeedRatio;
        
        // Clamp to minimum interval to prevent extremely fast steps
        return Mathf.Max(minimumStepInterval, dynamicInterval);
    }

    /// <summary>
    /// Manually trigger a step sound. Can be called from animation events or other systems.
    /// </summary>
    public void ManualStepTrigger()
    {
        TriggerStepSound();
    }

    /// <summary>
    /// Sets the base step interval dynamically.
    /// </summary>
    /// <param name="newInterval">New base interval in seconds between step sounds at normal speed.</param>
    public void SetStepInterval(float newInterval)
    {
        baseStepInterval = Mathf.Max(0.1f, newInterval);
    }

    /// <summary>
    /// Sets the minimum speed threshold for triggering step sounds.
    /// </summary>
    /// <param name="newThreshold">New minimum speed threshold.</param>
    public void SetSpeedThreshold(float newThreshold)
    {
        minimumSpeedThreshold = Mathf.Max(0f, newThreshold);
    }

    /// <summary>
    /// Sets the reference speed for step frequency calculations.
    /// </summary>
    /// <param name="newReferenceSpeed">New reference speed.</param>
    public void SetReferenceSpeed(float newReferenceSpeed)
    {
        referenceSpeed = Mathf.Max(0.1f, newReferenceSpeed);
    }

    /// <summary>
    /// Sets the speed influence multiplier.
    /// </summary>
    /// <param name="newMultiplier">New speed influence multiplier.</param>
    public void SetSpeedInfluenceMultiplier(float newMultiplier)
    {
        speedInfluenceMultiplier = Mathf.Max(0f, newMultiplier);
    }

    /// <summary>
    /// Gets the current dynamic step interval based on the entity's current speed.
    /// </summary>
    /// <returns>Current step interval in seconds.</returns>
    public float GetCurrentStepInterval()
    {
        Vector3 currentPosition = transform.position;
        float distanceMoved = Vector3.Distance(currentPosition, lastPosition);
        float currentSpeed = distanceMoved / Time.deltaTime;
        return CalculateDynamicStepInterval(currentSpeed);
    }
}
