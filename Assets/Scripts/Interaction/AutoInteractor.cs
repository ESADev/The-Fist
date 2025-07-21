using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Universal brain that automatically performs interactions based on a
/// <see cref="TargetScanner"/> perception system and an <see cref="Attacker"/>.
/// It chooses the best target and commands the Attacker according to the
/// assigned <see cref="InteractorProfileSO"/>.
/// </summary>
[RequireComponent(typeof(TargetScanner))]
[RequireComponent(typeof(Attacker))]
[RequireComponent(typeof(Faction))]

public class AutoInteractor : MonoBehaviour
{
    [Header("Profile")]
    [Tooltip("Profile describing which interactions this interactor is allowed to perform.")]
    [HideInInspector] public InteractorProfileSO interactorProfile;

    /// <summary>
    /// Fired when a new tactical target has been acquired.
    /// </summary>
    public event Action<GameObject> OnNewTargetAcquired;

    /// <summary>
    /// Fired when the current tactical target is lost.
    /// </summary>
    public event Action OnTargetLost;

    private GameObject currentTarget;

    private TargetScanner scanner;
    private Attacker attacker;
    private Faction faction;
    private Entity entity;

    private void Awake()
    {
        scanner = GetComponent<TargetScanner>();
        attacker = GetComponent<Attacker>();
        faction = GetComponent<Faction>();
        entity = GetComponent<Entity>();

        if (scanner == null)
        {
            Debug.LogError($"[AutoInteractor] Missing TargetScanner on {gameObject.name}", this);
            enabled = false;
            return;
        }

        if (attacker == null)
        {
            Debug.LogError($"[AutoInteractor] Missing Attacker on {gameObject.name}", this);
            enabled = false;
            return;
        }

        if (faction == null)
        {
            Debug.LogError($"[AutoInteractor] Missing Faction on {gameObject.name}", this);
            enabled = false;
            return;
        }

        if (entity == null)
        {
            Debug.LogError($"[AutoInteractor] Missing Entity on {gameObject.name}", this);
            enabled = false;
            return;
        }

        // Don't check for interactorProfile here - it may be assigned by Entity during initialization
    }

    /// <summary>
    /// Initializes the AutoInteractor with a specific InteractorProfileSO.
    /// /// This should be called before the first update to ensure proper functionality.
    /// </summary>
    /// <param name="profile">The InteractorProfileSO to use for this interactor.</param>
    /// <exception cref="ArgumentNullException">Thrown if profile is null.</exception>
    public void Initialize(InteractorProfileSO profile)
    {
        interactorProfile = profile;

        if (interactorProfile == null)
        {
            Debug.LogError($"[AutoInteractor] InteractorProfileSO is not assigned on {gameObject.name}", this);
            enabled = false;
        }
    }

    private void Update()
    {
        Tick();
    }

    /// <summary>
    /// Main update loop for deciding and performing automatic actions.
    /// </summary>
    public void Tick()
    {
        if (entity != null && entity.CurrentState != EntityState.Active)
        {
            return;
        }

        if (scanner == null || attacker == null || faction == null || interactorProfile == null)
        {
            return;
        }

        List<Entity> targets = scanner.GetTargetsInRange();
        Entity bestTarget = DecideBestTarget(targets);

        HandleTargetChange(bestTarget != null ? bestTarget.gameObject : null);

        HandleTargetInteraction(bestTarget);
    }

    /// <summary>
    /// Handles updating <see cref="currentTarget"/> and broadcasting the
    /// appropriate events when the target changes.
    /// </summary>
    /// <param name="newTarget">The newly selected target or null.</param>
    private void HandleTargetChange(GameObject newTarget)
    {
        if (newTarget == currentTarget)
        {
            return;
        }

        if (newTarget != null)
        {
            currentTarget = newTarget;
            Debug.Log($"[AutoInteractor] New target acquired: {currentTarget.name}", this);
            OnNewTargetAcquired?.Invoke(currentTarget);
        }
        else
        {
            if (currentTarget != null)
            {
                Debug.Log($"[AutoInteractor] Target lost by {gameObject.name}", this);
                OnTargetLost?.Invoke();
            }
            currentTarget = null;
        }
    }

    /// <summary>
    /// Handles interaction with the best target based on the interactor profile.  
    /// If the target is hostile, it will engage with the Attacker.
    /// If the target is friendly, it will attempt to perform other interactions iteratively.
    /// </summary>
    void HandleTargetInteraction(Entity bestTarget)
    {
        if (bestTarget != null)
        {
            if (interactorProfile.canAttack && bestTarget.Faction.CurrentFaction != faction.CurrentFaction)
            {
                attacker.Engage(bestTarget.gameObject);
            }
            else
            {
                attacker.Disengage();
                if (interactorProfile.canInteract && bestTarget.GetComponent<IInteractable>() != null)
                {
                    // Attempt to interact with the target
                    bestTarget.GetComponent<IInteractable>().Interact(this);
                }
                else if (interactorProfile.canUnlock && bestTarget.GetComponent<IUnlockable>() != null)
                {
                    // Attempt to unlock the target
                    bestTarget.GetComponent<IUnlockable>().Unlock(this);
                }
                else if (interactorProfile.canUpgrade && bestTarget.GetComponent<IUpgradable>() != null)
                {
                    // Attempt to upgrade the target
                    bestTarget.GetComponent<IUpgradable>().Upgrade(this);
                }
                else if (interactorProfile.canCollect && bestTarget.GetComponent<ICollectible>() != null)
                {
                    // Attempt to collect the target
                    bestTarget.GetComponent<ICollectible>().Collect(this);
                }
                else
                {
                    Debug.Log($"[AutoInteractor] No valid interaction available for {bestTarget.name} on {gameObject.name}", this);
                }
            }
        }
        else
        {
            attacker.Disengage();
        }
    }

    /// <summary>
    /// Selects the nearest hostile <see cref="IDestructible"/> target. If no hostile targets are found,
    /// selects the nearest friendly <see cref="IDestructible"/> target.
    /// </summary>
    /// <param name="targets">Potential targets detected by the scanner.</param>
    /// <returns>The best target GameObject or null if none found.</returns>
    private Entity DecideBestTarget(List<Entity> targets)
    {
        Entity bestEnemyTarget = null;
        float bestEnemySqr = float.PositiveInfinity;

        Entity bestFriendTarget = null;
        float bestFriendSqr = float.PositiveInfinity;

        foreach (Entity target in targets)
        {
            if (target == null) { continue; }
            if (!(target is Component component)) { continue; }

            Faction targetFaction = target.Faction;
            bool isFriend = targetFaction != null && targetFaction.CurrentFaction == faction.CurrentFaction;

            bool destructible = target.GetComponent<IDestructible>() != null || target.Health != null;
            if (!destructible)
            {
                continue;
            }

            float sqr = (target.transform.position - transform.position).sqrMagnitude;

            if (!isFriend)
            {
                if (sqr < bestEnemySqr)
                {
                    bestEnemySqr = sqr;
                    bestEnemyTarget = target;
                }
            }
            else
            {
                if (sqr < bestFriendSqr)
                {
                    bestFriendSqr = sqr;
                    bestFriendTarget = target;
                }
            }
        }

        if (bestEnemyTarget != null)
        {
            return bestEnemyTarget;
        }
        else if (bestFriendTarget != null && interactorProfile.canInteract)
        {
            return bestFriendTarget;
        }
        else
        {
            return null; // No valid targets found
        }
    }
}
