using UnityEngine;

/// <summary>
/// Central component that controls building progression and lock state.
/// </summary>
[DisallowMultipleComponent]
public class Building : MonoBehaviour
{
    [Header("Building Data")]
    /// <summary>
    /// ScriptableObject describing all upgrade levels for this building.
    /// </summary>
    [Tooltip("Data defining this building's upgrade path.")]
    public BuildingDataSO buildingData;

    [Header("State")]
    /// <summary>
    /// The building's current level starting at 1.
    /// </summary>
    [Tooltip("Current level of the building.")]
    public int currentLevel = 1;

    private bool isLocked = true;

    /// <summary>
    /// Slot that owns this building instance.
    /// </summary>
    private BuildingSlot _ownerSlot;


    /// <summary>
    /// Gets the current upgrade level of this building.
    /// </summary>
    public int CurrentLevel => currentLevel;

    /// <summary>
    /// Gets a value indicating whether the building is locked.
    /// </summary>
    public bool IsLocked => isLocked;

    /// <summary>
    /// Assigns the owning <see cref="BuildingSlot"/> that manages this building.
    /// </summary>
    /// <param name="owner">The owning slot.</param>
    public void SetOwner(BuildingSlot owner)
    {
        _ownerSlot = owner;
    }

    private void Awake()
    {
        if (buildingData == null)
        {
            Debug.LogError("[Building] BuildingDataSO not assigned.", this);
        }
    }

    /// <summary>
    /// Determines whether this building can be upgraded.
    /// </summary>
    /// <returns>True if an upgrade is allowed.</returns>
    public bool CanUpgrade()
    {
        if (buildingData == null)
        {
            return false;
        }

        if (currentLevel >= buildingData.levels.Count)
        {
            return false;
        }

        BuildingLevelData nextLevel = buildingData.levels[currentLevel];
        return ResourceManager.Instance != null && ResourceManager.Instance.CanAfford(ResourceType.Gold, nextLevel.cost);
    }

    /// <summary>
    /// Delegates the upgrade request to the owning <see cref="BuildingSlot"/>.
    /// </summary>
    /// <param name="interactor">Interactor requesting the upgrade.</param>
    public void Upgrade(AutoInteractor interactor = null)
    {
        if (_ownerSlot == null)
        {
            Debug.LogError($"[Building] Upgrade called but owner slot is null on {gameObject.name}.", this);
            return;
        }

        _ownerSlot.UpgradeBuilding();
    }

    /// <summary>
    /// Determines whether this building can be unlocked.
    /// </summary>
    /// <returns>True if unlocking is possible.</returns>
    public bool CanUnlock()
    {
        if (buildingData == null || !isLocked)
        {
            return false;
        }

        BuildingLevelData levelData = buildingData.levels.Count > 0 ? buildingData.levels[0] : null;
        if (levelData == null)
        {
            Debug.LogError("[Building] Level data missing.", this);
            return false;
        }

        return ResourceManager.Instance != null && ResourceManager.Instance.CanAfford(ResourceType.Gold, levelData.cost);
    }

    /// <summary>
    /// Unlocks the building if possible.
    /// </summary>
    /// <param name="interactor">Interactor requesting the unlock.</param>
    public void Unlock(AutoInteractor interactor = null)
    {
        if (!CanUnlock())
        {
            Debug.LogWarning($"[Building] Cannot unlock {gameObject.name}.", this);
            return;
        }

        BuildingLevelData levelData = buildingData.levels[0];
        ResourceManager.Instance.SpendResource(ResourceType.Gold, levelData.cost);
        isLocked = false;
        Debug.Log($"[Building] {gameObject.name} unlocked.", this);
    }
}
