/// <summary>
/// Provides upgrade functionality for an object.
/// </summary>
public interface IUpgradable
{
    /// <summary>
    /// Determines whether the object can currently be upgraded.
    /// </summary>
    /// <returns>True if an upgrade is allowed.</returns>
    bool CanUpgrade();

    /// <summary>
    /// Executes the upgrade process.
    /// </summary>
    void Upgrade();

    /// <summary>
    /// Gets the current upgrade level of the object.
    /// </summary>
    int CurrentLevel { get; }
}
