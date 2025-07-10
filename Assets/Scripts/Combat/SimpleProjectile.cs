using UnityEngine;

/// <summary>
/// Basic projectile that travels toward a target and applies damage on impact.
/// </summary>
public class SimpleProjectile : MonoBehaviour
{
    private float speed;

    private GameObject target;
    private GameObject attacker;
    private AttackDefinitionSO attackData;

    /// <summary>
    /// Initializes the projectile with its attacker, target and attack data.
    /// </summary>
    /// <param name="attacker">Origin of the projectile.</param>
    /// <param name="target">Target to hit.</param>
    /// <param name="attackData">Attack definition used for damage calculation.</param>
    public void Initialize(GameObject attacker, GameObject target, AttackDefinitionSO attackData)
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
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.transform.position) <= 0.2f)
        {
            if (target.TryGetComponent<Health>(out var health))
            {
                health.TakeDamage(attackData.damage, attacker, attackData);
            }
            else
            {
                Debug.LogWarning("[SimpleProjectile] Target has no Health component.", this);
            }
            Destroy(gameObject);
        }
    }
}
