using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles setup of an archer unit when it is placed on (or owned by) a tower:
/// - Disables movement so the archer stays on the tower.
/// - Applies data-driven damage & cooldown multipliers via runtime-cloned attack definitions/profile.
/// - Ensures the archer can rotate towards targets.
/// - Ensures the archer auto assigns faction from a parent FactionParent.
/// </summary>
public class ArcherTowerHandler : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Profile defining how this tower modifies its archer.")]
    public ArcherTowerProfileSO towerProfile;

    [Header("Archer References")]
    [Tooltip("Explicit reference to the archer's Attacker component. If left empty, will search children.")]
    public Attacker archerAttacker;

    [Tooltip("Optional explicit movement controller reference. If null, will search on the archer.")]
    public MovementController archerMovementController;

    /// <summary>
    /// Original attacker profile reference (not modified). Stored for potential future restoration.
    /// </summary>
    private AttackerProfileSO originalProfile;

    /// <summary>
    /// Runtime cloned profile with modified attacks.
    /// </summary>
    private AttackerProfileSO runtimeModifiedProfile;

    private bool applied;

    void Awake()
    {
        if (towerProfile == null)
        {
            Debug.LogError("[ArcherTowerHandler] TowerProfile not assigned.", this);
        }
    }

    void Start()
    {
        Apply();
    }

    /// <summary>
    /// Applies all tower effects to the archer (idempotent).
    /// </summary>
    public void Apply()
    {
        if (applied) return;
        ResolveReferences();

        if (archerAttacker == null)
        {
            Debug.LogError("[ArcherTowerHandler] No Attacker found to apply tower effects.", this);
            return;
        }

        if (towerProfile == null)
        {
            Debug.LogWarning("[ArcherTowerHandler] Missing tower profile; skipping stat modifications.", this);
        }
        else
        {
            ApplyStatModifiers();
        }

        DisableMovement();
        EnsureRotateTowardsComponent();
        EnsureAutoAssignFaction();

        applied = true;
    }

    void ResolveReferences()
    {
        if (archerAttacker == null)
        {
            archerAttacker = GetComponentInChildren<Attacker>();
            if (archerAttacker == null)
            {
                Debug.LogWarning("[ArcherTowerHandler] Could not auto-find Attacker in children.", this);
            }
        }

        if (archerMovementController == null && archerAttacker != null)
        {
            archerMovementController = archerAttacker.GetComponent<MovementController>();
        }
    }

    void DisableMovement()
    {
        if (archerMovementController != null)
        {
            archerMovementController.Stop();
            archerMovementController.enabled = false;
        }

        // Disable NavMeshAgent if present to ensure no residual pathing
        if (archerAttacker != null && archerAttacker.TryGetComponent<NavMeshAgent>(out var agent))
        {
            agent.isStopped = true;
            agent.enabled = false;
        }
    }

    void EnsureRotateTowardsComponent()
    {
        if (archerAttacker == null) return;
        if (archerAttacker.GetComponent<RotateTowardsBestTarget>() == null)
        {
            archerAttacker.gameObject.AddComponent<RotateTowardsBestTarget>();
        }
    }

    void EnsureAutoAssignFaction()
    {
        if (archerAttacker == null) return;
        if (archerAttacker.GetComponent<AutoAssignFaction>() == null)
        {
            archerAttacker.gameObject.AddComponent<AutoAssignFaction>();
        }
    }

    void ApplyStatModifiers()
    {
        if (archerAttacker.attackerProfile == null)
        {
            Debug.LogWarning("[ArcherTowerHandler] Archer has no attackerProfile; cannot apply modifiers.", this);
            return;
        }

        originalProfile = archerAttacker.attackerProfile;

        // Create a new AttackerProfileSO instance (not an asset) to avoid mutating shared asset.
        runtimeModifiedProfile = ScriptableObject.CreateInstance<AttackerProfileSO>();
        runtimeModifiedProfile.name = originalProfile.name + "_TowerRuntime";

        runtimeModifiedProfile.attacks = new List<AttackDefinitionSO>(originalProfile.attacks.Count);

        foreach (var attack in originalProfile.attacks)
        {
            if (attack == null)
            {
                runtimeModifiedProfile.attacks.Add(null);
                continue;
            }

            // Clone the attack definition (keeps subclass type e.g., RangedAttackDefinitionSO)
            var cloned = Instantiate(attack);

            // Apply multipliers
            cloned.damage *= Mathf.Max(0f, towerProfile.archerDamageMultiplier);
            cloned.cooldown *= Mathf.Max(0.01f, towerProfile.archerCooldownMultiplier);

            runtimeModifiedProfile.attacks.Add(cloned);
        }

        // Reinitialize attacker with modified profile
        archerAttacker.Initialize(runtimeModifiedProfile);
    }

    void OnDestroy()
    {
        // Optional cleanup of runtime ScriptableObjects (Unity auto-cleans on scene unload, but explicit destroy helps). 
        if (runtimeModifiedProfile != null)
        {
            foreach (var atk in runtimeModifiedProfile.attacks)
            {
                if (atk != null)
                {
                    Destroy(atk);
                }
            }
            Destroy(runtimeModifiedProfile);
        }
    }
}
