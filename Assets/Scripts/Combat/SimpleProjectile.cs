using UnityEngine;

/// <summary>
/// Basic projectile that follows a parabolic trajectory toward a target and applies damage on impact.
/// </summary>
public class SimpleProjectile : MonoBehaviour
{
    private float speed;
    private Vector3 velocity;
    private Vector3 targetPosition;
    private float gravity = 9.81f;

    private GameObject target;
    private GameObject attacker;
    private RangedAttackDefinitionSO attackData;

    /// <summary>
    /// Initializes the projectile with its attacker, target and attack data.
    /// </summary>
    /// <param name="attacker">Origin of the projectile.</param>
    /// <param name="target">Target to hit.</param>
    /// <param name="attackData">Attack definition used for damage calculation.</param>
    public void Initialize(GameObject attacker, GameObject target, RangedAttackDefinitionSO attackData)
    {
        if (target == null || attackData == null)
        {
            Debug.LogError("[SimpleProjectile] Invalid initialization parameters.", this);
            Destroy(gameObject);
            return;
        }

        this.attacker = attacker;
        this.target = target;
        this.attackData = attackData;
        speed = attackData.projectileSpeed;

        // Store the target's position at the moment of firing
        targetPosition = target.transform.position;

        // Calculate initial velocity for parabolic trajectory
        velocity = CalculateBallisticVelocity(transform.position, targetPosition, speed);
    }

    private void Update()
    {
        // Apply ballistic motion
        velocity.y -= gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        // Check if we've hit the actual target (not just the original position)
        if (target != null && Vector3.Distance(transform.position, target.transform.position) <= 0.5f)
        {
            HandleTargetHit();
            return;
        }

        // Check if we've hit the ground or reached the original target area (miss)
        if (Vector3.Distance(transform.position, targetPosition) <= 0.5f || transform.position.y <= targetPosition.y)
        {
            HandleMiss();
            return;
        }

        // Destroy projectile if it goes too far or too low
        if (Vector3.Distance(transform.position, targetPosition) > 100f || transform.position.y < targetPosition.y - 10f)
        {
            Destroy(gameObject);
        }
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

        // Calculate time of flight using the desired speed
        float timeToTarget = horizontalDistance / launchSpeed;

        // Calculate required vertical velocity to reach target height
        float verticalVelocity = (heightDifference / timeToTarget) + (0.5f * gravity * timeToTarget);

        // Calculate horizontal velocity components
        Vector3 horizontalVelocity = horizontalDisplacement.normalized * launchSpeed;

        return new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);
    }

    /// <summary>
    /// Handles when the projectile actually hits the target.
    /// </summary>
    private void HandleTargetHit()
    {
        if (target != null && target.TryGetComponent<Health>(out var health))
        {
            health.TakeDamage(attackData.damage, attacker, attackData);
            Debug.Log($"[SimpleProjectile] Direct hit on {target.name}!", this);
        }
        else
        {
            Debug.LogWarning("[SimpleProjectile] Target has no Health component.", this);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Handles when the projectile misses the target and hits the ground/original position.
    /// </summary>
    private void HandleMiss()
    {
        Debug.Log("[SimpleProjectile] Projectile missed target and hit the ground.", this);
        // Could add area damage or other effects here if desired
        Destroy(gameObject);
    }
}
