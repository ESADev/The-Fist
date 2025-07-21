using UnityEngine;

/// <summary>
/// Configuration data for a <see cref="UnitSpawner"/> component.
/// </summary>
[CreateAssetMenu(fileName = "UnitSpawnerProfile", menuName = "TheFist/Unit Spawner Profile")]
public class UnitSpawnerProfileSO : ScriptableObject
{
    [Header("Spawner Settings")]
    [Tooltip("Prefab of the unit to spawn.")]
    public GameObject unitPrefab;

    [Tooltip("Time in seconds between spawns.")]
    public float spawnRateInSeconds = 5f;
}
