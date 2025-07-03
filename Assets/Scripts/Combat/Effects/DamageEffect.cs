using UnityEngine;

/// <summary>
/// Effect that deals damage to a target's HealthComponent.
/// Damage value comes from a UnitStatsSO for data-driven balancing.
/// </summary>
public class DamageEffect : MonoBehaviour, IEffect
{
    [SerializeField] private UnitStatsSO stats;

    public void Apply(GameObject target)
    {
        if (stats == null) return;

        HealthComponent health = target.GetComponent<HealthComponent>();
        if (health != null)
        {
            health.TakeDamage(stats.damage);
        }
    }
}
