using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Governs NPC behaviour by combining movement, targeting and attacking logic.
/// Uses <see cref="TargetScanner"/> as the eyes, <see cref="MovementController"/>
/// as the legs and <see cref="Attacker"/> as the arms of the AI.
/// </summary>
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(TargetScanner))]
[RequireComponent(typeof(Attacker))]
public class AIController : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Profile describing how this AI evaluates targets.")]
    public AIProfileSO aiProfile;

    [Header("Targets")]
    [Tooltip("Ultimate destination when no other target is available.")]
    public Transform strategicTarget;

    private MovementController movementController;
    private TargetScanner scanner;
    private Attacker attacker;
    private Faction faction;
    private GameObject currentTarget;

    private enum AIState
    {
        Idle,
        Engaging
    }

    private AIState currentState = AIState.Idle;

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        scanner = GetComponent<TargetScanner>();
        attacker = GetComponent<Attacker>();
        faction = GetComponent<Faction>();

        if (movementController == null)
        {
            Debug.LogError($"[AIController] Missing MovementController on {gameObject.name}", this);
            enabled = false;
        }

        if (scanner == null)
        {
            Debug.LogError($"[AIController] Missing TargetScanner on {gameObject.name}", this);
            enabled = false;
        }

        if (attacker == null)
        {
            Debug.LogError($"[AIController] Missing Attacker on {gameObject.name}", this);
            enabled = false;
        }
    }

    private void Update()
    {
        Tick();
    }

    /// <summary>
    /// Called every frame to update AI behaviour.
    /// </summary>
    public void Tick()
    {
        HandleTargeting();
        ActOnState();
    }

    /// <summary>
    /// Determines the best available target and updates the current state.
    /// </summary>
    private void HandleTargeting()
    {
        if (scanner == null) { return; }

        List<IInteractable> targets = scanner.GetTargetsInRange();
        GameObject bestTarget = DecideBestTarget(targets);

        if (bestTarget != currentTarget)
        {
            currentTarget = bestTarget;
            if (currentTarget != null)
            {
                Debug.Log($"[AIController] New target acquired: {currentTarget.name}", this);
            }
        }

        currentState = currentTarget != null ? AIState.Engaging : AIState.Idle;
    }

    /// <summary>
    /// Executes actions based on the current state.
    /// </summary>
    private void ActOnState()
    {
        switch (currentState)
        {
            case AIState.Engaging:
                EngageTargetState();
                break;
            case AIState.Idle:
                IdleState();
                break;
        }
    }

    /// <summary>
    /// Moves toward and attacks the current target.
    /// </summary>
    private void EngageTargetState()
    {
        if (currentTarget == null)
        {
            currentState = AIState.Idle;
            return;
        }

        attacker.Engage(currentTarget);
        movementController.MoveTo(currentTarget.transform.position);
    }

    /// <summary>
    /// Handles behaviour when no immediate target is available.
    /// </summary>
    private void IdleState()
    {
        attacker.Disengage();

        if (strategicTarget != null)
        {
            movementController.MoveTo(strategicTarget.position);
        }
        else
        {
            movementController.Stop();
        }
    }

    /// <summary>
    /// Selects the most appropriate GameObject from a list of interactable targets.
    /// Hostile and nearby targets are prioritized.
    /// </summary>
    /// <param name="targets">Collection of interactable targets detected by the scanner.</param>
    /// <returns>The chosen target GameObject or null if no suitable target exists.</returns>
    private GameObject DecideBestTarget(List<IInteractable> targets)
    {
        GameObject bestTarget = null;
        float bestScore = float.MinValue;

        foreach (IInteractable interactable in targets)
        {
            if (interactable == null) { continue; }

            Component comp = interactable as Component;
            if (comp == null) { continue; }

            GameObject targetGO = comp.gameObject;

            Faction targetFaction = targetGO.GetComponent<Faction>();
            if (faction != null && targetFaction != null && targetFaction.CurrentFaction == faction.CurrentFaction)
            {
                // Ignore friendly targets
                continue;
            }

            float distance = Vector3.Distance(transform.position, comp.transform.position);
            bool isDestructible = targetGO.GetComponent<IDestructible>() != null || targetGO.GetComponent<Health>() != null;

            float score = 0f;
            if (aiProfile != null)
            {
                if (isDestructible)
                {
                    score += aiProfile.destructibleBonus;
                }
                score -= distance * aiProfile.distanceWeight;
            }
            else
            {
                // Fallback scoring if no profile provided
                score -= distance;
                if (isDestructible) { score += 10f; }
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = targetGO;
            }
        }

        return bestTarget;
    }
}
