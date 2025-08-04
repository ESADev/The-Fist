using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the upgrade path and configuration for a building.
/// </summary>
[CreateAssetMenu(fileName = "BuildingUpgradeTree", menuName = "TheFist/Building Upgrade Tree")]
public class BuildingUpgradeTreeSO : ScriptableObject
{
    /// <summary>
    /// List of level data defining prefab and cost for each level.
    /// </summary>
    [Tooltip("List of level data defining prefab and cost for each level.")]
    public List<CharacterDefinitionSO> levels = new List<CharacterDefinitionSO>();
}