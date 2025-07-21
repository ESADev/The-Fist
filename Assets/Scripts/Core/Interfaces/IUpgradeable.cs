using System;
using UnityEngine;

/// <summary>
/// Provides upgrade functionality for an object.
/// </summary>
public interface IUpgradeable
{
    /// <summary>
    /// Fired when an upgrade is performed on this object.
    /// Parameters: the upgraded GameObject, the new level.
    /// </summary>
    event Action<GameObject, int> OnUpgraded;

    /// <summary>
    /// Determines whether the object can currently be upgraded.
    /// </summary>
    /// <returns>True if an upgrade is allowed.</returns>
    bool CanUpgrade();

    /// <summary>
    /// Executes the upgrade process.
    /// </summary>
    void Upgrade(AutoInteractor interactor = null);

    /// <summary>
    /// Gets the current upgrade level of the object.
    /// </summary>
    int CurrentLevel { get; }
}
