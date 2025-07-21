using System;
using UnityEngine;

/// <summary>
/// Manages construction and upgrades for a single building slot.
/// </summary>
[DisallowMultipleComponent]
public class BuildingSlot : MonoBehaviour, IUpgradable, IUnlockable
{
    /// <summary>
    /// Currently instantiated building occupying this slot.
    /// </summary>
    private GameObject _currentBuildingInstance;

    /// <summary>
    /// Data defining the upgrade path for the current building.
    /// </summary>
    private BuildingDataSO _currentBuildingData;

    /// <summary>
    /// Fired when the building managed by this slot is upgraded.
    /// Parameters: the upgraded GameObject and the new level.
    /// </summary>
    public event Action<GameObject, int> OnUpgraded;

    /// <summary>
    /// Fired when the building managed by this slot is unlocked.
    /// Parameters: the unlocked GameObject.
    /// </summary>
    public event Action<GameObject> OnUnlocked;

    /// <summary>
    /// Gets the current upgrade level of the building occupying this slot.
    /// </summary>
    public int CurrentLevel
    {
        get
        {
            if (_currentBuildingInstance == null)
            {
                return 0;
            }

            Building building = _currentBuildingInstance.GetComponent<Building>();
            return building != null ? building.CurrentLevel : 0;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the building occupying this slot is locked.
    /// </summary>
    public bool IsLocked
    {
        get
        {
            if (_currentBuildingInstance == null)
            {
                return true;
            }

            Building building = _currentBuildingInstance.GetComponent<Building>();
            return building == null || building.IsLocked;
        }
    }

    /// <summary>
    /// Builds the specified building in this slot if empty.
    /// </summary>
    /// <param name="buildingToBuild">Definition of the building to construct.</param>
    public void Build(BuildingDataSO buildingToBuild)
    {
        if (_currentBuildingInstance != null)
        {
            Debug.LogWarning($"[BuildingSlot] Slot already occupied on {gameObject.name}.", this);
            return;
        }

        if (buildingToBuild == null || buildingToBuild.levels.Count == 0)
        {
            Debug.LogError("[BuildingSlot] Invalid BuildingDataSO supplied to Build().", this);
            return;
        }

        _currentBuildingData = buildingToBuild;

        BuildingLevelData levelData = buildingToBuild.levels[0];
        if (levelData.prefab == null)
        {
            Debug.LogError("[BuildingSlot] Level 1 prefab missing in BuildingDataSO.", this);
            return;
        }

        _currentBuildingInstance = Instantiate(levelData.prefab, transform.position, transform.rotation, transform);

        Building buildingComponent = _currentBuildingInstance.GetComponent<Building>();
        if (buildingComponent == null)
        {
            Debug.LogError("[BuildingSlot] Instantiated prefab has no Building component.", this);
            Destroy(_currentBuildingInstance);
            _currentBuildingInstance = null;
            return;
        }

        buildingComponent.buildingData = buildingToBuild;
        buildingComponent.currentLevel = 1;
        buildingComponent.SetOwner(this);

        Debug.Log($"[BuildingSlot] Built {levelData.prefab.name} in slot {gameObject.name}.", this);
    }

    /// <summary>
    /// Determines whether the building occupying this slot can be upgraded.
    /// </summary>
    /// <returns>True if an upgrade is possible.</returns>
    public bool CanUpgrade()
    {
        if (_currentBuildingInstance == null)
        {
            return false;
        }

        Building building = _currentBuildingInstance.GetComponent<Building>();
        return building != null && building.CanUpgrade();
    }

    /// <summary>
    /// Executes an upgrade on the building occupying this slot.
    /// </summary>
    /// <param name="interactor">Interactor requesting the upgrade.</param>
    public void Upgrade(AutoInteractor interactor = null)
    {
        UpgradeBuilding();
    }

    /// <summary>
    /// Attempts to upgrade the building occupying this slot.
    /// </summary>
    public void UpgradeBuilding()
    {
        if (_currentBuildingInstance == null)
        {
            Debug.LogWarning($"[BuildingSlot] No building present to upgrade on {gameObject.name}.", this);
            return;
        }

        Building currentBuilding = _currentBuildingInstance.GetComponent<Building>();
        if (currentBuilding == null)
        {
            Debug.LogError("[BuildingSlot] Current building instance missing Building component.", this);
            return;
        }

        if (!currentBuilding.CanUpgrade())
        {
            Debug.LogWarning($"[BuildingSlot] Cannot upgrade building on {gameObject.name}.", this);
            return;
        }

        int nextLevelIndex = currentBuilding.CurrentLevel;
        if (_currentBuildingData == null || nextLevelIndex >= _currentBuildingData.levels.Count)
        {
            Debug.LogWarning("[BuildingSlot] Upgrade data missing or invalid.", this);
            return;
        }

        BuildingLevelData nextLevel = _currentBuildingData.levels[nextLevelIndex];

        if (ResourceManager.Instance == null)
        {
            Debug.LogError("[BuildingSlot] ResourceManager instance not found.", this);
            return;
        }

        if (!ResourceManager.Instance.SpendResource(ResourceType.Gold, nextLevel.cost))
        {
            Debug.LogWarning("[BuildingSlot] Unable to spend resources for upgrade.", this);
            return;
        }

        GameEvents.TriggerOnObjectUpgraded(_currentBuildingInstance, currentBuilding.CurrentLevel + 1);
        OnUpgraded?.Invoke(_currentBuildingInstance, currentBuilding.CurrentLevel + 1);

        Destroy(_currentBuildingInstance);

        _currentBuildingInstance = Instantiate(nextLevel.prefab, transform.position, transform.rotation, transform);
        Building newBuilding = _currentBuildingInstance.GetComponent<Building>();
        if (newBuilding == null)
        {
            Debug.LogError("[BuildingSlot] Upgraded prefab missing Building component.", this);
            Destroy(_currentBuildingInstance);
            _currentBuildingInstance = null;
            return;
        }

        newBuilding.buildingData = _currentBuildingData;
        newBuilding.currentLevel = nextLevelIndex + 1;
        newBuilding.SetOwner(this);

        Debug.Log($"[BuildingSlot] Upgraded building in slot {gameObject.name} to level {newBuilding.currentLevel}.", this);
    }

    /// <summary>
    /// Determines whether the building occupying this slot can be unlocked.
    /// </summary>
    /// <returns>True if unlocking is possible.</returns>
    public bool CanUnlock()
    {
        if (_currentBuildingInstance == null)
        {
            return false;
        }

        Building building = _currentBuildingInstance.GetComponent<Building>();
        return building != null && building.CanUnlock();
    }

    /// <summary>
    /// Unlocks the building occupying this slot if possible.
    /// </summary>
    /// <param name="interactor">Interactor requesting the unlock.</param>
    public void Unlock(AutoInteractor interactor = null)
    {
        if (_currentBuildingInstance == null)
        {
            Debug.LogWarning($"[BuildingSlot] No building present to unlock on {gameObject.name}.", this);
            return;
        }

        Building building = _currentBuildingInstance.GetComponent<Building>();
        if (building == null)
        {
            Debug.LogError("[BuildingSlot] Current building instance missing Building component.", this);
            return;
        }

        if (!building.CanUnlock())
        {
            Debug.LogWarning($"[BuildingSlot] Cannot unlock building on {gameObject.name}.", this);
            return;
        }

        building.Unlock();
        GameEvents.TriggerOnObjectUnlocked(_currentBuildingInstance);
        OnUnlocked?.Invoke(_currentBuildingInstance);
    }
}

