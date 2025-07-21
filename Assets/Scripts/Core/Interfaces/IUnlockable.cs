using System;
using UnityEngine;

/// <summary>
/// Describes an entity that can be locked and unlocked.
/// </summary>
public interface IUnlockable
{
    /// <summary>
    /// Fired when this object is unlocked.
    /// Parameters: the unlocked GameObject.
    /// </summary>
    event Action<GameObject> OnUnlocked;

    /// <summary>
    /// Gets a value indicating whether the object is currently locked.
    /// </summary>
    bool IsLocked { get; }

    /// <summary>
    /// Unlocks the object.
    /// </summary>
    void Unlock(AutoInteractor interactor = null);

    /// <summary>
    /// Determines whether the object can be unlocked.
    /// </summary>
    /// <returns>True if unlocking is possible.</returns>
    bool CanUnlock();
}
