using UnityEngine;

/// <summary>
/// Central component that controls building progression and lock state.
/// </summary>
[DisallowMultipleComponent]
public class Building : MonoBehaviour, IUpgradable, IUnlockable
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
    /// Fired when the building is upgraded.
    /// Parameters: the upgraded GameObject and new level.
    /// </summary>
    public event System.Action<GameObject, int> OnUpgraded;

    /// <summary>
    /// Fired when the building is unlocked.
    /// Parameters: the unlocked GameObject.
    /// </summary>
    public event System.Action<GameObject> OnUnlocked;

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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public void Upgrade(AutoInteractor interactor = null)
    {
        if (_ownerSlot == null)
        {
            Debug.LogError($"[Building] Upgrade called but owner slot is null on {gameObject.name}.", this);
            return;
        }

        _ownerSlot.UpgradeBuilding();
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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
        GameEvents.TriggerOnObjectUnlocked(gameObject);
        OnUnlocked?.Invoke(gameObject);
        Debug.Log($"[Building] {gameObject.name} unlocked.", this);
    }
}
