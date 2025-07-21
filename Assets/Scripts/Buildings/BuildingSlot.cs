using UnityEngine;

/// <summary>
/// Manages construction and upgrades for a single building slot.
/// </summary>
[DisallowMultipleComponent]
public class BuildingSlot : MonoBehaviour
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
        currentBuilding.OnUpgraded?.Invoke(_currentBuildingInstance, currentBuilding.CurrentLevel + 1);

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
}

