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
    private AIMovementState previousState = AIMovementState.MovingStrategic;
    private float tacticalRange = 1f;
    private bool wasWithinTacticalRange = false;

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
        if (strategicTarget == null && !TryGetComponent<AutoAssignStrategicTarget>(out var assigner))
        {
            Debug.LogError($"[AIMovementBrain] Strategic target not assigned on {gameObject.name}", this);
            return;
        }

        if (movementController != null && movementController.enabled)
        {
            Debug.Log($"[AIMovementBrain] Moving towards {strategicTarget.name}");
            movementController.MoveTo(strategicTarget.transform);
            currentState = AIMovementState.MovingStrategic;
            previousState = AIMovementState.EngagingTactical;
        }
    }

    private void Update()
    {
        if (entity != null && entity.CurrentState != EntityState.Active)
        {
            movementController.Stop();
            Debug.Log($"[AIMovementBrain] Entity {gameObject.name} is not active, stopping movement.", this);
            return;
        }

        if (!IsMovementControllerValid())
        {
            return;
        }

        UpdateCurrentStateLogic();
        
        // Only execute movement behavior when state changes or when tactical situation changes
        if (HasStateChanged() || (currentState == AIMovementState.EngagingTactical && HasTacticalSituationChanged()))
        {
            ExecuteMovementBehavior();
            previousState = currentState;
        }
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
    /// Checks if the AI movement state has changed since the last update.
    /// </summary>
    /// <returns>True if the state has changed, false otherwise.</returns>
    private bool HasStateChanged()
    {
        return currentState != previousState;
    }

    /// <summary>
    /// Checks if the tactical situation has changed (e.g., moved in/out of range).
    /// </summary>
    /// <returns>True if tactical situation changed, false otherwise.</returns>
    private bool HasTacticalSituationChanged()
    {
        if (tacticalTarget == null) return false;
        
        float distanceToTarget = Vector3.Distance(transform.position, tacticalTarget.transform.position);
        bool currentlyWithinRange = IsWithinTacticalRange(distanceToTarget);
        
        if (currentlyWithinRange != wasWithinTacticalRange)
        {
            wasWithinTacticalRange = currentlyWithinRange;
            return true;
        }
        
        return false;
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
            previousState = currentState;
            currentState = AIMovementState.MovingStrategic;
            wasWithinTacticalRange = false;
            return;
        }

        if (HasStateChanged())
        {
            Debug.Log("[AIMovementBrain] Transitioning to tactical engagement.", this);
        }

        // Use the cached range state from HasTacticalSituationChanged()
        if (wasWithinTacticalRange)
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
        if (HasStateChanged())
        {
            Debug.Log("[AIMovementBrain] Transitioning to strategic movement.", this);
        }
        
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
        previousState = currentState;
        currentState = AIMovementState.EngagingTactical;
        UpdateTacticalRange();
        
        // Initialize tactical range state
        float distanceToTarget = Vector3.Distance(transform.position, tacticalTarget.transform.position);
        wasWithinTacticalRange = IsWithinTacticalRange(distanceToTarget);
        
        Debug.Log($"[AIMovementBrain] Tactical target acquired: {target.name}");
    }

    /// <summary>
    /// Handles the event when a tactical target is lost.
    /// </summary>
    private void HandleTargetLost()
    {
        tacticalTarget = null;
        previousState = currentState;
        currentState = AIMovementState.MovingStrategic;
        wasWithinTacticalRange = false;
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
