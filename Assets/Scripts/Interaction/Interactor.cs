using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Universal brain that automatically performs interactions based on a
/// <see cref="TargetScanner"/> perception system and an <see cref="Attacker"/>.
/// It chooses the best target and commands the Attacker according to the
/// assigned <see cref="InteractorProfileSO"/>.
/// </summary>
[RequireComponent(typeof(TargetScanner))]
[RequireComponent(typeof(Attacker))]
[RequireComponent(typeof(Faction))]
public class Interactor : MonoBehaviour
{
    [Header("Profile")]
    [Tooltip("Profile describing which interactions this interactor is allowed to perform.")]
    public InteractorProfileSO interactorProfile;

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

        if (interactorProfile == null)
        {
            Debug.LogError($"[Interactor] InteractorProfileSO is not assigned on {gameObject.name}", this);
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
        if (scanner == null || attacker == null || faction == null || interactorProfile == null)
        {
            return;
        }

        List<IInteractable> targets = scanner.GetTargetsInRange();
        GameObject bestTarget = DecideBestTarget(targets);

        if (bestTarget != null && interactorProfile.canAttack)
        {
            attacker.Engage(bestTarget);
        }
        else
        {
            attacker.Disengage();
        }
    }

    /// <summary>
    /// Selects the nearest hostile <see cref="IDestructible"/> target.
    /// </summary>
    /// <param name="targets">Potential targets detected by the scanner.</param>
    /// <returns>The best target GameObject or null if none found.</returns>
    private GameObject DecideBestTarget(List<IInteractable> targets)
    {
        GameObject bestTarget = null;
        float bestSqr = float.PositiveInfinity;

        foreach (IInteractable target in targets)
        {
            if (target == null) { continue; }
            if (!(target is Component component)) { continue; }

            GameObject targetGO = component.gameObject;
            Faction targetFaction = targetGO.GetComponent<Faction>();
            if (targetFaction != null && targetFaction.CurrentFaction == faction.CurrentFaction)
            {
                // Skip friendly targets
                continue;
            }

            bool destructible = targetGO.GetComponent<IDestructible>() != null || targetGO.GetComponent<Health>() != null;
            if (!destructible)
            {
                continue;
            }

            float sqr = (targetGO.transform.position - transform.position).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                bestTarget = targetGO;
            }
        }

        return bestTarget;
    }
}
