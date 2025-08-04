using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Configuration data for a <see cref="UnitSpawner"/> component.
/// </summary>
[CreateAssetMenu(fileName = "UnitSpawnerProfile", menuName = "TheFist/Unit Spawner Profile")]
public class UnitSpawnerProfileSO : ScriptableObject
{
    [Header("Spawner Settings")]
    [Tooltip("Prefab of the unit to spawn.")]
    public List<UnitSpawnerUnit> units;

    [Tooltip("Rate to spawn units, in seconds")]
    public float spawnRate = 5f;
}

[Serializable]
public class UnitSpawnerUnit
{
    public CharacterDefinitionSO unit;
    public float probability = 1f;
}