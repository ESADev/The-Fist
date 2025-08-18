using UnityEngine;

/// <summary>
/// Spawns a footstep VFX (particle, decal, etc.) at intervals determined by movement speed.
/// The VFX key used is fixed to "footstep" and will be looked up in the VFXManager's library.
/// Y position is always clamped to a configured ground height (default 0).
/// </summary>
[RequireComponent(typeof(MovementController))]
public class FootstepVFXTrigger : MonoBehaviour
{
    [Header("Step VFX Configuration")] 
    [Tooltip("Base time interval between footstep VFX at the reference speed.")] 
    [SerializeField] private float baseStepInterval = 0.5f;

    [Tooltip("Minimum movement speed required before spawning footstep VFX.")] 
    [SerializeField] private float minimumSpeedThreshold = 0.1f;

    [Header("Speed Based Frequency")]
    [Tooltip("Reference speed at which the base interval applies.")] 
    [SerializeField] private float referenceSpeed = 3f;

    [Tooltip("How strongly speed affects footstep frequency (1 = linear based on ratio).")] 
    [SerializeField] private float speedInfluenceMultiplier = 1f;

    [Tooltip("Minimum possible interval regardless of speed.")] 
    [SerializeField] private float minimumStepInterval = 0.1f;

    [Header("Placement Settings")] 
    [Tooltip("Fixed Y level at which footsteps spawn.")] 
    [SerializeField] private float groundY = 0f;

    [Tooltip("Optional horizontal offset applied forward relative to movement direction.")] 
    [SerializeField] private float forwardOffset = 0.5f;

    [Tooltip("Random horizontal radius jitter for variation (XZ plane).")] 
    [SerializeField] private float positionJitterRadius = 0.5f;

    [Header("VFX Key")] 
    [Tooltip("Key used in VFXLibrary to spawn the footstep VFX.")] 
    [SerializeField] private string footstepVFXKey = "footstep";

    private MovementController movementController;
    private float timeSinceLastStep;
    private Vector3 lastPosition;

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        if (movementController == null)
        {
            Debug.LogError("[FootstepVFXTrigger] MovementController missing.", this);
            enabled = false;
        }
    }

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (!enabled || movementController == null) return;

        Vector3 currentPosition = transform.position;
        float distanceMoved = Vector3.Distance(currentPosition, lastPosition);
        float currentSpeed = distanceMoved / Mathf.Max(Time.deltaTime, 0.0001f);

        float dynamicInterval = CalculateDynamicStepInterval(currentSpeed);
        timeSinceLastStep += Time.deltaTime;

        if (currentSpeed >= minimumSpeedThreshold && timeSinceLastStep >= dynamicInterval)
        {
            SpawnFootstepVFX();
            timeSinceLastStep = 0f;
        }

        lastPosition = currentPosition;
    }

    private void SpawnFootstepVFX()
    {
        if (VFXManager.Instance == null)
        {
            return; // fail silently to avoid spam
        }

        // Base spawn position (XZ from entity, fixed Y)
        Vector3 basePos = transform.position;
        basePos.y = groundY;

        // Derive movement direction from last frame position delta (planar)
        Vector3 moveDelta = transform.position - lastPosition;
        moveDelta.y = 0f;
        Vector3 forwardDir = moveDelta.sqrMagnitude > 0.0001f ? moveDelta.normalized : transform.forward;
        if (forwardOffset != 0f)
        {
            basePos += forwardDir * forwardOffset;
        }

        // Random jitter in XZ for variation
        if (positionJitterRadius > 0f)
        {
            Vector2 jitter = Random.insideUnitCircle * positionJitterRadius;
            basePos.x += jitter.x;
            basePos.z += jitter.y;
        }

        VFXManager.Instance.PlayParticleEffect(footstepVFXKey, basePos, Quaternion.identity);
    }

    private float CalculateDynamicStepInterval(float currentSpeed)
    {
        if (currentSpeed <= 0f) return baseStepInterval;
        float speedRatio = currentSpeed / Mathf.Max(referenceSpeed, 0.0001f);
        float adjustedRatio = 1f + ((speedRatio - 1f) * speedInfluenceMultiplier);
        float interval = baseStepInterval / adjustedRatio;
        return Mathf.Max(minimumStepInterval, interval);
    }

    #region Public API
    public void ManualTrigger() => SpawnFootstepVFX();

    public float GetCurrentDynamicInterval()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        float currentSpeed = distanceMoved / Mathf.Max(Time.deltaTime, 0.0001f);
        return CalculateDynamicStepInterval(currentSpeed);
    }

    public void SetBaseInterval(float value) => baseStepInterval = Mathf.Max(0.01f, value);
    public void SetMinimumInterval(float value) => minimumStepInterval = Mathf.Max(0.01f, value);
    public void SetReferenceSpeed(float value) => referenceSpeed = Mathf.Max(0.01f, value);
    public void SetSpeedInfluence(float value) => speedInfluenceMultiplier = Mathf.Max(0f, value);
    public void SetMinimumSpeedThreshold(float value) => minimumSpeedThreshold = Mathf.Max(0f, value);
    public void SetGroundY(float value) => groundY = value;
    public void SetForwardOffset(float value) => forwardOffset = value;
    public void SetPositionJitterRadius(float value) => positionJitterRadius = Mathf.Max(0f, value);
    public void SetVFXKey(string key) { if (!string.IsNullOrEmpty(key)) footstepVFXKey = key; }
    #endregion
}
