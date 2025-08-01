using UnityEngine;

/// <summary>
/// Handles constructing and upgrading a building through simple interactions.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Entity))]
public class BuildingSlot : MonoBehaviour, IInteractable
{
    [Header("Building Data")]
    [Tooltip("Progression tree for this slot.")]
    public BuildingDataSO buildingData;

    private GameObject currentBuilding;
    private int currentLevel = 0;

    /// <summary>
    /// Gets the current building level. Zero means nothing built yet.
    /// </summary>
    public int CurrentLevel => currentLevel;

    /// <summary>
    /// Called when an interactor uses this slot. Builds the next level if possible.
    /// </summary>
    /// <param name="interactor">The interacting entity.</param>
    public void Interact(AutoInteractor interactor)
    {
        BuildNextLevel();
    }

    private void BuildNextLevel()
    {
        if (buildingData == null)
        {
            Debug.LogError("[BuildingSlot] BuildingDataSO not assigned.", this);
            return;
        }

        if (currentLevel >= buildingData.levels.Count)
        {
            Debug.Log($"[BuildingSlot] {gameObject.name} already at max level.", this);
            return;
        }

        BuildingLevelData levelData = buildingData.levels[currentLevel];
        if (ResourceManager.Instance != null && !ResourceManager.Instance.SpendResource(ResourceType.Gold, levelData.cost))
        {
            Debug.LogWarning("[BuildingSlot] Not enough resources to build.", this);
            return;
        }

        if (currentBuilding != null)
        {
            Destroy(currentBuilding);
        }

        if (levelData.prefab == null)
        {
            Debug.LogError("[BuildingSlot] Level prefab missing.", this);
            return;
        }

        currentBuilding = Instantiate(levelData.prefab, transform.position, transform.rotation, transform);

        Building buildingComponent = currentBuilding.GetComponent<Building>();
        if (buildingComponent != null)
        {
            buildingComponent.buildingData = buildingData;
            buildingComponent.SetOwner(this);
        }

        currentLevel++;

        if (currentLevel == 1)
        {
            GameEvents.TriggerOnObjectUnlocked(currentBuilding);
        }
        else
        {
            GameEvents.TriggerOnObjectUpgraded(currentBuilding, currentLevel);
        }

        Debug.Log($"[BuildingSlot] Built level {currentLevel} on {gameObject.name}.", this);
    }
}

