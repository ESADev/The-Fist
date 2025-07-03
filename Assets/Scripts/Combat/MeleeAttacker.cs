using UnityEngine;
using System;

/// <summary>
/// Handles melee attack logic by listening to UnitAIController events.
/// Uses a DamageEffect to apply damage when in range and off cooldown.
/// </summary>
[RequireComponent(typeof(UnitAIController))]
public class MeleeAttacker : MonoBehaviour
{
    // Shared stats asset provides attack rate and range
    [SerializeField] private MeleeUnitStatsSO stats;
    [SerializeField] private MonoBehaviour effectSource;

    private IEffect _effect;
    private UnitAIController _ai;
    private GameObject _currentTarget;
    private float _cooldownTimer;

    private void Awake()
    {
        _ai = GetComponent<UnitAIController>();
        _effect = effectSource as IEffect;
        if (stats == null)
        {
            Debug.LogError("[MeleeAttacker] Stats asset not assigned.", this);
            enabled = false;
        }
        if (_effect == null)
        {
            Debug.LogError("[MeleeAttacker] Effect source must implement IEffect.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (_ai != null)
        {
            _ai.OnTargetAcquired += HandleTargetAcquired;
            _ai.OnTargetLost += HandleTargetLost;
        }
    }

    private void OnDisable()
    {
        if (_ai != null)
        {
            _ai.OnTargetAcquired -= HandleTargetAcquired;
            _ai.OnTargetLost -= HandleTargetLost;
        }
    }

    private void Update()
    {
        _cooldownTimer -= Time.deltaTime;

        if (_currentTarget == null) return;

        float distance = Vector3.Distance(transform.position, _currentTarget.transform.position);
        if (distance <= stats.attackRange && _cooldownTimer <= 0f)
        {
            PerformAttack();
            _cooldownTimer = stats.attackRate;
        }
    }

    private void HandleTargetAcquired(GameObject target)
    {
        _currentTarget = target;
    }

    private void HandleTargetLost()
    {
        _currentTarget = null;
    }

    private void PerformAttack()
    {
        if (_effect != null && _currentTarget != null)
        {
            _effect.Apply(_currentTarget);
        }
    }
}
