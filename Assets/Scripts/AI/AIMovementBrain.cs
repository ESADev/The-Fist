using UnityEngine;
using System;

/// <summary>
/// Determines long range movement goals for an AI entity and commands its
/// <see cref="MovementController"/> accordingly. This component no longer
/// handles targeting or attacking.
/// </summary>
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(AutoInteractor))]
[RequireComponent(typeof(Attacker))]
[RequireComponent(typeof(Entity))]
public class AIMovementBrain : MonoBehaviour
{
    [Header("Strategy")]
    [Tooltip("Ultimate destination for this AI unit when play begins.")]
    public Transform strategicTarget;

    private MovementController movementController;
    private AutoInteractor interactor;
    private Attacker attacker;
    private Entity entity;
    private GameObject tacticalTarget;
    private AIMovementState currentState = AIMovementState.MovingStrategic;
    private float tacticalRange = 1f;

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        interactor = GetComponent<AutoInteractor>();
        attacker = GetComponent<Attacker>();
        entity = GetComponent<Entity>();

        if (movementController == null)
        {
            Debug.LogError($"[AIMovementBrain] Missing MovementController on {gameObject.name}", this);
            enabled = false;
        }

        if (interactor == null)
        {
            Debug.LogError($"[AIMovementBrain] Missing AutoInteractor on {gameObject.name}", this);
            enabled = false;
        }

        if (attacker == null)
        {
            Debug.LogError($"[AIMovementBrain] Missing Attacker on {gameObject.name}", this);
            enabled = false;
        }

        if (entity == null)
        {
            Debug.LogError($"[AIMovementBrain] Missing Entity on {gameObject.name}", this);
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
            Debug.LogError($"[AIMovementBrain] Strategic target not assigned on {gameObject.name}", this);
            return;
        }

        if (movementController != null && movementController.enabled)
        {
            Debug.Log($"[AIMovementBrain] Moving towards {strategicTarget.name}");
            movementController.MoveTo(strategicTarget.transform);
        }
    }

    private void Update()
    {
        if (entity != null && entity.CurrentState != EntityState.Active)
        {
            return;
        }

        if (!IsMovementControllerValid())
        {
            return;
        }

        UpdateCurrentStateLogic();
        ExecuteMovementBehavior();
    }

    /// <summary>
    /// Checks if the movement controller is valid and enabled.
    /// </summary>
    /// <returns>True if the movement controller can be used, false otherwise.</returns>
    private bool IsMovementControllerValid()
    {
        return movementController != null && movementController.enabled;
    }

    /// <summary>
    /// Updates logic specific to the current AI movement state.
    /// </summary>
    private void UpdateCurrentStateLogic()
    {
        if (currentState == AIMovementState.EngagingTactical)
        {
            UpdateTacticalRange();
        }
    }

    /// <summary>
    /// Executes movement behavior based on the current AI state.
    /// </summary>
    private void ExecuteMovementBehavior()
    {
        switch (currentState)
        {
            case AIMovementState.EngagingTactical:
                HandleTacticalEngagement();
                break;

            case AIMovementState.MovingStrategic:
                HandleStrategicMovement();
                break;
        }
    }

    /// <summary>
    /// Handles movement behavior when engaging a tactical target.
    /// </summary>
    private void HandleTacticalEngagement()
    {
        if (tacticalTarget == null)
        {
            currentState = AIMovementState.MovingStrategic;
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, tacticalTarget.transform.position);
        
        if (IsWithinTacticalRange(distanceToTarget))
        {
            movementController.Stop();
        }
        else
        {

            movementController.MoveTo(tacticalTarget.transform);
        }
    }

    /// <summary>
    /// Handles movement behavior when moving towards the strategic target.
    /// </summary>
    private void HandleStrategicMovement()
    {
        if (strategicTarget != null)
        {
            movementController.MoveTo(strategicTarget.transform.position);
        }
    }

    /// <summary>
    /// Determines if the current distance is within tactical engagement range.
    /// </summary>
    /// <param name="distance">The distance to check.</param>
    /// <returns>True if within tactical range, false otherwise.</returns>
    private bool IsWithinTacticalRange(float distance)
    {
        return distance <= tacticalRange;
    }

    /// <summary>
    /// Handles the event when a new tactical target is acquired.
    /// </summary>
    private void HandleTargetAcquired(GameObject target)
    {
        tacticalTarget = target;
        currentState = AIMovementState.EngagingTactical;
        UpdateTacticalRange();
        Debug.Log($"[AIMovementBrain] Tactical target acquired: {target.name}");
    }

    /// <summary>
    /// Handles the event when a tactical target is lost.
    /// </summary>
    private void HandleTargetLost()
    {
        tacticalTarget = null;
        currentState = AIMovementState.MovingStrategic;
        Debug.Log($"[AIMovementBrain] Tactical target lost for {gameObject.name}");
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
