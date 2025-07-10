using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles automatic interactions for its GameObject using attached systems
/// such as <see cref="TargetScanner"/> and <see cref="Attacker"/>.
/// This component decides at runtime which interaction to perform with
/// nearby objects.
/// </summary>
[RequireComponent(typeof(TargetScanner))]
[RequireComponent(typeof(Attacker))]
[RequireComponent(typeof(Faction))]
public class Interactor : MonoBehaviour
{
    private TargetScanner scanner;
    private Attacker attacker;
    private Faction faction;

    private void Awake()
    {
        scanner = GetComponent<TargetScanner>();
        attacker = GetComponent<Attacker>();
        faction = GetComponent<Faction>();

        if (scanner == null)
        {
            Debug.LogError($"[Interactor] Missing TargetScanner on {gameObject.name}", this);
            enabled = false;
            return;
        }

        if (attacker == null)
        {
            Debug.LogError($"[Interactor] Missing Attacker on {gameObject.name}", this);
            enabled = false;
            return;
        }

        if (faction == null)
        {
            Debug.LogError($"[Interactor] Missing Faction on {gameObject.name}", this);
            enabled = false;
        }
    }

    private void Update()
    {
        DecideAndPerformAutomaticAction();
    }

    /// <summary>
    /// Determines the best interaction for the current frame and executes it.
    /// Attacking hostile targets is prioritized over all other actions.
    /// </summary>
    private void DecideAndPerformAutomaticAction()
    {
        if (scanner == null || attacker == null || faction == null)
        {
            return;
        }

        List<IInteractable> targets = scanner.GetTargetsInRange();
        if (targets.Count == 0)
        {
            attacker.Disengage();
            return;
        }

        // 1. Attempt to attack hostile targets.
        foreach (IInteractable target in targets)
        {
            if (target == null) { continue; }

            Component component = target as Component;
            if (component == null) { continue; }

            GameObject targetGO = component.gameObject;
            Faction targetFaction = targetGO.GetComponent<Faction>();
            FactionType targetType = targetFaction != null ? targetFaction.CurrentFaction : FactionType.Neutral;

            if (targetType != faction.CurrentFaction && (targetGO.GetComponent<IDestructible>() != null || targetGO.GetComponent<Health>() != null))
            {
                attacker.Engage(targetGO);
                Debug.Log($"[Interactor] Attacking {targetGO.name}");
                return;
            }
        }

        // 2. Interact with friendly objects.
        foreach (IInteractable target in targets)
        {
            if (target == null) { continue; }

            Component component = target as Component;
            if (component == null) { continue; }

            GameObject targetGO = component.gameObject;
            Faction targetFaction = targetGO.GetComponent<Faction>();
            FactionType targetType = targetFaction != null ? targetFaction.CurrentFaction : FactionType.Neutral;

            if (targetType == faction.CurrentFaction)
            {
                if (targetGO.TryGetComponent<IUpgradable>(out var upgradable) && upgradable.CanUpgrade())
                {
                    upgradable.Upgrade();
                    Debug.Log($"[Interactor] Upgraded {targetGO.name}");
                    return;
                }

                if (targetGO.TryGetComponent<IUnlockable>(out var unlockable) && unlockable.CanUnlock())
                {
                    unlockable.Unlock();
                    Debug.Log($"[Interactor] Unlocked {targetGO.name}");
                    return;
                }
            }

            if (targetGO.TryGetComponent<ICollectible>(out var collectible))
            {
                collectible.Collect(this);
                Debug.Log($"[Interactor] Collected {targetGO.name}");
                return;
            }
        }

        attacker.Disengage();
    }
}
