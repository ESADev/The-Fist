using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scans the surrounding area for objects implementing <see cref="IInteractable"/>
/// according to parameters defined in a <see cref="TargetScannerSO"/> profile.
/// Acts as the "eyes" of NPCs and automated players.
/// </summary>
public class TargetScanner : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Profile defining scan radius and update frequency.")]
    [HideInInspector] public TargetScannerSO scannerProfile;

    /// <summary>
    /// List of interactable targets currently detected within range.
    /// </summary>
    private readonly List<Entity> targetsInRange = new List<Entity>();

    private float nextScanTime;

    private void Awake()
    {
        // Don't check for scannerProfile here - it may be assigned by Entity during initialization
    }

    public void Initialize(TargetScannerSO profile)
    {
        scannerProfile = profile;

        if (scannerProfile == null)
        {
            Debug.LogError($"[TargetScanner] Scanner profile is not assigned on {gameObject.name}", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (scannerProfile == null) { return; }

        if (Time.time >= nextScanTime)
        {
            ScanForTargets();
            nextScanTime = Time.time + scannerProfile.scanFrequency;
        }
    }

    /// <summary>
    /// Performs a physics overlap to find all interactables within range.
    /// </summary>
    private void ScanForTargets()
    {
        targetsInRange.Clear();

        Collider[] hits = Physics.OverlapSphere(transform.position, scannerProfile.detectionRadius);
        foreach (Collider hit in hits)
        {
            var entity = hit.GetComponentInParent<Entity>();
            if (entity == null || gameObject.GetComponentInParent<Entity>() == entity || entity.Health.IsDead) { continue; }

            targetsInRange.Add(entity);
        }

        Debug.Log($"[TargetScanner] {targetsInRange.Count} targets detected by {gameObject.name}");
    }


    /// <summary>
    /// Returns all currently detected interactable targets.
    /// </summary>
    /// <returns>List of targets in range.</returns>
    public List<Entity> GetTargetsInRange()
    {
        return new List<Entity>(targetsInRange);
    }

    /// <summary>
    /// Returns the closest interactable target regardless of type.
    /// </summary>
    /// <returns>The nearest target or null if none exist.</returns>
    public Entity GetNearestTarget()
    {
        float closestSqr = float.PositiveInfinity;
        Entity closest = null;
        foreach (var target in targetsInRange)
        {
            if (target == null) { continue; }
            Component c = target as Component;
            if (c == null) { continue; }
            float sqr = (c.transform.position - transform.position).sqrMagnitude;
            if (sqr < closestSqr)
            {
                closestSqr = sqr;
                closest = target;
            }
        }
        return closest;
    }

    /// <summary>
    /// Returns the nearest target of a specific interactable type.
    /// </summary>
    /// <typeparam name="T">Interface type derived from <see cref="IInteractable"/>.</typeparam>
    /// <returns>The nearest target implementing T, or null if none found.</returns>
    public T GetNearestTarget<T>() where T : class, IInteractable
    {
        float closestSqr = float.PositiveInfinity;
        T closest = null;
        foreach (var target in targetsInRange)
        {
            if (target is T typedTarget)
            {
                Component c = typedTarget as Component;
                if (c == null) { continue; }
                float sqr = (c.transform.position - transform.position).sqrMagnitude;
                if (sqr < closestSqr)
                {
                    closestSqr = sqr;
                    closest = typedTarget;
                }
            }
        }
        return closest;
    }
}
