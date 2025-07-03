using UnityEngine;
using System;

/// <summary>
/// Core component that manages health for any GameObject.
/// It raises events when health changes or the entity dies.
/// </summary>
public class HealthComponent : MonoBehaviour
{
    [SerializeField] private UnitStatsSO stats;

    private float _currentHealth;

    // Event fired whenever health changes. Provides current and max values.
    public event Action<float, float> OnHealthChanged;

    // Event fired when health reaches zero.
    public event Action OnDied;

    public float CurrentHealth => _currentHealth;

    private void Awake()
    {
        if (stats == null)
        {
            Debug.LogError("[HealthComponent] UnitStatsSO is not assigned!", this);
            enabled = false;
            return;
        }

        _currentHealth = stats.maxHealth;
        OnHealthChanged?.Invoke(_currentHealth, stats.maxHealth);
    }

    /// <summary>
    /// Apply damage to this entity, taking armor into account.
    /// </summary>
    /// <param name="damageAmount">Incoming damage before armor reduction.</param>
    public void TakeDamage(float damageAmount)
    {
        float finalDamage = Mathf.Max(1f, damageAmount - stats.armor);
        _currentHealth -= finalDamage;
        _currentHealth = Mathf.Max(0f, _currentHealth);

        OnHealthChanged?.Invoke(_currentHealth, stats.maxHealth);

        if (_currentHealth <= 0f)
        {
            OnDied?.Invoke();
        }
    }

    /// <summary>
    /// Heal this entity up to its maximum health.
    /// </summary>
    /// <param name="healAmount">Amount to heal.</param>
    public void Heal(float healAmount)
    {
        _currentHealth += healAmount;
        _currentHealth = Mathf.Min(_currentHealth, stats.maxHealth);

        OnHealthChanged?.Invoke(_currentHealth, stats.maxHealth);
    }
}
