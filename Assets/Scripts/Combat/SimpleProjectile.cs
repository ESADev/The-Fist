using UnityEngine;

/// <summary>
/// Basic projectile that follows a parabolic trajectory and applies damage on impact with Health components.
/// Requires a Rigidbody and Collider component for proper collision detection.
/// </summary>
public class SimpleProjectile : MonoBehaviour
{
    private float speed;
    private Vector3 targetPosition;

    private GameObject attacker;
    private RangedAttackDefinitionSO attackData;
    private Rigidbody rb;
    private FactionType targetFaction;

    /// <summary>
    /// Initializes the projectile with its attacker, target and attack data.
    /// </summary>
    /// <param name="attacker">Origin of the projectile.</param>
    /// <param name="target">Target position to aim for.</param>
    /// <param name="attackData">Attack definition used for damage calculation.</param>
    public void Initialize(GameObject attacker, GameObject target, RangedAttackDefinitionSO attackData, FactionType targetFaction)
    {
        if (target == null || attackData == null)
        {
            Debug.LogError("[SimpleProjectile] Invalid initialization parameters.", this);
            Destroy(gameObject);
            return;
        }

        this.attacker = attacker;
        this.attackData = attackData;
        this.targetFaction = targetFaction;
        speed = attackData.projectileSpeed;

        // Get or add Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Configure Rigidbody for projectile physics
        rb.useGravity = true;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;

        Vector3 targetPosOffset = new(0, 0.5f, 0);

        // Store the target's position at the moment of firing
        targetPosition = target.transform.position + targetPosOffset;

        // Calculate and apply initial velocity for parabolic trajectory
        Vector3 initialVelocity = CalculateBallisticVelocity(transform.position, targetPosition, speed);
        rb.linearVelocity = initialVelocity;
    }

    private void Update()
    {
        // Destroy projectile if it goes too far or too low (failsafe)
        if (Vector3.Distance(transform.position, targetPosition) > 100f || transform.position.y < targetPosition.y - 10f)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Handles collision detection when the projectile hits something.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        Entity target = collision.collider.GetComponentInParent<Entity>();
        if (target != null && target.gameObject == attacker)
        {
            return;
        }

        // VFX

        // SFX
        SFXManager.Instance.PlaySound("arrow hit", collision.transform.position);

        // Check if the hit object has a Health component
        if (target != null && target.Faction.CurrentFaction == targetFaction)
        {
            target.Health.TakeDamage(attackData.damage, attacker, attackData);

            Debug.Log($"[SimpleProjectile] Hit {target.gameObject.name}!", this);
            Destroy(gameObject);
        }

        HandleMiss();
    }

    /// <summary>
    /// Calculates the initial velocity needed for a ballistic trajectory to reach the target.
    /// </summary>
    /// <param name="startPos">Starting position of the projectile</param>
    /// <param name="targetPos">Target position</param>
    /// <param name="launchSpeed">Initial speed of the projectile</param>
    /// <returns>Initial velocity vector</returns>
    private Vector3 CalculateBallisticVelocity(Vector3 startPos, Vector3 targetPos, float launchSpeed)
    {
        Vector3 displacement = targetPos - startPos;
        Vector3 horizontalDisplacement = new Vector3(displacement.x, 0, displacement.z);
        float horizontalDistance = horizontalDisplacement.magnitude;
        float heightDifference = displacement.y;

        // Use Unity's Physics.gravity instead of our custom gravity
        float gravityMagnitude = Physics.gravity.magnitude;

        // Calculate time of flight using the desired speed
        float timeToTarget = horizontalDistance / launchSpeed;

        // Calculate required vertical velocity to reach target height
        float verticalVelocity = (heightDifference / timeToTarget) + (0.5f * gravityMagnitude * timeToTarget);

        // Calculate horizontal velocity components
        Vector3 horizontalVelocity = horizontalDisplacement.normalized * launchSpeed;

        return new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);
    }

    /// <summary>
    /// Handles when the projectile misses and hits the ground.
    /// </summary>
    private void HandleMiss()
    {
        Debug.Log("[SimpleProjectile] Projectile missed and hit the ground.", this);
        Destroy(gameObject);
    }
}
