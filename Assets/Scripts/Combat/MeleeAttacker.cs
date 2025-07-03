using UnityEngine;
using System;

/// <summary>
/// Handles melee attack logic. The component listens to a
/// <see cref="UnitAIController"/> if present but can also be
/// controlled manually (e.g. by player scripts) via
/// <see cref="SetTarget"/> and <see cref="ClearTarget"/>.
/// This decouples the attack behaviour from any specific
/// movement or targeting implementation.
/// </summary>
public class MeleeAttacker : MonoBehaviour
{
    // Shared stats asset provides attack rate and range
    [SerializeField] private MeleeUnitStatsSO stats;
    [SerializeField] private MonoBehaviour effectSource;

    private IEffect _effect;
    private UnitAIController _ai; // optional AI brain, may be null for player
    private GameObject _currentTarget;
    private float _cooldownTimer;

    /// <summary>
    /// Manually assign a target for this attacker.
    /// Useful for player controlled units that do not use
    /// <see cref="UnitAIController"/>.
    /// </summary>
    /// <param name="target">Target GameObject to attack.</param>
    public void SetTarget(GameObject target)
    {
        _currentTarget = target;
    }

    /// <summary>
    /// Clear the current target so the attacker stops attacking.
    /// </summary>
    public void ClearTarget()
    {
        _currentTarget = null;
    }

    /* Example usage for a player controlled unit:
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                SetTarget(hit.collider.gameObject);
            }
        }
    }
    */

    private void Awake()
    {
        // Grab the AI controller if one exists. Player controlled units
        // might not have this component attached.
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
        SetTarget(target);
    }

    private void HandleTargetLost()
    {
        ClearTarget();
    }

    private void PerformAttack()
    {
        if (_effect != null && _currentTarget != null)
        {
            _effect.Apply(_currentTarget);
        }
    }
}
