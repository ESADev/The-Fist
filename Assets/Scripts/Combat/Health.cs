using System;
using UnityEngine;

/// <summary>
/// Manages the health of an entity and handles damage and healing logic.
/// </summary>
public class Health : MonoBehaviour, IDestructible, IHealable
{
    [Header("Stats")]
    [Tooltip("Configuration asset containing health values.")]
    [HideInInspector] public HealthStatsSO stats;

    /// <summary>
    /// Current amount of health.
    /// </summary>
    public float CurrentHealth { get; private set; }

    /// <summary>
    /// Indicates whether the entity has died.
    /// </summary>
    public bool IsDead { get; private set; }

    /// <summary>
    /// Fired whenever the health value changes. Parameters are current health, max health, and whether the entity is dead.
    /// </summary>
    public event Action<float, float, bool> OnHealthChanged;

    /// <summary>
    /// Fired when the entity dies.
    /// </summary>
    public event Action<GameObject> OnDied;

    /// <summary>
    /// Fired specifically when this entity takes damage (after values are applied).
    /// Provides the DamageInfo used for global events so local listeners can react without subscribing globally.
    /// </summary>
    public event Action<DamageInfo> OnDamaged;

    // --- Auto Revive (Regeneration) State ---
    private float _timeSinceLastDamage;
    private bool _isRegenerating;

    public void Initialize(HealthStatsSO healthStats)
    {
        stats = healthStats;

        if (stats == null)
        {
            Debug.LogError($"[Health] HealthStatsSO is not assigned on {gameObject.name} and could not be auto-resolved.", this);
            enabled = false;
            return;
        }

        CurrentHealth = stats.maxHealth;
        IsDead = false;
    }

    /// <summary>
    /// Applies damage to this entity.
    /// </summary>
    /// <param name="baseDamage">Raw damage before mitigation.</param>
    /// <param name="attacker">GameObject that initiated the attack.</param>
    /// <param name="attackData">Definition of the attack that dealt the damage.</param>
    public void TakeDamage(float baseDamage, GameObject attacker, AttackDefinitionSO attackData)
    {
        if (IsDead || stats == null)
        {
            return;
        }

        float finalDamage = Mathf.Max(baseDamage - stats.armor, 0f);
        CurrentHealth = Mathf.Clamp(CurrentHealth - finalDamage, 0f, stats.maxHealth);

        Debug.Log($"[Health] {gameObject.name} took {finalDamage} damage from {(attacker != null ? attacker.name : "Unknown")}. Remaining health: {CurrentHealth}/{stats.maxHealth}");

    // Reset revive timers upon taking damage
    _timeSinceLastDamage = 0f;
    _isRegenerating = false;

    OnHealthChanged?.Invoke(CurrentHealth, stats.maxHealth, IsDead);

        DamageInfo damageInfo = new DamageInfo(attacker, gameObject, finalDamage, attackData);
    OnDamaged?.Invoke(damageInfo);
        GameEvents.TriggerOnUnitDamaged(damageInfo);

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Restores health to this entity.
    /// </summary>
    /// <param name="healAmount">Amount of health to restore.</param>
    public void Heal(float healAmount)
    {
        if (IsDead || stats == null)
        {
            return;
        }

        float previousHealth = CurrentHealth;
        CurrentHealth = Mathf.Clamp(CurrentHealth + healAmount, 0f, stats.maxHealth);

        if (Math.Abs(CurrentHealth - previousHealth) > Mathf.Epsilon)
        {
            Debug.Log($"[Health] {gameObject.name} healed for {healAmount}. Current health: {CurrentHealth}/{stats.maxHealth}");
            OnHealthChanged?.Invoke(CurrentHealth, stats.maxHealth, false);
        }
    }

    /// <summary>
    /// Handles the death of the entity.
    /// </summary>
    private void Die()
    {
        if (IsDead)
        {
            return;
        }

        IsDead = true;
        Debug.Log($"[Health] {gameObject.name} died.");
        OnDied?.Invoke(gameObject);
        GameEvents.TriggerOnUnitDied(gameObject);
    }

    private void Update()
    {
        HandleAutoRevive();
    }

    /// <summary>
    /// Handles automatic health regeneration ("revive") when out of combat.
    /// Trigger: No damage taken for reviveDelay seconds, then regenerate reviveSpeed * maxHealth per second until full.
    /// Interrupted: Any damage resets timer and stops regeneration.
    /// Never regenerates if the entity is dead.
    /// </summary>
    private void HandleAutoRevive()
    {
        if (stats == null || IsDead || !stats.autoReviveEnabled)
        {
            return;
        }

        if (!_isRegenerating)
        {
            // Count up time since last damage until we can start regenerating
            if (_timeSinceLastDamage < stats.reviveDelay)
            {
                _timeSinceLastDamage += Time.deltaTime;
                if (_timeSinceLastDamage >= stats.reviveDelay)
                {
                    _isRegenerating = true;
                }
            }
        }

        if (_isRegenerating && CurrentHealth < stats.maxHealth)
        {
            float previous = CurrentHealth;
            float regenAmount = stats.maxHealth * stats.reviveSpeed * Time.deltaTime;
            CurrentHealth = Mathf.Min(CurrentHealth + regenAmount, stats.maxHealth);

            if (Mathf.Abs(CurrentHealth - previous) > Mathf.Epsilon)
            {
                OnHealthChanged?.Invoke(CurrentHealth, stats.maxHealth, IsDead);
            }

            if (CurrentHealth >= stats.maxHealth)
            {
                _isRegenerating = false; // Done
            }
        }
    }
}
