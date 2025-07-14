/// <summary>
/// Describes an entity that can be locked and unlocked.
/// </summary>
public interface IUnlockable
{
    /// <summary>
    /// Gets a value indicating whether the object is currently locked.
    /// </summary>
    bool IsLocked { get; }

    /// <summary>
    /// Unlocks the object.
    /// </summary>
    void Unlock(Interactor interactor = null);

    /// <summary>
    /// Determines whether the object can be unlocked.
    /// </summary>
    /// <returns>True if unlocking is possible.</returns>
    bool CanUnlock();
}
