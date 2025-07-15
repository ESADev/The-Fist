using UnityEngine;
using System;

/// <summary>
/// Determines long range movement goals for an AI entity and commands its
/// <see cref="MovementController"/> accordingly. This component no longer
/// handles targeting or attacking.
/// </summary>
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(Interactor))]
[RequireComponent(typeof(Attacker))]
public class AIStrategyController : MonoBehaviour
{
    [Header("Strategy")]
    [Tooltip("Ultimate destination for this AI unit when play begins.")]
    public Transform strategicTarget;

    private MovementController movementController;
    private Interactor interactor;
    private Attacker attacker;
    private GameObject tacticalTarget;
    private AIMovementState currentState = AIMovementState.MovingStrategic;
    private float tacticalRange = 1f;

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        interactor = GetComponent<Interactor>();
        attacker = GetComponent<Attacker>();

        if (movementController == null)
        {
            Debug.LogError($"[AIStrategyController] Missing MovementController on {gameObject.name}", this);
            enabled = false;
        }

        if (interactor == null)
        {
            Debug.LogError($"[AIStrategyController] Missing Interactor on {gameObject.name}", this);
            enabled = false;
        }

        if (attacker == null)
        {
            Debug.LogError($"[AIStrategyController] Missing Attacker on {gameObject.name}", this);
            enabled = false;
        }

        if (attacker != null && attacker.attackerProfile != null)
        {
            foreach (var attack in attacker.attackerProfile.attacks)
            {
                if (attack != null && attack.range > tacticalRange)
                {
                    tacticalRange = attack.range;
                }
            }
        }
    }

    private void OnEnable()
    {
        if (interactor != null)
        {
            interactor.OnNewTargetAcquired += HandleTargetAcquired;
            interactor.OnTargetLost += HandleTargetLost;
        }
    }

    private void OnDisable()
    {
        if (interactor != null)
        {
            interactor.OnNewTargetAcquired -= HandleTargetAcquired;
            interactor.OnTargetLost -= HandleTargetLost;
        }
    }

    private void Start()
    {
        if (strategicTarget == null)
        {
            Debug.LogError($"[AIStrategyController] Strategic target not assigned on {gameObject.name}", this);
            return;
        }

        if (movementController != null && movementController.enabled)
        {
            Debug.Log($"[AIStrategyController] Moving towards {strategicTarget.name}");
            movementController.MoveTo(strategicTarget.transform);
        }
    }

    private void Update()
    {
        if (movementController == null || !movementController.enabled)
        {
            return;
        }

        if (currentState == AIMovementState.EngagingTactical)
        {
            UpdateTacticalRange();
        }

        switch (currentState)
        {
            case AIMovementState.EngagingTactical:
                if (tacticalTarget == null)
                {
                    currentState = AIMovementState.MovingStrategic;
                    break;
                }

                float distance = Vector3.Distance(transform.position, tacticalTarget.transform.position);
                if (distance <= tacticalRange)
                {
                    movementController.Stop();
                }
                else
                {
                    movementController.MoveTo(tacticalTarget.transform.position);
                }
                break;

            case AIMovementState.MovingStrategic:
                if (strategicTarget != null)
                {
                    movementController.MoveTo(strategicTarget.transform.position);
                }
                break;
        }
    }

    private void HandleTargetAcquired(GameObject target)
    {
        tacticalTarget = target;
        currentState = AIMovementState.EngagingTactical;
        UpdateTacticalRange();
        Debug.Log($"[AIStrategyController] Tactical target acquired: {target.name}");
    }

    private void HandleTargetLost()
    {
        tacticalTarget = null;
        currentState = AIMovementState.MovingStrategic;
        Debug.Log($"[AIStrategyController] Tactical target lost for {gameObject.name}");
    }

    /// <summary>
    /// Updates the tactical attack range based on the best available attack for the current target.
    /// </summary>
    private void UpdateTacticalRange()
    {
        if (attacker != null && tacticalTarget != null)
        {
            float range = attacker.GetBestAttackRange(tacticalTarget);
            if (range > 0f)
            {
                tacticalRange = range;
            }
        }
    }
}
