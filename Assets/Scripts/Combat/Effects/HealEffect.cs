using UnityEngine;

/// <summary>
/// Effect that heals a target's HealthComponent.
/// </summary>
public class HealEffect : MonoBehaviour, IEffect
{
    [SerializeField] public float healAmount = 10f;

    public void Apply(GameObject target)
    {
        HealthComponent health = target.GetComponent<HealthComponent>();
        if (health != null)
        {
            health.Heal(healAmount);
        }
    }
}
