using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the upgrade path and configuration for a building.
/// </summary>
[CreateAssetMenu(fileName = "BuildingData", menuName = "TheFist/Building Data")]
public class BuildingDataSO : ScriptableObject
{
    /// <summary>
    /// Display name of the building.
    /// </summary>
    [Tooltip("Display name of the building.")]
    public string buildingName = "New Building";

    /// <summary>
    /// List of level data defining prefab and cost for each level.
    /// </summary>
    [Tooltip("List of level data defining prefab and cost for each level.")]
    public List<BuildingLevelData> levels = new List<BuildingLevelData>();
}

/// <summary>
/// Data describing a single building level.
/// </summary>
[Serializable]
public class BuildingLevelData
{
    /// <summary>
    /// Cost required to unlock this level or upgrade to the next one.
    /// </summary>
    [Tooltip("Cost required to unlock this level or upgrade to the next one.")]
    public int cost = 0;

    /// <summary>
    /// Prefab representing this level of the building.
    /// </summary>
    [Tooltip("Prefab representing this level of the building.")]
    public GameObject prefab;
}
